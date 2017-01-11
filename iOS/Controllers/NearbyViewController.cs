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
		private MapView mapView;
		private IList<ParseItem> mDrops;

        public NearbyViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_NEARBY)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var lResult = LocationHelper.GetLocationResult();
			var camera = CameraPosition.FromCamera(lResult.Latitude, lResult.Longitude, zoom: 7);
			mapView = MapView.FromCamera(RectangleF.Empty, camera);
			mapView.MyLocationEnabled = false;
			mapView.Alpha = 0.9f;
			mapView.TappedMarker = ClickedDropItem;

			GetDrops();
		}

		void GetDrops()
		{
			ShowLoadingView(Constants.STR_DROPS_LOADING);

			mDrops = ParseService.GetDropItems();

			//foreach (var drop in drops)
			for (int i = 0; i < mDrops.Count; i ++)
			{
				var drop = mDrops[i];
				var iconData = NSData.FromUrl(new NSUrl(drop.IconURL.ToString()));
				var marker = new Marker
				{
					Position = new CLLocationCoordinate2D(drop.Location_Lat, drop.Location_Lnt),
					Map = mapView,
					Icon = UIImage.LoadFromData(iconData),
					ZIndex = i
				};
			}

			HideLoadingView();
		}

		bool ClickedDropItem(MapView mapView, Marker marker)
		{
			var dropItem = mDrops[marker.ZIndex];
			return true;
		}

		public override void ViewWillLayoutSubviews()
		{
			if (mapView != null && viewMapContent != null && viewMapContent.Window != null)
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
			mapView.Frame = new CGRect(0, 0, width, height);

			viewMapContent.AddSubview(mapView);
		}
    }
}