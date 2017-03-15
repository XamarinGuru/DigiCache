using Foundation;
using System;
using UIKit;
using Parse;

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

			InitUISettings();
		}

		void InitUISettings()
		{
			if (parseItem.ImageURL != null)
				imgImage.Image = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(parseItem.ImageURL.ToString())));

			lblName.Text = parseItem.Name;
			lblText.Text = parseItem.Text;

			if (ParseUser.CurrentUser != null)
			{
				var favoriteList = ItemModel.Favorite;

				foreach (var favoriteID in favoriteList)
				{
					if (favoriteID.Equals(ParseUser.CurrentUser.ObjectId))
						symbolFavorite.Selected = !symbolFavorite.Selected;
				}
			}
		}

		//#region actions
		//partial void ActionPlayVideo(UIButton sender)
		//{
		//	if (parseItem.VideoURL != null)
		//	{
		//		var strURL = parseItem.VideoURL.ToString();
		//		var url = new NSUrl(strURL);
		//		UIApplication.SharedApplication.OpenUrl(url);
		//	}
		//}

		//partial void ActionGoToLink(UIButton sender)
		//{
		//	if (parseItem.OtherLink != null && parseItem.OtherLink != "")
		//		UIApplication.SharedApplication.OpenUrl(new NSUrl(parseItem.OtherLink));
		//}
		//#endregion

		partial void ActionSaveFile(UIButton sender)
		{
			//throw new NotImplementedException();
		}

		partial void ActionModifyItems(UIButton sender)
		{
			DropItemViewController pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_ITEM) as DropItemViewController;
			pvc.ItemModel.parseItem = parseItem;
			NavigationController.PushViewController(pvc, true);
		}

		partial void ActionShareDropLocation(UIButton sender)
		{
			var dropIcon = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(parseItem.ImageURL.ToString())));
			var dropContent = string.Format("Drop Name:\n" + parseItem.Name + "\n\n" +
											"Drop Description:\n" + parseItem.Description + "\n\n" +
											"Drop Location:\n http://maps.apple.com/?ll={0},{1}", parseItem.Location_Lat, parseItem.Location_Lnt);
			NSObject[] activityItems = { dropIcon, NSObject.FromObject(dropContent) };
			UIActivityViewController activityViewController = new UIActivityViewController(activityItems, null);
			activityViewController.ExcludedActivityTypes = new NSString[] { };
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				activityViewController.PopoverPresentationController.SourceView = View;
				activityViewController.PopoverPresentationController.SourceRect = new CoreGraphics.CGRect((View.Bounds.Width / 2), (View.Bounds.Height / 4), 0, 0);
			}
			this.PresentViewController(activityViewController, true, null);
		}

		async partial void ActionFavorite(UIButton sender)
		{
			if (ParseUser.CurrentUser == null)
			{
				ShowMessageBox(Constants.STR_INVALID_USERINFO, null);
				return;
			}
			symbolFavorite.Selected = !symbolFavorite.Selected;

			var favoriteList = ItemModel.Favorite;

			for (int i = 0; i < favoriteList.Count; i++)
			{
				if (favoriteList[i].Equals(ParseUser.CurrentUser.ObjectId))
					favoriteList.RemoveAt(i);
			}

			if (symbolFavorite.Selected)
			{
				favoriteList.Add(ParseUser.CurrentUser.ObjectId);
			}

			ItemModel.Favorite = favoriteList;

			ShowLoadingView(Constants.STR_LOADING);

			var result = await ParseService.UpdateFavorite(ItemModel.parseItem);

			HideLoadingView();

			FavoritePopUp fPopup = FavoritePopUp.Create(symbolFavorite.Selected);
			fPopup.PopUp(true);
		}
	}
}