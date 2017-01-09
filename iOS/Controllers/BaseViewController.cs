using System;
using UIKit;
using BigTed;
using System.Threading.Tasks;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;

namespace Drop.iOS
{

	public partial class BaseViewController : UIViewController
	{
		public BaseViewController(string title = "") : base()
		{
			Init(title);
		}
		public BaseViewController(IntPtr handle, string title = "") : base(handle)
		{
			Init(title);
		}

		protected virtual void Init(string title)
		{
			this.NavigationController.NavigationBar.SetBackgroundImage(UIImage.FromFile("bar_top.png"), UIBarMetrics.Default);
			this.NavigationController.View.BackgroundColor = UIColor.Clear;
			this.NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;
			this.NavigationController.NavigationBar.ShadowImage = new UIImage();

			NavigationItem.HidesBackButton = true;

			switch (title)
			{
				case Constants.STR_iOS_VCNAME_LOGIN:
					break;
				case Constants.STR_iOS_VCNAME_HOME:
					NavigationItem.LeftBarButtonItem = null;
					break;
			}
		}

		public UIViewController GetVCWithIdentifier(string identifier)
		{
			UIStoryboard sb = UIStoryboard.FromName("Main", null);
			UIViewController pvc = sb.InstantiateViewController(identifier);
			return pvc;
		}

		public void ShowLoadingView(string title)
		{
			InvokeOnMainThread(() => { BTProgressHUD.Show(title, -1, ProgressHUD.MaskType.Black); });
		}

		public void HideLoadingView()
		{
			InvokeOnMainThread(() => { BTProgressHUD.Dismiss(); });
		}


		// Show the alert view
		public void ShowMessageBox(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			var alertView = new UIAlertView(title, message, null, cancelButton, otherButtons);
			alertView.Clicked += (sender, e) =>
			{
				if (e.ButtonIndex == 0)
				{
					return;
				}
				if (successHandler != null)
				{
					successHandler();
				}
			};
			alertView.Show();
		}

		//overloaded method
		protected void ShowMessageBox(string title, string message)
		{
			InvokeOnMainThread(() => { ShowMessageBox(title, message, "Ok", null, null); });
		}

		protected bool TextFieldShouldReturn(UITextField textField)
		{
			textField.ResignFirstResponder();
			return true;
		}

		protected static DateTime NSDateToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(2001, 1, 1, 0, 0, 0));
			reference = reference.AddSeconds(date.SecondsSinceReferenceDate);
			if (reference.IsDaylightSavingTime())
			{
				reference = reference.AddHours(1);
			}
			return reference;
		}

		//From Xamarin
		public static DateTime ToDateTime(NSDate date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return reference.AddSeconds(date.SecondsSinceReferenceDate);
		}

		//From Xamarin
		public static NSDate ToNSDate(DateTime date)
		{
			DateTime reference = TimeZone.CurrentTimeZone.ToLocalTime(
				new DateTime(2001, 1, 1, 0, 0, 0));
			return NSDate.FromTimeIntervalSinceReferenceDate(
				(date - reference).TotalSeconds + 3600 * 1);
		}

		protected void SetButtonStyle(UIButton button)
		{
			button.Layer.CornerRadius = 3;
			button.TintColor = UIColor.Clear;
			button.BackgroundColor = UIColor.Clear;
			button.Layer.BackgroundColor = UIColor.Clear.CGColor;
			button.ImageView.Image = new UIImage();
			button.ImageView.BackgroundColor = UIColor.Clear;//UIColor.FromRGBA(22.0f / 255, 38.0f / 255, 75.0f / 255, 0.2f).CGColor;
		}

		protected UIImage rotateImage(UIImage sourceImage, float rotate)
		{
			float rads = (float)Math.PI * rotate / 180;
			UIImage returnImage;

			using (CGImage imageRef = sourceImage.CGImage)
			{
				UIView rotatedViewBox = new UIView(new CGRect(0, 0, imageRef.Width, imageRef.Height));

				CGAffineTransform t = CGAffineTransform.MakeIdentity();
				t.Rotate(rads);
				rotatedViewBox.Transform = t;
				CGSize rotatedSize = rotatedViewBox.Frame.Size;


				UIGraphics.BeginImageContextWithOptions(rotatedSize, false, sourceImage.CurrentScale);
				var context = UIGraphics.GetCurrentContext();
				context.TranslateCTM(rotatedSize.Width / 2, rotatedSize.Height / 2);
				context.RotateCTM(rads);
				context.ScaleCTM(1.0f, -1.0f);
				var rect = new CGRect(-sourceImage.Size.Width / 2, -sourceImage.Size.Height / 2, sourceImage.Size.Width, sourceImage.Size.Height);
				context.DrawImage(rect, sourceImage.CGImage);
				//context.FillRect(rect);

				returnImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
			}

			return returnImage;
		}
	}
}