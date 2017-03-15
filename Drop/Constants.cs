using System;
namespace Drop
{
	public static class Constants
	{
		public const string APP_NAME = "Drop";
		public const string PARSE_APP_ID = "FsCghYDYIHRAorWAAWwNoDzxWV21nkXKziSuUV1w";
		public const string PARSE_NET_KEY = "G8G0knLW7r1LH4lcMqWKLReWFYvZaGSpRgJLcJF0";
		public const string PARSE_SERVER_URL = "https://parseapi.back4app.com";

		public const string FACEBOOK_APP_ID = "249022678854174";
		public const string FACEBOOK_DISPLAY_NAME = "Drop";

		public const string GOOGLE_MAP_API_KEY = "AIzaSyAiBwRUm_KZDv_sp3eI7F8hxkePqDTvY20";

		public static string[] FB_PERMISSIONS = new[] { "public_profile", "email" };

		//message
		public const string STR_STATUS_SUCCESS = "success";
		public const string STR_STATUS_FAIL = "fail";
		public const string STR_UNKNOWN_USER = "unknown user";

		public const string STR_LOADING = "Loading...";

		public const string STR_LOGIN_FAIL_TITLE = "Login Failed";
		public const string STR_LOGIN_FAIL_MSG = "The social network login failed for your account";
		public const string STR_LOGIN_LOADING = "Login...";

		public const string STR_DROPS_LOADING = "Retreving nearby items...";
		public const string STR_F_DROPS_LOADING = "Retreving favourite items...";

		public const string STR_DROP_INVALID = "You should specify at least Name & Icon & Location & Expiry date to drop your item.";
		public const string STR_DROP_SUCCESS_MSG = "Item successfully dropped.";

		public const string STR_ATTACH_TITLE = "Select Attach File Type...";
		public const string STR_CUSTOM_ICON_TITLE = "Customize your drop icon...";
		public const string STR_ATTACH_TEXT_TITLE = "Drop Text.";
		public const string STR_ATTACH_OTHER_TITLE = "Link / URL.";

		public const string STR_MODIFY_DENY_MSG = "You can't modify this drop.";

		public const string STR_VERIFY_PASSWORD_TITLE = "Require to access to this drop item.";
		public const string STR_INVALID_PASSWORD_TITLE = "Invalid Password.";

		public const string STR_INVALID_USERINFO = "Can't detect your info. You need to login with Facebook";

		public const string STR_FAVORITE_ADDED = "Successfully added to your \nFavourites list.";
		public const string STR_FAVORITE_DELETED = "Successfully removed from your \nFavourites list.";

		public const string STR_NO_EXPIRY = "No Expiry Date";

		//vc names
		public const string STR_iOS_VCNAME_LOGIN = "LoginViewController";
		public const string STR_iOS_VCNAME_HOME = "HomeViewController";
		public const string STR_iOS_VCNAME_ITEM = "DropItemViewController";
		public const string STR_iOS_VCNAME_NEARBY = "NearbyViewController";
		public const string STR_iOS_VCNAME_FAVORITE = "FavoriteViewController";
		public const string STR_iOS_VCNAME_DETAIL = "DropDetailViewController";
		public const string STR_iOS_VCNAME_LOCATION = "DropLocationViewController";

		public const string STR_DROID_VCNAME_LOGIN = "LoginViewController";
		public const string STR_DROID_VCNAME_HOME = "HomeViewController";
		public const string STR_DROID_VCNAME_ITEM = "DropItemViewController";
		public const string STR_DROID_VCNAME_NEARBY = "NearbyViewController";
		public const string STR_DROID_VCNAME_DETAIL = "DropDetailViewController";
		public const string STR_DROID_VCNAME_LOCATION = "DropLocationViewController";

		//tag
		public const int TAG_DROP_ITEM = 1;
		public const int TAG_DROP_NEARBY = 2;
		public const int TAG_DROP_SETTING = 3;

		public const int TAG_COLLEPS_NAME = 1;
		public const int TAG_COLLEPS_ICON = 2;
		public const int TAG_COLLEPS_LOCATION = 3;
		public const int TAG_COLLEPS_PERMISSION = 4;
		public const int TAG_COLLEPS_MODIFY = 5;
		public const int TAG_COLLEPS_PASSWORD = 6;
		public const int TAG_COLLEPS_EXPIRY = 7;

		public static int[] COLLEPS_HEIGHTS = new[] { 175, 130, 60, 150, 150, 60, 130 };

		//public const int TAG_D_COLLEPS_NAME = 0;
		//public const int TAG_D_COLLEPS_ICON = 1;
		//public const int TAG_D_COLLEPS_LOCATION = 2;
		//public const int TAG_D_COLLEPS_PERMISSION = 3;
		//public const int TAG_D_COLLEPS_PASSWORD = 4;
		//public const int TAG_D_COLLEPS_EXPIRY = 5;
		//public const int TAG_D_COLLEPS_SHARE = 6;

