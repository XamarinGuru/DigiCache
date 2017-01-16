using System;
using UIKit;
using BigTed;
using System.Threading.Tasks;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;
using System.Drawing;

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
			NavigationItem.HidesBackButton = true;

			var leftButton = new UIButton(new CGRect(0, 0, 65, 35));
			leftButton.SetImage(UIImage.FromFile("btn_back.png"), UIControlState.Normal);
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

			var rightButton = new UIButton(new CGRect(0, 0, 50, 35));
			rightButton.SetImage(UIImage.FromFile("btn_right.png"), UIControlState.Normal);
			rightButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(rightButton);

			switch (title)
			{
				case Constants.STR_iOS_VCNAME_LOGIN:
				case Constants.STR_iOS_VCNAME_HOME:
					NavigationItem.LeftBarButtonItem = null;
					NavigationItem.RightBarButtonItem = null;
					break;
				case Constants.STR_iOS_VCNAME_ITEM:
				case Constants.STR_iOS_VCNAME_NEARBY:
				case Constants.STR_iOS_VCNAME_LOCATION:
					NavigationItem.RightBarButtonItem = null;
					break;
				case Constants.STR_iOS_VCNAME_DETAIL:
					break;
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			NavigationController.NavigationBar.SetBackgroundImage(UIImage.FromFile("bar_top.png"), UIBarMetrics.Default);
			NavigationController.View.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.ShadowImage = new UIImage();
		}

		public UIViewController GetVCWithIdentifier(string identifier)
		{
			UIStoryboard sb = UIStoryboard.FromName("Main", null);
			var pvc = sb.InstantiateViewController(identifier) as UIViewController;
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

		public void ShowTextFieldBox(string title, string cancelButton, string[] otherButtons, Action<string> callback)
		{
			var alertView = new UIAlertView(title, null, null, cancelButton, otherButtons);

			alertView.AlertViewStyle = UIAlertViewStyle.PlainTextInput;

			alertView.Clicked += (sender, e) =>
			{
				if (e.ButtonIndex == 1)
					callback(alertView.GetTextField(0).Text);
			};
			alertView.Show();
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

		protected MediaFile ByteDataFromImage(UIImage image)
		{
			var imageData = image.AsJPEG(0.5f);
			var fileBytes = new Byte[imageData.Length];
			System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, fileBytes, 0, Convert.ToInt32(imageData.Length));

			var fileName = "Image_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".png";

			return new MediaFile(fileName, fileBytes);
		}

		protected MediaFile ByteDataFromVideoURL(NSUrl mediaURL)
		{
			NSData videoData = NSData.FromUrl(mediaURL);
			Byte[] fileBytes = new Byte[videoData.Length];
			System.Runtime.InteropServices.Marshal.Copy(videoData.Bytes, fileBytes, 0, Convert.ToInt32(videoData.Length));

			var fileName = "Video_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".mp4";

			return new MediaFile(fileName, fileBytes);
		}

		public UIImage MaxResizeImage(UIImage sourceImage, float maxWidth = 30, float maxHeight = 30)
		{
			try
			{
				var sourceSize = sourceImage.Size;
				var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
				if (maxResizeFactor > 1) return sourceImage;
				var width = maxResizeFactor * sourceSize.Width;
				var height = maxResizeFactor * sourceSize.Height;
				UIGraphics.BeginImageContext(new SizeF((float)width, (float)height));
				sourceImage.Draw(new RectangleF(0, 0, (float)width, (float)height));
				var resultImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();
				return resultImage;
			}
			catch
			{
				return null;
			}
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