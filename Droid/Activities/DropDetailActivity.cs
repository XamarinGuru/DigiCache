
using System;
using System.IO;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Parse;

namespace Drop.Droid
{
	[Activity(Label = "DropDetailActivity")]
	public class DropDetailActivity : BaseActivity
	{
		ItemModel ItemModel = new ItemModel();

		CheckBox _symbolFavorite;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.DropDetailLayout);

			InitUISettings();
		}

		void InitUISettings()
		{
			var parseItem = Global.selectedDrop;
			ItemModel.parseItem = parseItem;

			var imgImage = FindViewById<ImageView>(Resource.Id.imgImage);
			if (parseItem.ImageURL != null)
			{
				var imageBitmap = GetImageBitmapFromUrl(parseItem.ImageURL.ToString());
				imgImage.SetImageBitmap(imageBitmap);
			}

			FindViewById<TextView>(Resource.Id.lblName).Text = parseItem.Name;
			FindViewById<TextView>(Resource.Id.lblText).Text = parseItem.Text;

			_symbolFavorite = FindViewById<CheckBox>(Resource.Id.symbolFavorite);
			if (ParseUser.CurrentUser != null)
			{
				var favoriteList = ItemModel.Favorite;

				foreach (var favoriteID in favoriteList)
				{
					if (favoriteID.Equals(ParseUser.CurrentUser.ObjectId))
						_symbolFavorite.Checked = !_symbolFavorite.Checked;
				}
			}

			FindViewById(Resource.Id.ActionSaveFile).Click += ActionSaveFile;
			FindViewById(Resource.Id.ActionModifyItems).Click += ActionModifyItems;
			FindViewById(Resource.Id.ActionShareDropLocation).Click += ActionShareDropLocation;
			FindViewById(Resource.Id.ActionFavorite).Click += ActionFavorite;

			FindViewById(Resource.Id.ActionBack).Click += delegate
			{
				base.OnBackPressed();
				OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
			};
		}

		void ActionSaveFile(object sender, EventArgs e)
		{
			//throw new NotImplementedException();
		}

		void ActionModifyItems(object sender, EventArgs e)
		{
			if (!ItemModel.parseItem.IsModifyByUser())
			{
				ShowMessageBox(null, Constants.STR_MODIFY_DENY_MSG);
				return;
			}
			//DropItemViewController pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_ITEM) as DropItemViewController;
			//pvc.ItemModel.parseItem = parseItem;
			//NavigationController.PushViewController(pvc, true);
		}

		void ActionShareDropLocation(object sender, EventArgs e)
		{
			Bitmap bitmap = BitmapFactory.DecodeByteArray(ItemModel.Icon.fileData, 0, ItemModel.Icon.fileData.Length);

			var tempFilename = "test.png";
			var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
			var filePath = System.IO.Path.Combine(sdCardPath, tempFilename);
			using (var os = new FileStream(filePath, FileMode.Create))
			{
				bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
			}
			bitmap.Dispose();

			var imageUri = Android.Net.Uri.Parse($"file://{sdCardPath}/{tempFilename}");
			var sharingIntent = new Intent();
			sharingIntent.SetAction(Intent.ActionSend);
			sharingIntent.SetType("image/*");

			var dropContent = string.Format("Drop Name:\n" + ItemModel.Name + "\n\n" +
											"Drop Description:\n" + ItemModel.Description + "\n\n" +
											"Drop Location:\n http://maps.google.com/?ll={0},{1}", ItemModel.Location_Lat, ItemModel.Location_Lnt);

			sharingIntent.PutExtra(Intent.ExtraText, dropContent);
			sharingIntent.PutExtra(Intent.ExtraStream, imageUri);
			sharingIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
			StartActivity(Intent.CreateChooser(sharingIntent, "Share your drop."));
		}

		async void ActionFavorite(object sender, EventArgs e)
		{
			if (ParseUser.CurrentUser == null)
			{
				ShowMessageBox(Constants.STR_INVALID_USERINFO, null);
				return;
			}
			_symbolFavorite.Checked = !_symbolFavorite.Checked;

			var favoriteList = ItemModel.Favorite;

			for (int i = 0; i < favoriteList.Count; i++)
			{
				if (favoriteList[i].Equals(ParseUser.CurrentUser.ObjectId))
					favoriteList.RemoveAt(i);
			}

			if (_symbolFavorite.Checked)
			{
				favoriteList.Add(ParseUser.CurrentUser.ObjectId);
			}

			ItemModel.Favorite = favoriteList;

			ShowLoadingView(Constants.STR_LOADING);

			var result = await ParseService.UpdateFavorite(ItemModel.parseItem);

			HideLoadingView();

			var msg = _symbolFavorite.Checked ? Constants.STR_FAVORITE_ADDED : Constants.STR_FAVORITE_DELETED;
			ShowMessageBox(null, msg);

			//FavoritePopUp fPopup = FavoritePopUp.Create(symbolFavorite.Selected);
			//fPopup.PopUp(true);
		}
		//void ActionPlayVideo(object sender, EventArgs e)
		//{
		//	if (parseItem.VideoURL != null)
		//	{
		//		var uri = Android.Net.Uri.Parse(parseItem.VideoURL.ToString());
		//		var intent = new Intent(Intent.ActionView, uri);
		//		StartActivity(intent);
		//	}
		//	//if (parseItem.VideoURL != null)
		//	//{
		//	//	var nextActivity = new Intent(this, typeof(PlayVideoActivity));
		//	//	StartActivity(nextActivity);
		//	//	OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
		//	//}
		//}
		//void ActionGotoLink(object sender, EventArgs e)
		//{
		//	if (parseItem.OtherLink != null && parseItem.OtherLink != "")
		//	{
		//		var uri = Android.Net.Uri.Parse(parseItem.OtherLink);
		//		var intent = new Intent(Intent.ActionView, uri);
		//		StartActivity(intent);
		//	}
		//}
	}
}
