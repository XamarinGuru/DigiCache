using Foundation;
using System;
using UIKit;

using Google.Maps;
using CoreGraphics;
using System.Drawing;
using CoreLocation;
using System.Collections.Generic;

namespace Drop.iOS
{
    public partial class NearbyViewController : BaseViewController
    {
		private MapView mMapView;
		private IList<ParseItem> mDrops;

        public NearbyViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_NEARBY)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var lResult = LocationHelper.GetLocationResult();
			var camera = CameraPosition.FromCamera(lResult.Latitude, lResult.Longitude, zoom: 7);
			mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			mMapView.MyLocationEnabled = false;
			mMapView.Alpha = 0.9f;
			mMapView.TappedMarker = ClickedDropItem;
		}

		void GetDrops()
		{
			ShowLoadingView(Constants.STR_DROPS_LOADING);

			mDrops = ParseService.GetDropItems();

			for (int i = 0; i < mDrops.Count; i ++)
			{
				var drop = mDrops[i];
				var iconData = NSData.FromUrl(new NSUrl(drop.IconURL.ToString()));
				var marker = new Marker
				{
					Position = new CLLocationCoordinate2D(drop.Location_Lat, drop.Location_Lnt),
					Map = mMapView,
					Icon = UIImage.LoadFromData(iconData),
					ZIndex = i
				};
			}

			HideLoadingView();
		}

		bool ClickedDropItem(MapView mapView, Marker marker)
		{
			DropDetailViewController pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_DETAIL) as DropDetailViewController;
			pvc.parseItem = mDrops[marker.ZIndex];
			NavigationController.PushViewController(pvc, true);

			return true;
		}

		public override void ViewWillLayoutSubviews()
		{
			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			GetDrops();
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
    }
}