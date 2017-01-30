
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;

namespace Drop.Droid
{
	[Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]

	public class SplashActivity : BaseActivity
	{
		public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
		{
			base.OnCreate(savedInstanceState, persistentState);
		}

		protected override void OnResume()
		{
			base.OnResume();

			Task StartupWork = new Task(() =>
			{
				Task.Delay(500);
			});

			StartupWork.ContinueWith(t =>
			{
				StartDrop();
			}, TaskScheduler.FromCurrentSynchronizationContext());

			StartupWork.Start();
			//StartDrop();
		}

		public void StartDrop()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;
			bool isOnline = (activeConnection != null) && activeConnection.IsConnected;

			if (!isOnline)
			{
				ShowMessageBox("No internet connection", "Oops!No internet connection... Pls try again later", true);
				return;
			}

			StartActivity(new Intent(this, typeof(LoginActivity)));
		}
	}
}
