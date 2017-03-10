using Foundation;
using System;
using UIKit;
using System.Drawing;
using CoreGraphics;
using MapKit;
using CoreLocation;
using System.Text;
using AddressBook;
using Google.Maps;

namespace Drop.iOS
{
    public partial class DropLocationViewController : BaseViewController
    {
		public ItemModel ItemModel;

		private MapView mMapView;

		public DropLocationViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_LOCATION)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitMapView();
		}

		public override void ViewWillLayoutSubviews()
		{
			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}

		void InitMapView()
		{
			var lResult = LocationHelper.GetLocationResult();
			var camera = CameraPosition.FromCamera(lResult.Latitude, lResult.Longitude, zoom: Constants.MAP_ZOOM_LEVEL);
			mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			mMapView.MyLocationEnabled = false;
			mMapView.MapType = MapViewType.Satellite;
			mMapView.Alpha = 0.8f;
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

		void SetMapPin(double lat, double lng)
		{
			InvokeOnMainThread(() =>
			{
				var camera = CameraPosition.FromCamera(lat, lng, zoom: 18);
				mMapView = MapView.FromCamera(RectangleF.Empty, camera);
			});
		}

		partial void ActionConfirmLocation(UIButton sender)
		{
			ItemModel.Location_Lat = mMapView.Camera.Target.Latitude;
			ItemModel.Location_Lnt = mMapView.Camera.Target.Longitude;

			NavigationController.PopViewController(true);
		}
    }
}