
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Org.Json;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]
[assembly: MetaData("com.facebook.sdk.ApplicationName", Value = "@string/app_name")]

namespace Drop.Droid
{
	[Activity(Label = "LoginActivity")]
	public class LoginActivity : BaseActivity
	{
		ICallbackManager callbackManager;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.LoginActivity);

			InitUISettings();
			InitFBSettings();
		}

		void InitUISettings()
		{
			FindViewById<LinearLayout>(Resource.Id.ActionLoginUsingFacebook).Click += ActionLoginUsingFacebook;
			FindViewById<LinearLayout>(Resource.Id.ActionLoginAnonymously).Click += ActionLoginAnonymously;
		}

		void InitFBSettings()
		{
			FacebookSdk.SdkInitialize(this.ApplicationContext);
			callbackManager = CallbackManagerFactory.Create();
			var loginCallback = new FacebookCallback<LoginResult>
			{
				HandleSuccess = loginResult =>
				{
					var accessToken = AccessToken.CurrentAccessToken;
					FBGraphRequeest(accessToken, accessToken.UserId);
				},
				HandleCancel = () => { },
				HandleError = loginError => { }
			};
			LoginManager.Instance.RegisterCallback(callbackManager, loginCallback);
		}

		void FBGraphRequeest(AccessToken token, string userId)
		{
			ShowLoadingView(Constants.STR_LOGIN_LOADING);

			var requestCallback = new FacebookRequestCallback<JSONObject>
			{
				HandleSuccess = requestResult =>
				{
					var user = new User();
					var email = requestResult.OptString("email");

					user.Firstname = requestResult.OptString("first_name");
					user.Lastname = requestResult.OptString("last_name");
					user.Email = requestResult.OptString("email");
					user.Password = requestResult.OptString("email");
					user.Type = Constants.TAG_VISIBLE_SPECIFIC;

					ParseLogin(user);
				},
				HandleCancel = () => { },
				HandleError = loginError => { }
			};
			Bundle requestParams = new Bundle();
			requestParams.PutString("fields", "id,name,email,first_name,last_name,picture");
			GraphRequest graphRequest = GraphRequest.NewMeRequest(token, requestCallback);
			graphRequest.Parameters = requestParams;
			graphRequest.ExecuteAsync();
		}

		private async void ParseLogin(User user)
		{
			var response = await ParseService.SignUp(user);

			HideLoadingView();
			if (response == Constants.STR_STATUS_SUCCESS)
			{
				GoToHomeVC();
			}
			else {
				ShowMessageBox(Constants.STR_LOGIN_FAIL_TITLE, Constants.STR_LOGIN_FAIL_MSG);
			}
		}

		private void GoToHomeVC()
		{
			var mainActivity = new Intent(this, typeof(HomeActivity));
			StartActivity(mainActivity);
		}



		#region actions
		void ActionLoginUsingFacebook(object sender, EventArgs e)
		{
			LoginManager.Instance.LogInWithReadPermissions(this, Constants.FB_PERMISSIONS);
		}

		void ActionLoginAnonymously(object sender, EventArgs e)
		{
			ParseService.Logout();
			GoToHomeVC();
		}
		#endregion




		#region FB login callback
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
		}

		class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
		{
			public Action HandleCancel { get; set; }
			public Action<FacebookException> HandleError { get; set; }
			public Action<TResult> HandleSuccess { get; set; }

			public void OnCancel()
			{
				var c = HandleCancel;
				if (c != null)
					c();
			}

			public void OnError(FacebookException error)
			{
				var c = HandleError;
				if (c != null)
					c(error);
			}

			public void OnSuccess(Java.Lang.Object result)
			{
				var c = HandleSuccess;
				if (c != null)
					c(result.JavaCast<TResult>());
			}
		}

		class FacebookRequestCallback<TResult> : Java.Lang.Object, GraphRequest.IGraphJSONObjectCallback where TResult : Java.Lang.Object
		{
			public Action HandleCancel { get; set; }
			public Action<FacebookException> HandleError { get; set; }
			public Action<TResult> HandleSuccess { get; set; }

			public void OnCancel()
			{
				var c = HandleCancel;
				if (c != null)
					c();
			}

			public void OnError(FacebookException error)
			{
				var c = HandleError;
				if (c != null)
					c(error);
			}

			public void OnCompleted(JSONObject result, GraphResponse response)
			{
				var c = HandleSuccess;
				if (c != null)
					c(result.JavaCast<TResult>());
			}
		}
		#endregion
	}
}
