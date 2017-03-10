using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using CoreLocation;

using SDWebImage;

namespace Drop.iOS
{
    public partial class HomeViewController : BaseViewController
    {
		private IList<ParseItem> mDrops;

		public HomeViewController (IntPtr handle) : base (handle, Constants.STR_iOS_VCNAME_HOME)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//dropContent.Transform = CGAffineTransform.MakeScale(0, 0);
			//dropContent.Alpha = 0;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			GetDrops();
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			LocationHelper.LocationUpdated -= LocationUpdated;
		}

		void GetDrops()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.STR_DROPS_LOADING);

				mDrops = ParseService.GetDropItems();

				HideLoadingView();

				if (mDrops.Count == 0)
					LocationHelper.LocationUpdated -= LocationUpdated;
				else
					LocationHelper.LocationUpdated += LocationUpdated;
			});
		}

		partial void ActionHome(UIButton sender)
		{
			var scale = sender.Selected ? 1 : 0;
			var alpha = sender.Selected ? 1 : 0;

			UIView.Animate(0.3f, 0, UIViewAnimationOptions.Autoreverse | UIViewAnimationOptions.CurveEaseOut, () =>
		    {
			    dropContent.Transform = CGAffineTransform.MakeScale(scale, scale);
				dropContent.Alpha = alpha;
		    }, null);

			sender.Selected = !sender.Selected;
		}

		partial void ActionDrop(UIButton sender)
		{
			UIViewController pvc = new UIViewController();

			switch (sender.Tag)
			{
				case Constants.TAG_DROP_ITEM:
					pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_ITEM);
					break;
				case Constants.TAG_DROP_NEARBY:
					pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_NEARBY);
					break;
				case Constants.TAG_DROP_SETTING:
					pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_HOME);
					break;
				default:
					break;
			}

			NavigationController.PushViewController(pvc, true);
		}

		void LocationUpdated(object sender, EventArgs e)
		{
			CLLocationsUpdatedEventArgs locArgs = e as CLLocationsUpdatedEventArgs;
			var currentLocation = locArgs.Locations[locArgs.Locations.Length - 1];

			for (int i = 0; i < mDrops.Count; i++)
			{
				var drop = mDrops[i];

				CLLocation pointB = new CLLocation(drop.Location_Lat, drop.Location_Lnt);
				var distanceToB = pointB.DistanceFrom(currentLocation);

				if (distanceToB < Constants.VISIBILITY_LIMITATIN_M)
					VisibleDrop(drop, distanceToB);
			}
		}

		void VisibleDrop(ParseItem drop, double distance)
		{
			foreach (UIView view in virtualDropContent.Subviews)
				view.RemoveFromSuperview();
			
			var scale = 1 - distance / Constants.VISIBILITY_LIMITATIN_M;

			if (scale <= 0)
				return;

			var vdropSize = Constants.VDROP_MAX_SIZE * scale;
			var posX = View.Frame.Size.Width / 2 - vdropSize / 2;
			var posY = View.Frame.Size.Height / 2 - vdropSize / 2;

			var realDrop = new UIImageView(new CGRect(posX, posY, vdropSize, vdropSize));
			realDrop.SetImage(
				url: new NSUrl(drop.IconURL.ToString()),
				placeholder: UIImage.FromBundle("icon_drop1.png")
			);

			realDrop.ContentMode = UIViewContentMode.ScaleAspectFill;

			UILabel lblDistance = new UILabel(new CGRect(10, 100, 500, 100));
			lblDistance.Text = distance.ToString();
			lblDistance.TextColor = UIColor.Red;

			virtualDropContent.AddSubview(realDrop);
			virtualDropContent.AddSubview(lblDistance);
		}
    }
}