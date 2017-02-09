
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Net;
using UniversalImageLoader.Core;

namespace Drop.Droid
{
	[Activity(Label = "HomeActivity")]
	public class HomeActivity : BaseActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
	{
		LayoutInflater controlInflater = null;

		const int Location_Request_Code = 0;

		LocationManager _locationManager;

		ImageView aniBgView;
		LinearLayout aniContentView;

		int nInitialAniviewHeight;

		private IList<ParseItem> mDrops;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			var config = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
			ImageLoader.Instance.Init(config);

			SetUIVariablesAndActions();
		}

		protected override void OnStart()
		{
			base.OnResume();

			string[] PermissionsLocation =
			{
				Manifest.Permission.AccessCoarseLocation,
				Manifest.Permission.AccessFineLocation
			};
			//Explain to the user why we need to read the contacts
			ActivityCompat.RequestPermissions(this, PermissionsLocation, Location_Request_Code);
		}

		protected override void OnStop()
		{
			base.OnStop();

			_locationManager.RemoveUpdates(this);
		}

		void GetDrops()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.STR_DROPS_LOADING);

				mDrops = ParseService.GetDropItems();

				HideLoadingView();

				if (mDrops.Count == 0)
					_locationManager.RemoveUpdates(this);
				else
					RunOnUiThread(() =>
					{
						_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
						Location currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
						OnLocationChanged(currentLocation);
					});
			});
		}

		private void SetUIVariablesAndActions()
		{
			controlInflater = LayoutInflater.From(BaseContext);
			var viewControl = controlInflater.Inflate(Resource.Layout.HomeActivity, null);
			var layoutParamsControl = new ActionBar.LayoutParams(ActionBar.LayoutParams.MatchParent, ActionBar.LayoutParams.MatchParent);
			this.AddContentView(viewControl, layoutParamsControl);

			#region UI Variables
			aniBgView = FindViewById<ImageView>(Resource.Id.aniBgView);
			aniContentView = FindViewById<LinearLayout>(Resource.Id.aniContentView);
			#endregion

			nInitialAniviewHeight = aniBgView.Height;

			#region Actions
			FindViewById<ImageView>(Resource.Id.ActionHome).Click += ActionHome;
			FindViewById<LinearLayout>(Resource.Id.ActionDropItem).Click += ActionDrop;
			FindViewById<LinearLayout>(Resource.Id.ActionDropNearby).Click += ActionDrop;
			FindViewById<LinearLayout>(Resource.Id.ActionDropSetting).Click += ActionDrop;
			#endregion
		}

		void ActionHome(object sender, EventArgs e)
		{
			HomeButtonAnimation(aniBgView);
			HomeButtonAnimation(aniContentView);
		}

		void ActionDrop(object sender, EventArgs e)
		{
			Intent dropAC = new Intent();
			switch (int.Parse(((LinearLayout)sender).Tag.ToString()))
			{
				case Constants.TAG_DROP_ITEM:
					dropAC = new Intent(this, typeof(DropItemActivity));
					break;
				case Constants.TAG_DROP_NEARBY:
					dropAC = new Intent(this, typeof(NearbyActivity));
					break;
				case Constants.TAG_DROP_SETTING:
					dropAC = new Intent(this, typeof(NearbyActivity));
					break;
				default:
					break;
			}

			StartActivity(dropAC);
		}

		void HomeButtonAnimation(View aniView)
		{
			if (aniView.Visibility.Equals(ViewStates.Gone))
			{
				aniView.Visibility = ViewStates.Visible;

				ValueAnimator mAnimator = slideAnimator(0, nInitialAniviewHeight, aniView);
				mAnimator.Start();
			}
			else {
				nInitialAniviewHeight = aniView.Height;

				ValueAnimator mAnimator = slideAnimator(nInitialAniviewHeight, 0, aniView);
				mAnimator.Start();
				mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
				{
					aniView.Visibility = ViewStates.Gone;
				};
			}
		}

		private ValueAnimator slideAnimator(int start, int end, View content)
		{
			ValueAnimator animator = ValueAnimator.OfInt(start, end);
			animator.Update += (object sender, ValueAnimator.AnimatorUpdateEventArgs e) =>
			{
				var value = (int)animator.AnimatedValue;
				ViewGroup.LayoutParams layoutParams = content.LayoutParameters;
				layoutParams.Height = value;
				content.LayoutParameters = layoutParams;
			};
			return animator;
		}

		void VisibleDrop(ParseItem drop, double distance)
		{
			RelativeLayout virtualDropContent = FindViewById<RelativeLayout>(Resource.Id.dropContent);
			if (virtualDropContent.ChildCount > 0)
				virtualDropContent.RemoveAllViews();

			var scale = 1 - distance / Constants.VISIBILITY_LIMITATIN_M;

			if (scale <= 0)
				return;

			ImageView realDrop = new ImageView(this);

			RelativeLayout.LayoutParams rParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
			rParams.AddRule(LayoutRules.CenterInParent);
			realDrop.SetScaleType(ImageView.ScaleType.FitCenter);
			realDrop.LayoutParameters = rParams;

			ImageLoader imageLoader = ImageLoader.Instance;
			imageLoader.DisplayImage(drop.IconURL.ToString(), realDrop);

			virtualDropContent.AddView(realDrop);

			var metrics = Resources.DisplayMetrics;
			var VDROP_MAX_SIZE = metrics.WidthPixels / 5;

			var vdropSize = VDROP_MAX_SIZE * scale;

			RelativeLayout.LayoutParams layoutParams = new RelativeLayout.LayoutParams((int)vdropSize, (int)vdropSize);
			realDrop.LayoutParameters = layoutParams;
			realDrop.RequestLayout();
		}

		#region current location
		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			switch (requestCode)
			{
				case Location_Request_Code:
					{
						if (grantResults.Length > 0 && grantResults[0] == (int)Permission.Granted)
						{
							GetDrops();
						}
						else {
							//SetMyLocationOnMap(false);
						}
						return;
					}
			}
		}

		public void OnLocationChanged(Location location)
		{
			for (int i = 0; i < mDrops.Count; i++)
			{
				var drop = mDrops[i];

				Location pointB = new Location("");
				pointB.Latitude = drop.Location_Lat;
				pointB.Longitude = drop.Location_Lnt;
				var distanceToB = pointB.DistanceTo(location);

				if (distanceToB < Constants.VISIBILITY_LIMITATIN_M)
					VisibleDrop(drop, distanceToB);
			}
		}

		public void OnProviderDisabled(string provider)
		{
			using (var alert = new AlertDialog.Builder(this))
			{
				alert.SetTitle("Please enable GPS");
				alert.SetMessage("Enable GPS in order to get your current location.");

				alert.SetPositiveButton("Enable", (senderAlert, args) =>
				{
					Intent intent = new Intent(global::Android.Provider.Settings.ActionLocationSourceSettings);
					StartActivity(intent);
				});

				alert.SetNegativeButton("Continue", (senderAlert, args) =>
				{
					alert.Dispose();
				});

				Dialog dialog = alert.Create();
				dialog.Show();
			}
		}

		public void OnProviderEnabled(string provider)
		{
		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
		}

		#endregion

	}
}
