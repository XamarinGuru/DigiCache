using Foundation;
using System;
using UIKit;
using GalaSoft.MvvmLight.Helpers;

namespace Drop.iOS
{
    public partial class DropItemViewController : BaseViewController
	{
		private ItemModel ItemModel { get; set; }

		private string mText;

		public DropItemViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_ITEM)
		{
			ItemModel = new ItemModel();
		}
        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var tap = new UITapGestureRecognizer(() => { View.EndEditing(true); });
			View.AddGestureRecognizer(tap);

			SetUISettings();
			SetInputBinding();
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

		private void SetInputBinding()
		{
			this.SetBinding(() => ItemModel.Name, () => txtName.Text, BindingMode.TwoWay);
			this.SetBinding(() => ItemModel.Description, () => txtDescription.Text, BindingMode.TwoWay);
			//this.SetBinding(() => ItemModel.Text, () => mText, BindingMode.TwoWay);

			//this.SetBinding(() => txtName.Text, () => ItemModel.Name, BindingMode.OneWay);
			//this.SetBinding(() => txtDescription.Text, () => ItemModel.Description, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.Text, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.Photo, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.Video, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.Other, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.Icon, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.Permission, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.Password, BindingMode.OneWay);
			//this.SetBinding(() => mText, () => ItemModel.ExpiryDate, BindingMode.OneWay);
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

		async partial void ActionDropItem(UIButton sender)
		{
			ShowLoadingView(Constants.STR_LOADING);
			var result = await ParseService.AddDropItem(ItemModel.parseItem);
			HideLoadingView();

			if (result == Constants.STR_STATUS_SUCCESS)
				ShowMessageBox(null, Constants.STR_DROP_SUCCESS_MSG);
			else
				ShowMessageBox(null, result);
			//rootVC.SetCurrentPage(2);
		}
	}
}