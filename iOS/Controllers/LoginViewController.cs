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
			ParseService.Logout();
			GoToHomeVC();
		}

		private void GoToHomeVC()
		{
			//AppSettings.UserType = Constants.TAG_VISIBLE_EVERY;
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
			LoginProcess(result.Token);
		}

		private void LoginProcess(AccessToken fbToken)
		{
			//AppSettings.UserFBID = fbToken.UserID;

			ShowLoadingView(Constants.STR_LOGIN_LOADING);

			FacebookLoggedIn(fbToken.UserID);
		}

		private void FacebookLoggedIn(string userId)
		{
			var fields = "?fields=id,name,email,first_name,last_name,picture";
			var request = new GraphRequest("/" + userId + fields, null, Facebook.CoreKit.AccessToken.CurrentAccessToken.TokenString, null, "GET");
			var requestConnection = new GraphRequestConnection();
			requestConnection.AddRequest(request, (connection, result, error) =>
			{
				if (error != null)
				{
					HideLoadingView();
					//new UIAlertView("Error...", error.Description, null, "Ok", null).Show();
					ShowMessageBox(Constants.STR_LOGIN_FAIL_TITLE, Constants.STR_LOGIN_FAIL_MSG);
					return;
				}

				var userInfo = (NSDictionary)result;

				var user = new User();

				user.Firstname = userInfo["first_name"].ToString();
				user.Lastname = userInfo["last_name"].ToString();
				user.Email = userInfo["email"].ToString();
				user.Password = userInfo["email"].ToString();
				user.Type = Constants.TAG_VISIBLE_SPECIFIC;

				var tmp1 = (NSDictionary)userInfo["picture"];
				var tmp2 = (NSDictionary)tmp1["data"];

				ParseLogin(user);
			});
			requestConnection.Start();
		}

		private async void ParseLogin(User user)
		{
			var response = await ParseService.SignUp(user);

			HideLoadingView();
			if (response == Constants.STR_STATUS_SUCCESS)
			{
				//AppSettings.UserType = Constants.TAG_VISIBLE_SPECIFIC;
				GoToHomeVC();
			}
			else {
				ShowMessageBox(Constants.STR_LOGIN_FAIL_TITLE, Constants.STR_LOGIN_FAIL_MSG);
			}
		}
		#endregion
	}
}