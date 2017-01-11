using Foundation;
using System;
using UIKit;

namespace Drop.iOS
{
    public partial class DropDetailViewController : BaseViewController
    {
		public ParseItem parseItem;

		private ItemModel ItemModel { get; set; }

        public DropDetailViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_DETAIL)
		{
			ItemModel = new ItemModel();
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ItemModel.parseItem = parseItem;

			if (parseItem.ImageURL != null)
				imgImage.Image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(parseItem.ImageURL.ToString())));

			lblName.Text = parseItem.Name;
			lblText.Text = parseItem.Text;
		}

		#region actions
		partial void ActionPlayVideo(UIButton sender)
		{
			if (parseItem.VideoURL != null)
				UIApplication.SharedApplication.OpenUrl(new NSUrl(parseItem.Other.ToString()));
		}

		partial void ActionGoToLink(UIButton sender)
		{
			if (parseItem.Other != null && parseItem.Other != "")
				UIApplication.SharedApplication.OpenUrl(new NSUrl(parseItem.Other.ToString()));
		}
		#endregion
	}
}