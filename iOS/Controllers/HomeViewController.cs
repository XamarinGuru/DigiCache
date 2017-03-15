using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using CoreLocation;

using SDWebImage;
using Parse;

namespace Drop.iOS
{
    public partial class HomeViewController : BaseViewController
    {
		private IList<ParseItem> mDrops;

		public HomeViewController (IntPtr handle) : base (handle, Constants.STR_iOS_VCNAME_HOME)
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			foreach (UIView view in virtualDropContent.Subviews)
				view.RemoveFromSuperview();

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
					{
						if (ParseUser.CurrentUser == null)
						{
							ShowMessageBox(Constants.STR_INVALID_USERINFO, null);
							return;
						}
						pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_FAVORITE);
					}
					break;
				default:
					break;
			}

			NavigationController.PushViewController(pvc, true);
		}

		void LocationUpdated(object sender, EventArgs e)
		{
			foreach (UIView view in virtualDropContent.Subviews)
				view.RemoveFromSuperview();
			
			CLLocationsUpdatedEventArgs locArgs = e as CLLocationsUpdatedEventArgs;
			var currentLocation = locArgs.Locations[locArgs.Locations.Length - 1];

			for (int i = 0; i < mDrops.Count; i++)
			{
				var drop = mDrops[i];

				CLLocation pointB = new CLLocation(drop.Location_Lat, drop.Location_Lnt);
				var distanceToB = pointB.DistanceFrom(currentLocation);

				if (distanceToB < Constants.VISIBILITY_LIMITATIN_M)
					VisibleDrop(drop, currentLocation);
			}
		}

		void VisibleDrop(ParseItem drop, CLLocation cLocation)
		{
			var vDrop = VirtualDrop.Create();
			vDrop.SetView(drop, cLocation, this);

			virtualDropContent.AddSubview(vDrop);
		}
    }
}