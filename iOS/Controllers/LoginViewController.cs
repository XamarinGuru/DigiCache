using Foundation;
using System;
using UIKit;

using Facebook.LoginKit;
using Facebook.CoreKit;

namespace Drop.iOS
{
    public partial class LoginViewController : UIViewController
    {
		

		public LoginViewController (IntPtr handle) : base (handle)
        {
        }

		partial void ActionFBLogin(UIButton sender)
		{
			var FBLoginManager = new Facebook.LoginKit.LoginManager();
			FBLoginManager.LogOut();
			FBLoginManager.LogInWithReadPermissions(Constants.FB_PERMISSIONS, this, HandleLoginManagerRequestTokenHandler);
		}

		partial void ActionAnonLogin(UIButton sender)
		{
			UIStoryboard sb = UIStoryboard.FromName("Main", null);
			UIViewController pvc = sb.InstantiateViewController("HomeViewController");
			NavigationController.PushViewController(pvc, true);
		}

		#region FBLogin Callback
		void HandleLoginManagerRequestTokenHandler(LoginManagerLoginResult result, NSError error)
		{
			if (error != null)
			{
				//ShowMessageBox("Ups", error.Description, "Ok", null, null);
				//ShowMessageBox("Login Failed", "The social network login failed for your account", "Ok", null, null);
				return;
			}
			if (result.IsCancelled)
			{
				return;
			}
			var userid = result.Token.UserID;
			FacebookLoggedIn(result.Token.UserID);
		}
		private void FacebookLoggedIn(string userId)
		{
			//ShowLoadingView("Getting some user data...");

			var fields = "?fields=id,name,email,first_name,last_name,picture";
			var request = new GraphRequest("/" + userId + fields, null, Facebook.CoreKit.AccessToken.CurrentAccessToken.TokenString, null, "GET");
			var requestConnection = new GraphRequestConnection();
			requestConnection.AddRequest(request, (connection, result, error) =>
			{

				if (error != null)
				{
					//HideLoadingView();
					//new UIAlertView("Error...", error.Description, null, "Ok", null).Show();
					new UIAlertView("Login Failed", "The social network login failed for your account", null, "Ok", null).Show();
					return;
				}

				var userInfo = (NSDictionary)result;

				//AppSettings.LoginType = (int)LoginType.Facebook;
				//AppSettings.UserType = "";
				//AppSettings.UserFirstName = userInfo["first_name"].ToString();
				//AppSettings.UserLastName = userInfo["last_name"].ToString();
				//AppSettings.UserEmail = userInfo["email"].ToString();

				var tmp1 = (NSDictionary)userInfo["picture"];
				var tmp2 = (NSDictionary)tmp1["data"];
				//AppSettings.UserPhoto = tmp2["url"].ToString();

				//AppSettings.UserToken = GetMd5Hash(md5Hash, userInfo["email"].ToString());

				//////we got all the data we need at this point, FB login successful

				//bool registerSuccessful = false;

				//Task runSync = Task.Factory.StartNew(async () =>
				//{
				//	registerSuccessful = await RegisterUser();
				//}).Unwrap();
				//runSync.Wait();

				//if (!registerSuccessful)
				//{
				//	GoToCreateAccountScreen();
				//	return;
				//}

				//ShowHomeScreen();
			});
			requestConnection.Start();
		}
		#endregion
	}
}