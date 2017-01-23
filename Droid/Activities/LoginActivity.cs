
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Drop;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

[assembly: MetaData("com.facebook.sdk.ApplicationId", Value = Constants.FACEBOOK_APP_ID)]
[assembly: MetaData("com.facebook.sdk.ApplicationName", Value = Constants.APP_NAME)]

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

			InitFBSettings();

			InitUISettings();
		}

		void InitFBSettings()
		{
			callbackManager = CallbackManagerFactory.Create();
			var loginCallback = new FacebookCallback<LoginResult>
		}

		void InitUISettings()
		{
			FindViewById<LinearLayout>(Resource.Id.ActionLoginUsingFacebook).Click += ActionLoginUsingFacebook;
			FindViewById<LinearLayout>(Resource.Id.ActionLoginAnonymously).Click += ActionLoginAnonymously;
		}

		void ActionLoginUsingFacebook(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void ActionLoginAnonymously(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
	}
}
