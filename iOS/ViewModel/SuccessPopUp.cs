using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using CoreGraphics;

namespace Drop.iOS
{
    public partial class SuccessPopUp : UIView
    {
		Action ShareCallback;
		Action CancelCallback;

		public delegate void PopWillCloseHandler();
		public event PopWillCloseHandler PopWillClose;

		private UIVisualEffectView effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));

		public SuccessPopUp (IntPtr handle) : base (handle)
        {
        }
		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public static SuccessPopUp Create()
		{
			var arr = NSBundle.MainBundle.LoadNib("SuccessPopUp", null, null);
			var v = Runtime.GetNSObject<SuccessPopUp>(arr.ValueAt(0));

			var width = UIScreen.MainScreen.Bounds.Width * 0.95f;
			var height = width;
			var posX = (UIScreen.MainScreen.Bounds.Width - width) / 2;
			var posY = (UIScreen.MainScreen.Bounds.Height - height) / 2;
			v.Frame = new CGRect(posX, posY, width, height);
			v.effectView.Alpha = 0;

			return v;
		}

		public void PopUp(bool animated = true, Action shareCallback = null, Action cancelCallback = null)
		{
			ShareCallback = shareCallback;
			CancelCallback = cancelCallback;

			UIWindow window = UIApplication.SharedApplication.KeyWindow;
			effectView.Frame = window.Bounds;
			window.EndEditing(true);
			window.AddSubview(effectView);
			window.AddSubview(this);

			if (animated)
			{
				Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);
				UIView.Animate(0.15, delegate
				{
					Transform = CGAffineTransform.MakeScale(1, 1);
					effectView.Alpha = 0.8f;
				}, null);
			}
			else
			{
				effectView.Alpha = 0.8f;
			}
		}

		public void Close(bool animated = true)
		{
			if (animated)
			{
				UIView.Animate(0.15, delegate
				{
					Transform = CGAffineTransform.MakeScale(0.1f, 0.1f);
					effectView.Alpha = 0;
				}, delegate
				{
					this.RemoveFromSuperview();
					effectView.RemoveFromSuperview();
					if (null != PopWillClose) PopWillClose();
				});
			}
			else
			{
				if (null != PopWillClose) PopWillClose();
			}
		}

		partial void ActionShare(UIButton sender)
		{
			ShareCallback();
			Close();
		}

		partial void ActionClose(UIButton sender)
		{
			CancelCallback();
			Close();
		}
	}
}