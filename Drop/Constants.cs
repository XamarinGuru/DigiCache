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

		public static string[] FB_PERMISSIONS = new[] { "public_profile", "email" };

		//message
		public const string STR_STATUS_SUCCESS = "success";
		public const string STR_STATUS_FAIL = "fail";

		public const string STR_LOADING = "Loading...";

		public const string STR_LOGIN_FAIL_TITLE = "Login Failed";
		public const string STR_LOGIN_FAIL_MSG = "The social network login failed for your account";
		public const string STR_LOGIN_LOADING = "Login...";


		public const string STR_DROP_SUCCESS_MSG = "Dropped your item successfully.";

		//vc names
		public const string STR_iOS_VCNAME_LOGIN = "LoginViewController";
		public const string STR_iOS_VCNAME_HOME = "HomeViewController";
		public const string STR_iOS_VCNAME_ITEM = "DropItemViewController";

		//tag
		public const int TAG_DROP_ITEM = 1;
		public const int TAG_DROP_NEARBY = 2;
		public const int TAG_DROP_SETTING = 3;

		public const int TAG_COLLEPS_NAME = 185;
		public const int TAG_COLLEPS_ICON = 125;
		public const int TAG_COLLEPS_PERMISSION = 255;
		public const int TAG_COLLEPS_PASSWORD = 126;
		public const int TAG_COLLEPS_EXPIRY = 190;

		//table names
		public const string STR_TABLE_DROP_ITEM = "DropItems";

		public const string STR_FIELD_NAME = "name";
		public const string STR_FIELD_DESCRIPTION = "description";
		public const string STR_FIELD_TEXT = "text";
		public const string STR_FIELD_EXPIRY = "expiry";
	}
}
