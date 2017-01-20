using Foundation;
using System;
using UIKit;
using GalaSoft.MvvmLight.Helpers;
using MobileCoreServices;

namespace Drop.iOS
{
    public partial class DropItemViewController : BaseViewController
	{
		private ItemModel ItemModel { get; set; }

		private UIImagePickerController mMediaPicker;

		public DropItemViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_ITEM)
		{
			ItemModel = new ItemModel();
		}

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var tap = new UITapGestureRecognizer(() => { View.EndEditing(true); });
			View.AddGestureRecognizer(tap);

			SetInitialSettings();
			SetInputBinding();
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			lblLocationLat.Text = ItemModel.Location_Lat.ToString();
			lblLocationLog.Text = ItemModel.Location_Lnt.ToString();
		}

		void SetInitialSettings()
		{
			mMediaPicker = new UIImagePickerController();
			mMediaPicker.Canceled += PickupMediaCanceledHandler;

			heightName.Constant = 0;
			heightIcon.Constant = 0;
			heightLocation.Constant = 0;
			heightPermission.Constant = 0;
			heightPassword.Constant = 0;
			heightExpiry.Constant = 0;
			viewName.Alpha = 0;
			viewIcon.Alpha = 0;
			viewLocation.Alpha = 0;
			viewPermission.Alpha = 0;
			viewPassword.Alpha = 0;
			viewExpiry.Alpha = 0;

			SetDatePicker(txtExpireDate);
		}



		private void SetInputBinding()
		{
			this.SetBinding(() => ItemModel.Name, () => txtName.Text, BindingMode.TwoWay);
			this.SetBinding(() => ItemModel.Description, () => txtDescription.Text, BindingMode.TwoWay);

			this.SetBinding(() => ItemModel.Password, () => txtPassword.Text, BindingMode.TwoWay);

			this.SetBinding(() => ItemModel.ExpiryDate, () => txtExpireDate.Text, BindingMode.OneWay);
			this.SetBinding(() => txtExpireDate.Text, () => ItemModel.ExpiryDate, BindingMode.OneWay).ObserveSourceEvent("ValueChanged");
			//this.SetBinding(() => mText, () => ItemModel.ExpiryDate, BindingMode.OneWay);
		}



		#region Actions
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
				case Constants.TAG_COLLEPS_LOCATION:
					heightLocation.Constant = constant;
					viewLocation.Alpha = alpha;
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

		partial void ActionItem(UIButton sender)
		{
			var actionSheet = new UIActionSheet(Constants.STR_ATTACH_TITLE, null, "Cancel", null, Constants.TYPE_ATTACH);
			actionSheet.Clicked += SelectedAttachType;
			actionSheet.ShowInView(this.View);
		}

		partial void ActionDefailtIcon(UIButton sender)
		{
			sender.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

			string strIconName = "";
			switch (sender.Tag)
			{
				case Constants.TAG_DEFAILT_ICON1:
					strIconName = Constants.STR_DEFAILT_ICON6;
					break;
				case Constants.TAG_DEFAILT_ICON2:
					strIconName = Constants.STR_DEFAILT_ICON7;
					break;
				case Constants.TAG_DEFAILT_ICON3:
					strIconName = Constants.STR_DEFAILT_ICON8;
					break;
				case Constants.TAG_DEFAILT_ICON4:
					strIconName = Constants.STR_DEFAILT_ICON9;
					break;
				case Constants.TAG_DEFAILT_ICON5:
					strIconName = Constants.STR_DEFAILT_ICON10;
					break;
				default:
					break;
			}
			imgDropIcon.Image = UIImage.FromFile(strIconName);
			ItemModel.Icon = ByteDataFromImage(MaxResizeImage(imgDropIcon.Image));
		}

		partial void ActionCustomIcon(UIButton sender)
		{
			var actionSheet = new UIActionSheet(Constants.STR_CUSTOM_ICON_TITLE, null, "Cancel", null, Constants.TYPE_FROM_SOURCE);
			actionSheet.Clicked += SelectedCustomIcon;
			actionSheet.ShowInView(this.View);
		}

		partial void ActionCurrentLocation(UIButton sender)
		{
			sender.Selected = !sender.Selected;

			if (sender.Selected)
			{
				var lResult = LocationHelper.GetLocationResult();
				ItemModel.Location_Lat = lResult.Latitude;
				ItemModel.Location_Lnt = lResult.Longitude;

				lblLocationLat.Text = ItemModel.Location_Lat.ToString();
				lblLocationLog.Text = ItemModel.Location_Lnt.ToString();
			}
			else
			{
				ItemModel.Location_Lat = Constants.LOCATION_AUSTRALIA[0];
				ItemModel.Location_Lnt = Constants.LOCATION_AUSTRALIA[1];
				lblLocationLat.Text = ItemModel.Location_Lat.ToString();
				lblLocationLog.Text = ItemModel.Location_Lnt.ToString();
			}
		}

		partial void ActionCustomLocation(UIButton sender)
		{
			var pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_LOCATION) as DropLocationViewController;
			pvc.ItemModel = this.ItemModel;
			NavigationController.PushViewController(pvc, true);
		}

		partial void ActionVisibility(UIButton sender)
		{
			btnVisibleEvery.Selected = false;
			btnVisibleMe.Selected = false;
			btnVisibleSpecific.Selected = false;

			switch (sender.Tag)
			{
				case Constants.TAG_VISIBLE_EVERY:
					btnVisibleEvery.Selected = true;
					break;
				case Constants.TAG_VISIBLE_ME:
					btnVisibleMe.Selected = true;
					break;
				case Constants.TAG_VISIBLE_SPECIFIC:
					btnVisibleSpecific.Selected = true;
					break;
			}

			ItemModel.Visibility = (int)sender.Tag;
		}

		partial void ActionAcessiblity(UIButton sender)
		{
			//throw new NotImplementedException();
		}

		partial void ActionPassword(UISwitch sender)
		{
			txtPassword.Enabled = sender.On;
			if (!sender.Selected)
				txtPassword.Text = string.Empty;
		}


		partial void ActionEligiblity(UIButton sender)
		{
			//throw new NotImplementedException();
		}

		partial void ActionShare(UIButton sender)
		{
			//throw new NotImplementedException();
		}


		async partial void ActionDropItem(UIButton sender)
		{
			if (!ItemModel.IsValidDrop())
			{
				ShowMessageBox(null, Constants.STR_DROP_INVALID);
				return;
			}
			
			ShowLoadingView(Constants.STR_LOADING);

			var result = await ParseService.AddDropItem(ItemModel.parseItem);

			HideLoadingView();

			if (result == Constants.STR_STATUS_SUCCESS)
				ShowMessageBox(null, Constants.STR_DROP_SUCCESS_MSG);
			else
				ShowMessageBox(null, result);
			//rootVC.SetCurrentPage(2);
		}
		#endregion

		#region delegate
		void SelectedAttachType(object sender, UIButtonEventArgs e)
		{
			switch (e.ButtonIndex)
			{
				case Constants.INDEX_TEXT:
					ShowTextFieldBox(Constants.STR_ATTACH_TEXT_TITLE, "Cancel", new[] { "OK" }, SetDropText);
					break;

				case Constants.INDEX_FROM_LIBRARY:
					mMediaPicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
					//mMediaPicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary | UIImagePickerControllerSourceType.Camera);
					mMediaPicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
					mMediaPicker.FinishedPickingMedia -= PickupMediaIconFinishedHandler;
					mMediaPicker.FinishedPickingMedia -= PickupMediaAttachFinishedHandler;
					mMediaPicker.FinishedPickingMedia += PickupMediaAttachFinishedHandler;
					NavigationController.PresentModalViewController(mMediaPicker, true);
					break;

				case Constants.INDEX_FROM_CAMERA:
					mMediaPicker.SourceType = UIImagePickerControllerSourceType.Camera;
					mMediaPicker.MediaTypes = new string[] { UTType.Image, UTType.Movie };
					mMediaPicker.FinishedPickingMedia -= PickupMediaIconFinishedHandler;
					mMediaPicker.FinishedPickingMedia -= PickupMediaAttachFinishedHandler;
					mMediaPicker.FinishedPickingMedia += PickupMediaAttachFinishedHandler;
					NavigationController.PresentModalViewController(mMediaPicker, true);
					break;

				case Constants.INDEX_OTHER:
					ShowTextFieldBox(Constants.STR_ATTACH_OTHER_TITLE, "Cancel", new[] { "OK" }, SetOtherLink);
					break;
			}

		}
		void SelectedCustomIcon(object sender, UIButtonEventArgs e)
		{
			switch (e.ButtonIndex)
			{
				case 0:
					mMediaPicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
					mMediaPicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
					mMediaPicker.FinishedPickingMedia -= PickupMediaIconFinishedHandler;
					mMediaPicker.FinishedPickingMedia -= PickupMediaAttachFinishedHandler;
					mMediaPicker.FinishedPickingMedia += PickupMediaIconFinishedHandler;
					NavigationController.PresentModalViewController(mMediaPicker, true);
					break;

				case 1:
					mMediaPicker.SourceType = UIImagePickerControllerSourceType.Camera;
					mMediaPicker.MediaTypes = new string[] { UTType.Image };
					mMediaPicker.FinishedPickingMedia -= PickupMediaIconFinishedHandler;
					mMediaPicker.FinishedPickingMedia -= PickupMediaAttachFinishedHandler;
					mMediaPicker.FinishedPickingMedia += PickupMediaIconFinishedHandler;
					NavigationController.PresentModalViewController(mMediaPicker, true);
					break;
			}
		}


		#endregion

		#region functions
		void SetDropText(string text)
		{
			ItemModel.Text = text;
			btnDropTextSymbol.Selected = text != "" ? true : false;
		}

		void SetOtherLink(string text)
		{
			ItemModel.OtherLink = text;
			btnDropOtherSymbol.Selected = text != "" ? true : false;
		}
		#endregion

		#region handler
		void PickupMediaAttachFinishedHandler(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			try
			{
				var isImage = false;

				switch (e.Info[UIImagePickerController.MediaType].ToString())
				{
					case "public.image":
						Console.WriteLine("Image selected");
						isImage = true;
						break;

					case "public.video":
						Console.WriteLine("Video selected");
						break;
				}

				if (isImage)
				{
					var originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
					var fileBytes = ByteDataFromImage(originalImage);

					ItemModel.Image = fileBytes;
					btnDropImageSymbol.Selected = true;
				}
				else
				{
					NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
					var fileBytes = ByteDataFromVideoURL(mediaURL);

					ItemModel.Video = fileBytes;
					btnDropVideoSymbol.Selected = true;
				}
				mMediaPicker.DismissViewController(true, null);
			}
			catch (Exception ex)
			{
				mMediaPicker.DismissViewController(true, null);
				Console.WriteLine(ex.Message);
			}
		}

		void PickupMediaIconFinishedHandler(object sender, UIImagePickerMediaPickedEventArgs e)
		{
			try
			{
				var originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
				var resizedImage = MaxResizeImage(originalImage);
				imgDropIcon.Image = originalImage;
				ItemModel.Icon = ByteDataFromImage(resizedImage);

				mMediaPicker.DismissViewController(true, null);
			}
			catch (Exception ex)
			{
				mMediaPicker.DismissViewController(true, null);
				Console.WriteLine(ex.Message);
			}
		}



		void PickupMediaCanceledHandler(object sender, EventArgs e)
		{
			mMediaPicker.DismissViewController(true, null);
		}
		#endregion
	}

	[Preserve]
	static class PreserveEventsAndSettersHack
	{
		[Preserve]
		static void Hack()
		{
			var l = new UILabel();
			l.Text = l.Text + "";

			var tf = new UITextField();
			tf.Text = tf.Text + "";
			tf.EditingChanged += delegate { };
			tf.ValueChanged += delegate { };

			var tv = new UITextView();
			tv.Text = tv.Text + "";
			tv.Changed += delegate { };

			var vc = new UIViewController();
			vc.Title = vc.Title + "";
			vc.Editing = !vc.Editing;
		}
	}
}