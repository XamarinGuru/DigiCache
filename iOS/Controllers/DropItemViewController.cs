using Foundation;
using System;
using UIKit;

namespace Drop.iOS
{
    public partial class DropItemViewController : BaseViewController
	{
		public DropItemViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_ITEM)
		{
		}
        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var tap = new UITapGestureRecognizer(() => { View.EndEditing(true); });
			View.AddGestureRecognizer(tap);

			SetUISettings();
		}

		void SetUISettings()
		{
			heightName.Constant = 0;
			heightIcon.Constant = 0;
			heightPermission.Constant = 0;
			heightPassword.Constant = 0;
			heightExpiry.Constant = 0;
			viewName.Alpha = 0;
			viewIcon.Alpha = 0;
			viewPermission.Alpha = 0;
			viewPassword.Alpha = 0;
			viewExpiry.Alpha = 0;
		}

		partial void ActionColleps(UIButton sender)
		{
			this.View.LayoutIfNeeded();

			UIView.BeginAnimations("ds");
			UIView.SetAnimationDuration(0.5f);

			var constant = sender.Selected ? 0 : sender.Tag;
			var alpha = sender.Selected ? 0 : 1;
			switch (sender.Tag)
			{
				case Constants.TAG_COLLEPS_NAME:
					heightName.Constant = constant;
					viewName.Alpha = alpha;
					break;
				case Constants.TAG_COLLEPS_ICON:
					heightIcon.Constant = constant;
					viewIcon.Alpha = alpha;
					break;
				case Constants.TAG_COLLEPS_PERMISSION:
					heightPermission.Constant = constant;
					viewPermission.Alpha = alpha;
					break;
				case Constants.TAG_COLLEPS_PASSWORD:
					heightPassword.Constant = constant;
					viewPassword.Alpha = alpha;
					break;
				case Constants.TAG_COLLEPS_EXPIRY:
					heightExpiry.Constant = constant;
					viewExpiry.Alpha = alpha;
					break;
				default:
					break;
			}

			View.LayoutIfNeeded();
			UIView.CommitAnimations();

			sender.Selected = !sender.Selected;
		}
	}
}