using System;
using UIKit;
using BigTed;
using System.Threading.Tasks;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;
using System.Drawing;
using AVFoundation;

namespace Drop.iOS
{
	public partial class BaseViewController : UIViewController
	{
		public static Boolean isAttachedPurchaseCallbacks = false;

		private AVCaptureSession captureSession;
		private AVCaptureDeviceInput captureDeviceInput;
		private AVCaptureStillImageOutput stillImageOutput;
		private AVCaptureVideoPreviewLayer videoPreviewLayer;
		private UIView contentView;


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

			var leftButton = new UIButton(new CGRect(0, 0, 40, 40));
			leftButton.SetImage(UIImage.FromFile("icon_back.png"), UIControlState.Normal);
			leftButton.TouchUpInside += (sender, e) => NavigationController.PopViewController(true);
			NavigationItem.LeftBarButtonItem = new UIBarButtonItem(leftButton);

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

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			if (!isAttachedPurchaseCallbacks)
			{
				isAttachedPurchaseCallbacks = true;
				AttachToPurchaseManager();
			}

			await AuthorizeCameraUse();
			SetupLiveCameraStream();
		}

		public void AttachToPurchaseManager()
		{
			AppDelegate.PurchaseManager.ReceivedValidProducts += (products) => { };
			AppDelegate.PurchaseManager.InAppProductPurchaseUserCanceled += (transaction, product) =>
			{
				ShowMessageBox("Xamarin.InAppPurchase", String.Format("Attempt to purchase {0} has Cancelled: {1}", product.Title, transaction.Error.ToString()));
			};
			AppDelegate.PurchaseManager.InAppProductPurchaseFailed += (transaction, product) =>
			{
				ShowMessageBox("Xamarin.InAppPurchase", String.Format("Attempt to purchase {0} has failed: {1}", product.Title, transaction.Error.ToString()));
			};
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
			NavigationController.View.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.BackgroundColor = UIColor.Clear;
			NavigationController.NavigationBar.ShadowImage = new UIImage();

			NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIDeviceOrientationDidChangeNotification"), DeviceRotated);

			if (captureSession != null && !captureSession.Running)
				captureSession.StartRunning();
		}

		#region live camera background
		private async Task AuthorizeCameraUse()
		{
			var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

			if (authorizationStatus != AVAuthorizationStatus.Authorized)
			{
				await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
			}
		}

		public void SetupLiveCameraStream()
		{
			captureSession = new AVCaptureSession();

			videoPreviewLayer = new AVCaptureVideoPreviewLayer(captureSession)
			{
				Frame = this.View.Frame
			};

			contentView = new UIView(this.View.Frame);
			contentView.Layer.AddSublayer(videoPreviewLayer);

			this.View.AddSubview(contentView);

			this.View.SendSubviewToBack(contentView);

			var captureDevice = AVCaptureDevice.DefaultDeviceWithMediaType(AVMediaType.Video);
			ConfigureCameraForDevice(captureDevice);
			captureDeviceInput = AVCaptureDeviceInput.FromDevice(captureDevice);
			captureSession.AddInput(captureDeviceInput);

			var dictionary = new NSMutableDictionary();
			dictionary[AVVideo.CodecKey] = new NSNumber((int)AVVideoCodec.JPEG);
			stillImageOutput = new AVCaptureStillImageOutput()
			{
				OutputSettings = new NSDictionary()
			};

			captureSession.AddOutput(stillImageOutput);
			captureSession.StartRunning();
		}

		private void ConfigureCameraForDevice(AVCaptureDevice device)
		{
			var error = new NSError();
			if (device.IsFocusModeSupported(AVCaptureFocusMode.ContinuousAutoFocus))
			{
				device.LockForConfiguration(out error);
				device.FocusMode = AVCaptureFocusMode.ContinuousAutoFocus;
				device.UnlockForConfiguration();
			}
			else if (device.IsExposureModeSupported(AVCaptureExposureMode.ContinuousAutoExposure))
			{
				device.LockForConfiguration(out error);
				device.ExposureMode = AVCaptureExposureMode.ContinuousAutoExposure;
				device.UnlockForConfiguration();
			}
			else if (device.IsWhiteBalanceModeSupported(AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance))
			{
				device.LockForConfiguration(out error);
				device.WhiteBalanceMode = AVCaptureWhiteBalanceMode.ContinuousAutoWhiteBalance;
				device.UnlockForConfiguration();
			}
		}
		#endregion


		private void DeviceRotated(NSNotification notification)
		{
			if (contentView != null)
				contentView.Frame = this.View.Frame;

			if (videoPreviewLayer != null)
				videoPreviewLayer.Frame = this.View.Frame;
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

		protected void SetDatePicker(UITextField field, UIDatePickerMode mode = UIDatePickerMode.Time, String format = "{0: d MMMM yyyy}", bool futureDatesOnly = true, DateTime? minimumDateTime = null, bool changeOnEdit = false)
		{
			UIDatePicker picker = new UIDatePicker();
			picker.Mode = UIDatePickerMode.Date;

			picker.MaximumDate = ToNSDate(DateTime.Now.AddYears(1));
			if (minimumDateTime != null)
			{
				NSDate nsMinDateTime = ToNSDate((DateTime)minimumDateTime);
				picker.MinimumDate = nsMinDateTime;
			}
			if (futureDatesOnly)
			{
				NSDate nsMinDateTime = ToNSDate(DateTime.Now);
				//picker.SetDate
				picker.MinimumDate = nsMinDateTime;
			}

			picker.ValueChanged += (object s, EventArgs e) =>
			{
				if (futureDatesOnly)
				{
					NSDate nsMinDateTime = ToNSDate(DateTime.Now);
					picker.MinimumDate = nsMinDateTime;
				}
				if (changeOnEdit)
				{
					updateSetupDateTimePicker(field, picker.Date, format, s, e);
				}
			};

			// Setup the toolbar
			UIToolbar toolbar = new UIToolbar();
			toolbar.BarStyle = UIBarStyle.Black;
			toolbar.Translucent = true;
			toolbar.SizeToFit();

			// Create a 'done' button for the toolbar and add it to the toolbar
			UIBarButtonItem doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				updateSetupDateTimePicker(field, picker.Date, format, s, e, true);
			});

			toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);

			field.InputView = picker;
			field.InputAccessoryView = toolbar;

			field.ShouldChangeCharacters = new UITextFieldChange(delegate (UITextField textField, NSRange range, string replacementString)
			{
				return false;
			});
		}

		private void updateSetupDateTimePicker(UITextField field, NSDate date, String format, object sender, EventArgs e, bool done = false)
		{
			var newDate = NSDateToDateTime(date);

			var str = String.Format(format, newDate);

			field.Text = str;
			field.SendActionForControlEvents(UIControlEvent.ValueChanged);
			if (done)
			{
				field.ResignFirstResponder();
			}
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
			var imageData = image.AsPNG();
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

		public UIImage MaxResizeImage(UIImage sourceImage)
		{
			try
			{
				var sourceSize = sourceImage.Size;
				var maxResizeFactor = Math.Max(Constants.MDROP_MAX_SIZE / sourceSize.Width, Constants.MDROP_MAX_SIZE / sourceSize.Height);
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