		public const int INDEX_TEXT = 0;
		public const int INDEX_FROM_LIBRARY = 1;
		public const int INDEX_FROM_CAMERA = 2;
		public const int INDEX_OTHER = 3;

		public const int INDEX_ANDROID_TEXT = 0;
		public const int INDEX_ANDROID_IMAGE_FROM_LIBRARY = 1;
		public const int INDEX_ANDROID_VIDEO_FROM_LIBRARY = 2;
		public const int INDEX_ANDROID_IMAGE_FROM_CAMERA = 3;
		public const int INDEX_ANDROID_VIDEO_FROM_CAMERA = 4;
		public const int INDEX_ANDROID_OTHER = 5;

		public const int TAG_VISIBLE_EVERY = 0;
		public const int TAG_VISIBLE_ME = 1;
		public const int TAG_VISIBLE_SPECIFIC = 2;


		//parse table names
		public const string STR_TABLE_DROP_ITEM = "DropItems";

		//parse field names
		public const string STR_FIELD_USERID = "UserID";
		public const string STR_FIELD_NAME = "Name";
		public const string STR_FIELD_DESCRIPTION = "Description";
		public const string STR_FIELD_TEXT = "Text";
		public const string STR_FIELD_IMAGE = "Image";
		public const string STR_FIELD_VIDEO = "Video";
		public const string STR_FIELD_OTHER_LINK = "Other_Link";
		public const string STR_FIELD_ICON = "Icon";
		public const string STR_FIELD_LOCATION_LNT = "Location_Lnt";
		public const string STR_FIELD_LOCATION_LAT = "Location_Lat";
		public const string STR_FIELD_VISIBILITY = "Visibility";
		public const string STR_FIELD_MODIFY = "Modify";
		public const string STR_FIELD_PASSWORD = "Password";
		public const string STR_FIELD_EXPIRY = "Expiry";
		public const string STR_FIELD_FAVORITE = "Favorite";

		//defailt icon name
		public const string STR_DEFAILT_ICON1 = "icon_drop1.png";
		public const string STR_DEFAILT_ICON2 = "icon_drop2.png";
		public const string STR_DEFAILT_ICON3 = "icon_drop3.png";
		public const string STR_DEFAILT_ICON4 = "icon_drop4.png";
		public const string STR_DEFAILT_ICON5 = "icon_drop5.png";

		public const string STR_DEFAILT_ICON6 = "icon_drop6.png";
		public const string STR_DEFAILT_ICON7 = "icon_drop7.png";
		public const string STR_DEFAILT_ICON8 = "icon_drop8.png";
		public const string STR_DEFAILT_ICON9 = "icon_drop9.png";
		public const string STR_DEFAILT_ICON10 = "icon_drop10.png";

		//
		public static string[] TYPE_ATTACH = new[] {    "Text",
														"Photo Library",
														"Camera",
														"Link / URL" };

		public static string[] TYPE_ATTACH_DROID = new[] {    "Text",
														"Photo Library",
														"Photo Library",
														"Camera",
														"Camera",
														"Link / URL" };

		public static string[] TYPE_FROM_SOURCE = new[] {   "Photo Library",
															"Camera" };

		public static double[] LOCATION_AUSTRALIA = new double[] { 35.2809f, 149.1300f };


		public const int VDROP_MAX_SIZE = 100;
		public const int MDROP_MAX_SIZE = 30;

		public const int MAP_ZOOM_LEVEL = 18;


		public enum VISIVILITY_TYPE
		{
			EVERYONE,
			ONLY_ME,
			SPECIAL_USER
		}

		#region in-app purchase
		public enum PURCHASE_TYPE
		{
			DROP,
			VIEW,
			EXPIRY
		}

		public const int PURCHASE_DISTANCE = 300;

		public static string[] PURCHASE_ID = new string[]{
			"DropDistantCache",
			"OpenDistantCache",
			"NoExpiryDrop"
		};
		public static string[] PURCHASE_TITLE = new string[]{
			"You're Too Far Away.",
			"You're Too Far Away.",
			"That Date Wont work!"
		};
		public static string[] PURCHASE_DESCRIPTION1 = new string[] {
			"You need to be within 15m \nto drop on this location.",
			"You need to be within 15m \nto view this cache.",
			"You can only leave a cache for \n12 months."
		};
		public static string[] PURCHASE_DESCRIPTION2 = new string[] {
			"Leave a cache anywhere in the world",
			"Look inside a distance cache",
			"Make your DigiCache Last forever!"
		};
		public static string[] PURCHASE_BUTTON = new string[] {
			"Buy A Distance Cache Drop",
			"Buy One Sneak Peak",
			"Buy A Forever Cache"
		};
		#endregion
	}
}
