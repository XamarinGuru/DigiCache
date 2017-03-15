using Foundation;
using System;
using UIKit;
using SDWebImage;

namespace Drop.iOS
{
    public partial class FavoriteCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("FavoriteCell");
		public static readonly UINib Nib;

		ParseItem mDrop = new ParseItem();
		Action<ParseItem> removeCallback;
		Action<ParseItem> mapCallback;

		static FavoriteCell()
		{
			Nib = UINib.FromName("FavoriteCell", NSBundle.MainBundle);
		}

		public FavoriteCell(IntPtr handle) : base(handle)
		{
		}

		public void SetCell(ParseItem drop, Action<ParseItem> removeCallback, Action<ParseItem> mapCallback)
		{
			mDrop = drop;
			this.removeCallback = removeCallback;
			this.mapCallback = mapCallback;

			if (drop.IconURL != null)
				imgIcon.SetImage(
					url: new NSUrl(drop.IconURL.ToString()),
					placeholder: UIImage.FromBundle("icon_vendor.jpg")
				);

			lblName.Text = drop.Name;
			lblLat.Text = "LAT: " + drop.Location_Lat.ToString("F6");
			lblLong.Text = "LONG: " + drop.Location_Lnt.ToString("F6");
		}

		partial void ActionMap(UIButton sender)
		{
			mapCallback(mDrop);
		}

		partial void OnClickX(UIButton sender)
		{
			removeCallback(mDrop);
		}
	}
}