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
			CLLocation dLocation = new CLLocation(drop.Location_Lat, drop.Location_Lnt);
			var distance = dLocation.DistanceFrom(uLocation);

			var imgScale = 1 - distance / Constants.VISIBILITY_LIMITATIN_M;
			var vdropSize = Constants.VDROP_MAX_SIZE * imgScale;

			var angle = DegreeBearing(uLocation, dLocation);
			var distanceScale = (distance / Constants.VISIBILITY_LIMITATIN_M) * (rootVC.View.Frame.Size.Width / 2);

			var posX = Math.Cos(angle * (Math.PI / 180.0)) * distanceScale + rootVC.View.Frame.Size.Width / 2;
			var posY = Math.Sin(angle * (Math.PI / 180.0)) * distanceScale + rootVC.View.Frame.Size.Height / 2;

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
			//throw new NotImplementedException();
		}

		#region calculator virtual drop location
		double DegreeBearing(CLLocation loc1, CLLocation loc2)
		{
			var dlon = ToRad(loc2.Coordinate.Longitude - loc1.Coordinate.Longitude);
			var dPhi = Math.Log(Math.Tan(ToRad(loc2.Coordinate.Latitude) / 2 + Math.PI / 4) / Math.Tan(ToRad(loc1.Coordinate.Latitude) / 2 + Math.PI / 4));
			if (Math.Abs(dlon) > Math.PI)
			{
				dlon = (dlon > 0) ? (dlon - 2 * Math.PI) : (2 * Math.PI + dlon);
			}
			return ToBearing(Math.Atan2(dlon, dPhi));
		}

		double ToRad(double degree)
		{
			return degree * (Math.PI / 180);
		}
		double ToBearing(double radian)
		{
			double degree = ToDegree(radian);
			return (degree + 360) % 360;
		}
		double ToDegree(double radian)
		{
			return radian * 180 / Math.PI;
		}
		#endregion
	}
}