using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using CoreGraphics;

namespace Drop.iOS
{
    public partial class FavoritePopUp : UIView
    {
        public delegate void PopWillCloseHandler();
		public event PopWillCloseHandler PopWillClose;

		private UIVisualEffectView effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));

		public FavoritePopUp(IntPtr handle) : base (handle)
        {
		}

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public static FavoritePopUp Create(bool isAdded)
		{
			var arr = NSBundle.MainBundle.LoadNib("FavoritePopUp", null, null);
			var v = Runtime.GetNSObject<FavoritePopUp>(arr.ValueAt(0));

			var width = UIScreen.MainScreen.Bounds.Width * 0.95f;
			var height = width * 7 / 9;
			var posX = (UIScreen.MainScreen.Bounds.Width - width) / 2;
			var posY = (UIScreen.MainScreen.Bounds.Height - height) / 2;
			v.Frame = new CGRect(posX, posY, width, height);
			v.effectView.Alpha = 0;

			v.lblTitle.Text = isAdded ? Constants.STR_FAVORITE_ADDED : Constants.STR_FAVORITE_DELETED;

			return v;
		}

		public void PopUp(bool animated = true)
		{
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

		partial void ActionClose(UIButton sender)
		{
			Close();
		}
    }
}