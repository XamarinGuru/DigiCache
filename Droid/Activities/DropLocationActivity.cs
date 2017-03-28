
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;

namespace Drop.Droid
{
	[Activity(Label = "DropLocationActivity")]
	public class DropLocationActivity : BaseActivity, IOnMapReadyCallback
	{
		LatLng _currentLocation;

		SupportMapFragment _mapFragment;
		GoogleMap _map = null;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.DropLocationLayout);

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

				SetMyLocationOnMap();
			}
		}

		public void DragMapPinProcess(CameraPosition cameraPos)
		{
			_currentLocation = cameraPos.Target;
		}

		private void SetMyLocationOnMap()
		{
			if (_map == null) return;

			var myLocation = GetGPSLocation();

			CameraUpdate cu_center = CameraUpdateFactory.NewLatLngZoom(new LatLng(myLocation.Latitude, myLocation.Longitude), Constants.MAP_ZOOM_LEVEL);

			_map.MoveCamera(cu_center);
		}

		private class CameraPositionlHandler : Handler
		{
			CameraPosition _lastCameraPosition;
			GoogleMap _googleMap;
			DropLocationActivity rootVC;

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
