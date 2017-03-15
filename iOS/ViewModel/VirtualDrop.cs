using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using CoreGraphics;
using SDWebImage;
using CoreLocation;

namespace Drop.iOS
{
	public partial class VirtualDrop : UIView
	{
		ParseItem mDrop;
		HomeViewController rootVC;

		public VirtualDrop(IntPtr handle) : base(handle)
		{
		}

		public static VirtualDrop Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("VirtualDrop", null, null);
			var v = Runtime.GetNSObject<VirtualDrop>(arr.ValueAt(0));
			return v;
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public void SetView(ParseItem drop, CLLocation uLocation, HomeViewController rootVC)
		{
			mDrop = drop;
			this.rootVC = rootVC;

			CLLocation dLocation = new CLLocation(drop.Location_Lat, drop.Location_Lnt);
			var distance = dLocation.DistanceFrom(uLocation);

			var imgScale = 1 - distance / Constants.PURCHASE_DISTANCE;
			var vdropSize = Constants.VDROP_MAX_SIZE * imgScale;

			var angle = DegreeBearing(uLocation, dLocation);
			var distanceScale = (distance / Constants.PURCHASE_DISTANCE) * (rootVC.View.Frame.Size.Width / 2);

			var posX = rootVC.View.Frame.Size.Width / 2 + Math.Sin(angle) * distanceScale - vdropSize / 2;
			var posY = rootVC.View.Frame.Size.Height / 2 - Math.Cos(angle) * distanceScale - vdropSize / 2;

			var frame = new CGRect(posX, posY, vdropSize, vdropSize);

			this.Frame = frame;

			lblDistance.Text = distance.ToString("F2") + " m";

			imgIcon.SetImage(
				url: new NSUrl(drop.IconURL.ToString()),
				placeholder: UIImage.FromBundle("icon_drop1.png")
			);

			LayoutIfNeeded();
		}

		partial void ActionDetail(UIButton sender)
		{
			var pvc = rootVC.GetVCWithIdentifier(Constants.STR_iOS_VCNAME_NEARBY) as NearbyViewController;
			rootVC.NavigationController.PushViewController(pvc, true);
		}

		#region calculator virtual drop location
		double DegreeBearing(CLLocation loc1, CLLocation loc2)
		{;
			double fLat = ToRad(loc1.Coordinate.Latitude);
			double fLng = ToRad(loc1.Coordinate.Longitude);
			double tLat = ToRad(loc2.Coordinate.Latitude);
			double tLng = ToRad(loc2.Coordinate.Longitude);

			double dLon = tLng - fLng;
			double y = Math.Sin(dLon) * Math.Cos(tLat);
			double x = Math.Cos(fLat) * Math.Sin(tLat) - Math.Sin(fLat) * Math.Cos(tLat) * Math.Cos(dLon);
			double radian = Math.Atan2(y, x);
			return (radian);
		}

		double ToRad(double degree)
		{
			return degree * (Math.PI / 180);
		}
		double ToDegree(double radian)
		{
			return radian * 180 / Math.PI;
		}
		#endregion
	}
}