
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Net;

namespace Drop.Droid
{
	[Activity(Label = "NearbyActivity")]
	public class NearbyActivity : FragmentActivity, IOnMapReadyCallback, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback, GoogleMap.IOnMarkerClickListener
	{
		const int Location_Request_Code = 0;

		LocationManager _locationManager;

		SupportMapFragment _mapFragment;
		GoogleMap _map = null;

		private IList<ParseItem> mDrops;
		private IList<string> dropIDs;

		ParseItem mSelectedDrop;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);

			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.NearbyLayout);

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			_mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			_mapFragment.GetMapAsync(this);

			FindViewById(Resource.Id.ActionBack).Click += delegate
			{
				base.OnBackPressed();
				OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
			};
		}

		void GetDrops()
		{
			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.STR_DROPS_LOADING);

				mDrops = ParseService.GetDropItems();

				if (_map == null) return;

				dropIDs = new List<string>();

				for (int i = 0; i < mDrops.Count; i++)
				{
					var drop = mDrops[i];

					MarkerOptions markerOpt = new MarkerOptions();
					markerOpt.SetPosition(new LatLng(drop.Location_Lat, drop.Location_Lnt));

					URL iconUri = new URL(drop.IconURL.ToString());
					var stream = iconUri.OpenConnection().InputStream;
					Bitmap bmp = BitmapFactory.DecodeStream(stream);
					markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(bmp));

					RunOnUiThread(() =>
					{
						var marker = _map.AddMarker(markerOpt);
						dropIDs.Add(marker.Id);

					});
				}

				HideLoadingView();
			});
		}

		#region google map

		public void OnMapReady(GoogleMap googleMap)
		{
			_map = googleMap;

			if (_map != null)
			{
				_map.MapType = GoogleMap.MapTypeSatellite;
				_map.SetOnMarkerClickListener(this);

				string[] PermissionsLocation =
				{
					Manifest.Permission.AccessCoarseLocation,
					Manifest.Permission.AccessFineLocation
				};
				//Explain to the user why we need to read the contacts
				ActivityCompat.RequestPermissions(this, PermissionsLocation, Location_Request_Code);

				SetMyLocationOnMap();
				GetDrops();
			}
		}

		private void SetMyLocationOnMap()
		{
			if (_map == null) return;

			var myLocation = GetGPSLocation();

			CameraUpdate cu_center;
			if (myLocation != null)
				cu_center = CameraUpdateFactory.NewLatLngZoom(new LatLng(myLocation.Latitude, myLocation.Longitude), 11);
			else
				cu_center = CameraUpdateFactory.NewLatLngZoom(new LatLng(Constants.LOCATION_AUSTRALIA[0], Constants.LOCATION_AUSTRALIA[1]), 11);

			_map.MoveCamera(cu_center);
		}
		#endregion

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
							//SetMyLocationOnMap(true);
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

		private Location GetGPSLocation()
		{
			_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
			Location currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
			_locationManager.RemoveUpdates(this);

			//if (currentLocation == null)
			//{
			//	currentLocation = new Location();
			//	currentLocation.Latitude = Constants.LOCATION_AUSTRALIA[0];
			//	currentLocation.Longitude = Constants.LOCATION_AUSTRALIA[1];
			//}
			return currentLocation;
		}
		#endregion

		public bool OnMarkerClick(Marker marker)
		{
			mSelectedDrop = new ParseItem();
			for (var i = 0; i < dropIDs.Count; i++)
			{
				if (marker.Id == dropIDs[i])
					mSelectedDrop = mDrops[i];
			}
			if (mSelectedDrop == null) return false;

			if (mSelectedDrop.Password == string.Empty || mSelectedDrop.Password == null)
			{
				var nextActivity = new Intent(this, typeof(DropDetailActivity));
				Global.selectedDrop = mSelectedDrop;
				StartActivity(nextActivity);
				OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
			}
			else {
				//MyInputDialog myDiag = MyInputDialog.newInstance(Constants.STR_VERIFY_PASSWORD_TITLE, VerifyPassword);
				MyInputDialog myDiag = MyInputDialog.newInstance("Password?", VerifyPassword);
				myDiag.Show(FragmentManager, "Diag");
			}
			return true;
		}

		void VerifyPassword(string text)
		{
			if (mSelectedDrop.Password == text)
			{
				var nextActivity = new Intent(this, typeof(DropDetailActivity));
				Global.selectedDrop = mSelectedDrop;
				StartActivity(nextActivity);
				OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
			}
			else {
				ShowMessageBox(null, Constants.STR_INVALID_PASSWORD_TITLE);
			}
		}

		void ShowLoadingView(string title)
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Show(this, title, -1, MaskType.Black);
			});
		}

		void HideLoadingView()
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Dismiss(this);
			});
		}

		public void ShowMessageBox(string title, string message, bool isFinish = false)
		{
			AlertDialog.Builder alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetCancelable(false);
			alert.SetPositiveButton("OK", delegate { if (isFinish) Finish(); });
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}
	}
}
