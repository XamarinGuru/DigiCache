using Foundation;
using System;
using UIKit;

using Facebook.LoginKit;
using Facebook.CoreKit;

namespace Drop.iOS
{
	public partial class LoginViewController : BaseViewController
    {
		public LoginViewController (IntPtr handle) : base (handle, Constants.STR_iOS_VCNAME_LOGIN)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}

		partial void ActionFBLogin(UIButton sender)
		{
			var FBLoginManager = new Facebook.LoginKit.LoginManager();
			FBLoginManager.LogOut();
			FBLoginManager.LogInWithReadPermissions(Constants.FB_PERMISSIONS, this, HandleLoginManagerRequestTokenHandler);
		}

		partial void ActionAnonLogin(UIButton sender)
		{
			GoToHomeVC();
		}

		private void GoToHomeVC()
		{
			var pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_HOME);
			NavigationController.PushViewController(pvc, true);
		}

		#region FBLogin Callback
		void HandleLoginManagerRequestTokenHandler(LoginManagerLoginResult result, NSError error)
		{
			if (error != null)
			{
				//ShowMessageBox("Ups", error.Description, "Ok", null, null);
				ShowMessageBox(Constants.STR_LOGIN_FAIL_TITLE, Constants.STR_LOGIN_FAIL_MSG);
				return;
			}
			if (result.IsCancelled)
			{
				return;
			}
			ParseLogin(result.Token);
		}

		async void ParseLogin(AccessToken fbToken)
		{
			var userid = fbToken.UserID;
			var token = fbToken.TokenString;
			var expiration = NSDateToDateTime(fbToken.ExpirationDate);

			ShowLoadingView(Constants.STR_LOGIN_LOADING);
			var response = await ParseService.FacebookSignUp(userid, token, expiration);
			HideLoadingView();

			if (response == Constants.STR_STATUS_SUCCESS)
			{
				GoToHomeVC();
			}
			else {
				ShowMessageBox(Constants.STR_LOGIN_FAIL_TITLE, Constants.STR_LOGIN_FAIL_MSG);
			}
		}

		//private void FacebookLoggedIn(string userId)
		//{
		//	ShowLoadingView("Getting some user data...");

		//	var fields = "?fields=id,name,email,first_name,last_name,picture";
		//	var request = new GraphRequest("/" + userId + fields, null, Facebook.CoreKit.AccessToken.CurrentAccessToken.TokenString, null, "GET");
		//	var requestConnection = new GraphRequestConnection();
		//	requestConnection.AddRequest(request, (connection, result, error) =>
		//	{

		//		if (error != null)
		//		{
		//			HideLoadingView();
		//			new UIAlertView("Error...", error.Description, null, "Ok", null).Show();
		//			new UIAlertView("Login Failed", "The social network login failed for your account", null, "Ok", null).Show();
		//			return;
		//		}

		//		var userInfo = (NSDictionary)result;

		//		var UserFirstName = userInfo["first_name"].ToString();
		//		var UserLastName = userInfo["last_name"].ToString();
		//		var UserEmail = userInfo["email"].ToString();

		//		var tmp1 = (NSDictionary)userInfo["picture"];
		//		var tmp2 = (NSDictionary)tmp1["data"];
		//		var UserPhoto = tmp2["url"].ToString();
		//	});
		//	requestConnection.Start();
		//}
		#endregion
	}
}