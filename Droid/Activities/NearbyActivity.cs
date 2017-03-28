
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
	public class NearbyActivity : BaseActivity, TextureView.ISurfaceTextureListener, IOnMapReadyCallback, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback, GoogleMap.IOnMarkerClickListener
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

		protected override void OnResume()
		{
			base.OnResume();

			_textureView = FindViewById<TextureView>(Resource.Id.textureCamera);
			_textureView.SurfaceTextureListener = this;
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
					if (drop.IsVisibilityByUser())
						AddDropInMap(drop, i);

					//MarkerOptions markerOpt = new MarkerOptions();
					//markerOpt.SetPosition(new LatLng(drop.Location_Lat, drop.Location_Lnt));

					//var metrics = Resources.DisplayMetrics;
					//var wScreen = metrics.WidthPixels;
					                       
					//Bitmap bmp = GetImageBitmapFromUrl(drop.IconURL.ToString());
					//Bitmap newBitmap = scaleDown(bmp, wScreen / 15, true);
					//markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

					//RunOnUiThread(() =>
					//{
					//	var marker = _map.AddMarker(markerOpt);
					//	dropIDs.Add(marker.Id);

					//});
				}

				HideLoadingView();
			});
		}

		void AddDropInMap(ParseItem drop, int index)
		{
			MarkerOptions markerOpt = new MarkerOptions();
			markerOpt.SetPosition(new LatLng(drop.Location_Lat, drop.Location_Lnt));

			var metrics = Resources.DisplayMetrics;
			var wScreen = metrics.WidthPixels;

			Bitmap bmp = GetImageBitmapFromUrl(drop.IconURL.ToString());
			Bitmap newBitmap = scaleDown(bmp, wScreen / 15, true);
			markerOpt.SetIcon(BitmapDescriptorFactory.FromBitmap(newBitmap));

			RunOnUiThread(() =>
			{
				var marker = _map.AddMarker(markerOpt);
				dropIDs.Add(marker.Id);

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
				cu_center = CameraUpdateFactory.NewLatLngZoom(new LatLng(myLocation.Latitude, myLocation.Longitude), Constants.MAP_ZOOM_LEVEL);
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
				var location = GetGPSLocation();
				Location pointB = new Location("");
				pointB.Latitude = mSelectedDrop.Location_Lat;
				pointB.Longitude = mSelectedDrop.Location_Lnt;
				var distanceToB = pointB.DistanceTo(location);

				if (distanceToB > Constants.PURCHASE_DISTANCE)
				{
					PurchasePopUp myDiag = PurchasePopUp.newInstance(Constants.PURCHASE_TYPE.VIEW, OpenPurchase);
					myDiag.Show(FragmentManager, "Diag");
				}
				else
				{
					ViewDropDetail();
				}
			}
			else
			{
				//MyInputDialog myDiag = MyInputDialog.newInstance(Constants.STR_VERIFY_PASSWORD_TITLE, VerifyPassword);
				InputPopUp myDiag = InputPopUp.newInstance("Password?", VerifyPassword);
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

		void ViewDropDetail()
		{
			var nextActivity = new Intent(this, typeof(DropDetailActivity));
			Global.selectedDrop = mSelectedDrop;
			StartActivity(nextActivity);
			OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
		}

		void OpenPurchase()
		{
			_selectedProduct = _products[(int)Constants.PURCHASE_TYPE.VIEW];
			if (_selectedProduct != null)
				_serviceConnection.BillingHandler.BuyProduct(_selectedProduct);
		}
	}
}
