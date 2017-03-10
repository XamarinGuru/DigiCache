using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using CoreGraphics;

namespace Drop.iOS
{
    public partial class PurchasePopUp : UIView
    {
		Constants.PURCHASE_TYPE mType;
		Action PurchaseCallBack;

		public delegate void PopWillCloseHandler();
		public event PopWillCloseHandler PopWillClose;

		private UIVisualEffectView effectView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));

        public PurchasePopUp (IntPtr handle) : base (handle)
        {
        }

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
		}

		public static PurchasePopUp Create(Constants.PURCHASE_TYPE type)
		{
			var arr = NSBundle.MainBundle.LoadNib("PurchasePopUp", null, null);
			var v = Runtime.GetNSObject<PurchasePopUp>(arr.ValueAt(0));

			v.mType = type;

			var width = UIScreen.MainScreen.Bounds.Width * 0.95f;
			var height = width * 7 / 9;
			var posX = (UIScreen.MainScreen.Bounds.Width - width) / 2;
			var posY = (UIScreen.MainScreen.Bounds.Height - height) / 2;
			v.Frame = new CGRect(posX, posY, width, height);
			v.effectView.Alpha = 0;

			v.lblTitle.Text = Constants.PURCHASE_TITLE[(int)type];
			v.lblDescription1.Text = Constants.PURCHASE_DESCRIPTION1[(int)type];
			v.lblDescription2.Text = Constants.PURCHASE_DESCRIPTION2[(int)type];
			v.lblBtnTitle.Text = Constants.PURCHASE_BUTTON[(int)type];

			return v;
		}

		public void PopUp(bool animated = true, Action callBack = null)
		{
			PurchaseCallBack = callBack;

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

		partial void ActionPurchase(UIButton sender)
		{
			PurchaseCallBack();
			Close();
		}

		partial void ActionClose(UIButton sender)
		{
			Close();
		}
	}
}
