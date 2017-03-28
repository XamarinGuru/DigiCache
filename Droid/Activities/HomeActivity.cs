
using System;
using System.Collections.Generic;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Views;
using Android.Widget;
using Parse;
using UniversalImageLoader.Core;

namespace Drop.Droid
{
	[Activity(Label = "HomeActivity")]
	public class HomeActivity : BaseActivity
	{
		ImageView aniBgView;
		LinearLayout aniContentView;

		int nInitialAniviewHeight;

		IList<ParseItem> mDrops;

		RelativeLayout virtualDropContent;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.HomeActivity);

			LocationResultPermissionCallback = GetDrops;
			LocationChangedCallback = OnLocationChangedCallback;

			var config = ImageLoaderConfiguration.CreateDefault(ApplicationContext);
			ImageLoader.Instance.Init(config);

			InitUISettings();
		}

		private void InitUISettings()
		{
			#region UI Variables
			aniBgView = FindViewById<ImageView>(Resource.Id.aniBgView);
			aniContentView = FindViewById<LinearLayout>(Resource.Id.aniContentView);
			virtualDropContent = FindViewById<RelativeLayout>(Resource.Id.dropContent);
			#endregion

			nInitialAniviewHeight = aniBgView.Height;

			#region Actions
			FindViewById<ImageView>(Resource.Id.ActionHome).Click += ActionHome;
			FindViewById<LinearLayout>(Resource.Id.ActionDropItem).Click += ActionDrop;
			FindViewById<LinearLayout>(Resource.Id.ActionDropNearby).Click += ActionDrop;
			FindViewById<LinearLayout>(Resource.Id.ActionDropSetting).Click += ActionDrop;
			#endregion
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
					{
						if (ParseUser.CurrentUser == null)
						{
							ShowMessageBox(Constants.STR_INVALID_USERINFO, null);
							return;
						}
						dropAC = new Intent(this, typeof(FavoriteActivity));
					}
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

		void OnLocationChangedCallback(Location location)
		{
			if (virtualDropContent.ChildCount > 0)
				virtualDropContent.RemoveAllViews();

			if (location == null) return;
			
			for (int i = 0; i < mDrops.Count; i++)
			{
				var drop = mDrops[i];

				Location pointB = new Location("");
				pointB.Latitude = drop.Location_Lat;
				pointB.Longitude = drop.Location_Lnt;
				var distanceToB = pointB.DistanceTo(location);

				if (distanceToB < Constants.PURCHASE_DISTANCE)
					VisibleDrop(drop, location);
			}
		}

		void VisibleDrop(ParseItem drop, Location uLocation)
		{
			Location dLocation = new Location("");
			dLocation.Latitude = drop.Location_Lat;
			dLocation.Longitude = drop.Location_Lnt;
			var distanceToB = dLocation.DistanceTo(uLocation);

			var scale = 1 - distanceToB / Constants.PURCHASE_DISTANCE;

			if (scale <= 0)
				return;

			var virtualDrop = LayoutInflater.From(this).Inflate(Resource.Layout.VirtualDrop, null);

			virtualDrop.Click += ActionDropDetail;

			virtualDrop.FindViewById<TextView>(Resource.Id.lblDistance).Text = distanceToB.ToString("F2") + " m";

			var imgIcon = virtualDrop.FindViewById<ImageView>(Resource.Id.imgIcon);
			ImageLoader imageLoader = ImageLoader.Instance;
			imageLoader.DisplayImage(drop.IconURL.ToString(), imgIcon);


			var screenWidth = Resources.DisplayMetrics.WidthPixels;
			var VDROP_MAX_SIZE = screenWidth / 10;

			var vdropSize = VDROP_MAX_SIZE * scale;

			var angle = DegreeBearing(uLocation, dLocation);
			var distanceScale = (distanceToB / Constants.PURCHASE_DISTANCE) * (screenWidth / 2);

			var posX = screenWidth / 2 - vdropSize / 2 + Math.Sin(angle) * distanceScale;
			var posY = screenWidth / 2 - vdropSize / 2 - Math.Cos(angle) * distanceScale;

			LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams((int)vdropSize, (int)vdropSize);
			imgIcon.LayoutParameters = layoutParams;

			RelativeLayout.LayoutParams layoutParams1 = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
			virtualDrop.LayoutParameters = layoutParams1;
			layoutParams1.LeftMargin = (int)(posX);
			layoutParams1.TopMargin = (int)(posY);
			virtualDrop.RequestLayout();
		
			virtualDropContent.AddView(virtualDrop);
		}

		void ActionDropDetail(object sender, EventArgs e)
		{
			Intent dropAC = new Intent(this, typeof(NearbyActivity));
			StartActivity(dropAC);
		}

		#region calculator virtual drop location
		double DegreeBearing(Location loc1, Location loc2)
		{
			;
			double fLat = ToRad(loc1.Latitude);
			double fLng = ToRad(loc1.Longitude);
			double tLat = ToRad(loc2.Latitude);
			double tLng = ToRad(loc2.Longitude);

			double dLon = tLng - fLng;
			double y = Math.Sin(dLon) * Math.Cos(tLat);
			double x = Math.Cos(fLat) * Math.Sin(tLat) - Math.Sin(fLat) * Math.Cos(tLat) * Math.Cos(dLon);
			double radian = Math.Atan2(y, x);
			return (radian);
		}

		double ToRad(double degree)
		{
			return degree * (Math.PI / 180);
		}
		double ToDegree(double radian)
		{
			return radian * 180 / Math.PI;
		}
		#endregion
	}
}
