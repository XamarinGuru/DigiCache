
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Views;

namespace Drop.Droid
{
	[Activity(Label = "NearbyActivity")]
	public class NearbyActivity : BaseActivity, IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener
	{
		SupportMapFragment _mapFragment;
		GoogleMap _map = null;

		IList<ParseItem> mDrops;
		IList<string> dropIDs;

		ParseItem mSelectedDrop;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);

			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.NearbyLayout);

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
					if (drop.IsVisibilityByUser())
						AddDropInMap(drop, i);
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

				SetMyLocationOnMap();
				GetDrops();
			}
		}

		private void SetMyLocationOnMap()
		{
			if (_map == null) return;

			var myLocation = GetGPSLocation();

			CameraUpdate cu_center = CameraUpdateFactory.NewLatLngZoom(new LatLng(myLocation.Latitude, myLocation.Longitude), Constants.MAP_ZOOM_LEVEL);
			_map.MoveCamera(cu_center);
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
