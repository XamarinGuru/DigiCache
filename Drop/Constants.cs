using System;
namespace Drop
{
	public static class Constants
	{
		public const string PARSE_APP_ID = "FsCghYDYIHRAorWAAWwNoDzxWV21nkXKziSuUV1w";
		public const string PARSE_NET_KEY = "G8G0knLW7r1LH4lcMqWKLReWFYvZaGSpRgJLcJF0";
		public const string PARSE_SERVER_URL = "https://parseapi.back4app.com";

		public const string FACEBOOK_APP_ID = "249022678854174";
		public const string FACEBOOK_DISPLAY_NAME = "Drop";

		public const string GOOGLE_MAP_API_KEY = "AIzaSyAiBwRUm_KZDv_sp3eI7F8hxkePqDTvY20";
		public const string GOOGLE_AUTO_FILL_URL = "https://maps.googleapis.com/maps/api/place/autocomplete/json";

		public static string[] FB_PERMISSIONS = new[] { "public_profile", "email" };

		//message
		public const string STR_STATUS_SUCCESS = "success";
		public const string STR_STATUS_FAIL = "fail";
		public const string STR_UNKNOWN_USER = "unknown user";

		public const string STR_LOADING = "Loading...";

		public const string STR_LOGIN_FAIL_TITLE = "Login Failed";
		public const string STR_LOGIN_FAIL_MSG = "The social network login failed for your account";
		public const string STR_LOGIN_LOADING = "Login...";

		public const string STR_DROPS_LOADING = "Retreving Drop Items...";

		public const string STR_DROP_INVALID = "You should specify at least Name & Icon & Location to drop your item.";
		public const string STR_DROP_SUCCESS_MSG = "Dropped your item successfully.";

		public const string STR_ATTACH_TITLE = "Select Attach File Type...";
		public const string STR_CUSTOM_ICON_TITLE = "Customize your drop icon...";
		public const string STR_ATTACH_TEXT_TITLE = "Describe to your drop.";
		public const string STR_ATTACH_OTHER_TITLE = "Other link.";

		public const string STR_VERIFY_PASSWORD_TITLE = "Require to access to this drop item.";
		public const string STR_INVALID_PASSWORD_TITLE = "Invalid Password.";

		//vc names
		public const string STR_iOS_VCNAME_LOGIN = "LoginViewController";
		public const string STR_iOS_VCNAME_HOME = "HomeViewController";
		public const string STR_iOS_VCNAME_ITEM = "DropItemViewController";
		public const string STR_iOS_VCNAME_NEARBY = "NearbyViewController";
		public const string STR_iOS_VCNAME_DETAIL = "DropDetailViewController";
		public const string STR_iOS_VCNAME_LOCATION = "DropLocationViewController";
		//tag
		public const int TAG_DROP_ITEM = 1;
		public const int TAG_DROP_NEARBY = 2;
		public const int TAG_DROP_SETTING = 3;

		public const int TAG_COLLEPS_NAME = 185;
		public const int TAG_COLLEPS_ICON = 125;
		public const int TAG_COLLEPS_LOCATION = 126;
		public const int TAG_COLLEPS_PERMISSION = 255;
		public const int TAG_COLLEPS_PASSWORD = 127;
		public const int TAG_COLLEPS_EXPIRY = 190;

		public const int TAG_DEFAILT_ICON1 = 11;
		public const int TAG_DEFAILT_ICON2 = 12;
		public const int TAG_DEFAILT_ICON3 = 13;
		public const int TAG_DEFAILT_ICON4 = 14;
		public const int TAG_DEFAILT_ICON5 = 15;

		public const int INDEX_TEXT = 0;
		public const int INDEX_FROM_LIBRARY = 1;
		public const int INDEX_FROM_CAMERA = 2;
		public const int INDEX_OTHER = 3;

		public const int TAG_VISIBLE_EVERY = 0;
		public const int TAG_VISIBLE_ME = 1;
		public const int TAG_VISIBLE_SPECIFIC = 2;


		//table names
		public const string STR_TABLE_DROP_ITEM = "DropItems";

		public const string STR_FIELD_USERID = "UserID";
		public const string STR_FIELD_NAME = "Name";
		public const string STR_FIELD_DESCRIPTION = "Description";
		public const string STR_FIELD_TEXT = "Text";
		public const string STR_FIELD_IMAGE = "Image";
		public const string STR_FIELD_VIDEO = "Video";
		public const string STR_FIELD_OTHER = "Other";
		public const string STR_FIELD_ICON = "Icon";
		public const string STR_FIELD_LOCATION_LNT = "Location_Lnt";
		public const string STR_FIELD_LOCATION_LAT = "Location_Lat";
		public const string STR_FIELD_VISIBILITY = "Visibility";
		public const string STR_FIELD_PASSWORD = "Password";
		public const string STR_FIELD_EXPIRY = "Expiry";

		//defailt icon name
		public const string STR_DEFAILT_ICON1 = "icon_drop1.png";
		public const string STR_DEFAILT_ICON2 = "icon_drop2.png";
		public const string STR_DEFAILT_ICON3 = "icon_drop3.png";
		public const string STR_DEFAILT_ICON4 = "icon_drop4.png";
		public const string STR_DEFAILT_ICON5 = "icon_drop5.png";

		//
		public static string[] TYPE_ATTACH = new[] { 	"Text", 
														"Select Image & Video from Photo Library", 
														"Take Image & Video from Camera",
														"Other..." };
		public static string[] TYPE_FROM_SOURCE = new[] {	"Select Icon from Photo Library",
															"Take Icon from Camera" };

		public static double[] LOCATION_AUSTRALIA = new double[] { 35.2809f, 149.1300f };
		
	}
}
