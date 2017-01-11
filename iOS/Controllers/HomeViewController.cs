using Foundation;
using System;
using UIKit;
using CoreGraphics;

namespace Drop.iOS
{
    public partial class HomeViewController : BaseViewController
    {
		public HomeViewController (IntPtr handle) : base (handle, Constants.STR_iOS_VCNAME_HOME)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			dropContent.Transform = CGAffineTransform.MakeScale(0, 0);
			dropContent.Alpha = 0;
		}

		partial void ActionHome(UIButton sender)
		{
			var scale = sender.Selected ? 0 : 1;
			var alpha = sender.Selected ? 0 : 1;

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

    }
}