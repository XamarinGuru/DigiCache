using Foundation;
using System;
using UIKit;

using Google.Maps;
using CoreGraphics;
using System.Drawing;
using CoreLocation;
using System.Collections.Generic;
using StoreKit;
using Xamarin.InAppPurchase;
using Parse;

namespace Drop.iOS
{
    public partial class NearbyViewController : BaseViewController
    {
		private MapView mMapView;
		private IList<ParseItem> mDrops;
		private ParseItem mSelectedDrop;

        public NearbyViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_NEARBY)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitMapView();

			GetDrops();

			AppDelegate.PurchaseManager.InAppProductPurchased -= PurchaseSuccessCallback;
			AppDelegate.PurchaseManager.InAppProductPurchased += PurchaseSuccessCallback;
		}

		#region in-app purchase
		void PurchaseSuccessCallback(SKPaymentTransaction transaction, InAppProduct product)
		{
			if (product.ProductIdentifier == Constants.PURCHASE_ID[(int)Constants.PURCHASE_TYPE.VIEW])
			{
				ViewDropDetail();
			}
		}
		void OpenPurchase()
		{
			InAppProduct product = AppDelegate.PurchaseManager.FindProduct(Constants.PURCHASE_ID[(int)Constants.PURCHASE_TYPE.DROP]);
			AppDelegate.PurchaseManager.BuyProduct(product);
		}
		#endregion

  		#region google map
		public override void ViewWillLayoutSubviews()
		{
			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}
		public void RepaintMap()
		{
			foreach (var subview in viewMapContent.Subviews)
			{
				subview.RemoveFromSuperview();
			}

			viewMapContent.LayoutIfNeeded();
			var width = viewMapContent.Frame.Width;
			var height = viewMapContent.Frame.Height;
			mMapView.Frame = new CGRect(0, 0, width, height);

			viewMapContent.AddSubview(mMapView);
		}
		void InitMapView()
		{
			var lResult = LocationHelper.GetLocationResult();
			var camera = CameraPosition.FromCamera(lResult.Latitude, lResult.Longitude, zoom: Constants.MAP_ZOOM_LEVEL);
			mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			mMapView.MyLocationEnabled = false;
			mMapView.MapType = MapViewType.Satellite;
			mMapView.Alpha = 0.8f;
			mMapView.TappedMarker = ClickedDropItem;
		}
		#endregion

		void GetDrops()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.STR_DROPS_LOADING);

				mDrops = ParseService.GetDropItems();

				for (int i = 0; i < mDrops.Count; i++)
				{
					var drop = mDrops[i];
					if (drop.IsVisibilityByUser())
						AddDropInMap(drop, i);
				}

				HideLoadingView();
			});
		}

		void AddDropInMap(ParseItem drop, int index)
		{
			var iconData = NSData.FromUrl(new NSUrl(drop.IconURL.ToString()));

			InvokeOnMainThread(() =>
			{
				var marker = new Marker
				{
					Position = new CLLocationCoordinate2D(drop.Location_Lat, drop.Location_Lnt),
					Map = mMapView,
					Icon = UIImage.LoadFromData(iconData),
					ZIndex = index
				};
			});
		}

		bool ClickedDropItem(MapView mapView, Marker marker)
		{
			mSelectedDrop = mDrops[marker.ZIndex];
			if (mSelectedDrop.Password == string.Empty || mSelectedDrop.Password == null)
			{
				var currentLocation = LocationHelper.GetLocationResult();
				CLLocation dLocation = new CLLocation(mSelectedDrop.Location_Lat, mSelectedDrop.Location_Lnt);
				CLLocation cLocation = new CLLocation(currentLocation.Latitude, currentLocation.Longitude);
				var distance = dLocation.DistanceFrom(cLocation);

				if (distance > Constants.PURCHASE_DISTANCE)
				{
					PurchasePopUp pPopup = PurchasePopUp.Create(Constants.PURCHASE_TYPE.VIEW);
					pPopup.PopUp(true, OpenPurchase);
				}
				else
				{
					ViewDropDetail();
				}
			}
			else {
				ShowTextFieldBox(Constants.STR_VERIFY_PASSWORD_TITLE, "Cancel", new[] { "OK" }, VerifyPassword);
			}

			return true;
		}

		void VerifyPassword(string text)
		{
			if (mSelectedDrop.Password == text)
			{
				ViewDropDetail();
			}
			else {
				ShowMessageBox(null, Constants.STR_INVALID_PASSWORD_TITLE);
			}
		}

		void ViewDropDetail()
		{
			DropDetailViewController pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_DETAIL) as DropDetailViewController;
			pvc.parseItem = mSelectedDrop;
			NavigationController.PushViewController(pvc, true);
		}
    }
}