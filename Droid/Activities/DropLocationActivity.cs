
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

namespace Drop.Droid
{
	[Activity(Label = "DropLocationActivity")]
	public class DropLocationActivity : FragmentActivity, TextureView.ISurfaceTextureListener, IOnMapReadyCallback, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
	{
		Android.Hardware.Camera _camera;
		TextureView textureCamera;

		const int Location_Request_Code = 0;

		LatLng _currentLocation;
		LocationManager _locationManager;

		SupportMapFragment _mapFragment;
		GoogleMap _map = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.DropLocationLayout);

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			_mapFragment = (SupportMapFragment)SupportFragmentManager.FindFragmentById(Resource.Id.map);
			_mapFragment.GetMapAsync(this);

			FindViewById(Resource.Id.ActionConfirm).Click += delegate
			{
				Intent myIntent = new Intent(this, typeof(DropItemActivity));
				myIntent.PutExtra("Latitude", _currentLocation.Latitude);
				myIntent.PutExtra("Longitude", _currentLocation.Longitude);
				SetResult(Result.Ok, myIntent);
				//Finish();

				base.OnBackPressed();
				OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
			};

			FindViewById(Resource.Id.ActionBack).Click += delegate 
			{
				base.OnBackPressed();
				OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
			};

			textureCamera = FindViewById<TextureView>(Resource.Id.textureCamera);
			textureCamera.SurfaceTextureListener = this;
		}

		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
		{
			_camera = Android.Hardware.Camera.Open();

			textureCamera.LayoutParameters = new RelativeLayout.LayoutParams(w, h);

			try
			{
				_camera.SetPreviewTexture(surface);

				var display = this.WindowManager.DefaultDisplay;
				if (display.Rotation == SurfaceOrientation.Rotation0)
					_camera.SetDisplayOrientation(90);
				else
					_camera.SetDisplayOrientation(180);

				_camera.StartPreview();

			}
			catch (Java.IO.IOException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
		{
		}

		public void OnSurfaceTextureUpdated(SurfaceTexture surface)
		{
		}
		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			_camera.StopPreview();
			_camera.Release();

			return true;
		}

		#region google map
		private CameraPositionlHandler _cameraPositionHandler;

		public void OnMapReady(GoogleMap googleMap)
		{
			_map = googleMap;

			if (_map != null)
			{
				_map.MapType = GoogleMap.MapTypeSatellite;

				_cameraPositionHandler = new CameraPositionlHandler(_map, this);

				_map.CameraChange += OnCameraChanged;

				string[] PermissionsLocation =
				{
					Manifest.Permission.AccessCoarseLocation,
					Manifest.Permission.AccessFineLocation
				};
				//Explain to the user why we need to read the contacts
				ActivityCompat.RequestPermissions(this, PermissionsLocation, Location_Request_Code);

				SetMyLocationOnMap();
			}
		}

		public async void DragMapPinProcess(CameraPosition cameraPos)
		{
			try
			{
				_currentLocation = cameraPos.Target;
			}
			catch
			{
			}
		}

		private void SetMyLocationOnMap()
		{
			if (_map == null) return;

			var myLocation = GetGPSLocation();

			CameraUpdate cu_center = CameraUpdateFactory.NewLatLngZoom(new LatLng(myLocation.Latitude, myLocation.Longitude), Constants.MAP_ZOOM_LEVEL);

			_map.MoveCamera(cu_center);
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

			if (currentLocation == null)
			{
				currentLocation.Latitude = Constants.LOCATION_AUSTRALIA[0];
				currentLocation.Longitude = Constants.LOCATION_AUSTRALIA[1];
			}
			return currentLocation;
		}
		#endregion

		private class CameraPositionlHandler : Handler
		{
			private CameraPosition _lastCameraPosition;
			private GoogleMap _googleMap;
			private DropLocationActivity rootVC;

			public CameraPositionlHandler(GoogleMap googleMap, DropLocationActivity rootVC)
			{
				_googleMap = googleMap;
				this.rootVC = rootVC;
			}

			public override void HandleMessage(Message msg)
			{
				if (_googleMap != null)
				{
					if (msg.What == 1)
					{
						_lastCameraPosition = _googleMap.CameraPosition;
					}
					else if (msg.What == 2)
					{
						if (_lastCameraPosition.Equals(_googleMap.CameraPosition))
						{
							rootVC.DragMapPinProcess(_lastCameraPosition);
						}
					}
				}
			}
		}
		void OnCameraChanged(object sender, GoogleMap.CameraChangeEventArgs e)
		{
			_cameraPositionHandler.RemoveMessages(1);
			_cameraPositionHandler.RemoveMessages(2);
			_cameraPositionHandler.SendEmptyMessageDelayed(1, 300);
			_cameraPositionHandler.SendEmptyMessageDelayed(2, 600);
		}
		#endregion
	}
}
