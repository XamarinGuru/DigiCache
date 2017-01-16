using Foundation;
using System;
using UIKit;
using System.Drawing;
using CoreGraphics;
using MapKit;
using CoreLocation;
using System.Text;
using AddressBook;

namespace Drop.iOS
{
    public partial class DropLocationViewController : BaseViewController
    {
		public ItemModel ItemModel;

		private MKMapView mMapView;

		LocationPrediction objAutoCompleteLocationClass;
		LocationAutoCompleteTableSource objLocationAutoCompleteTableSource;
		string strAutoCompleteQuery;

		public DropLocationViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_LOCATION)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			InitMapView();

			txtSearchBar.TextChanged += SearchTextChanged;

			StringBuilder builderLocationAutoComplete = new StringBuilder(Constants.GOOGLE_AUTO_FILL_URL);
			builderLocationAutoComplete.Append("?input={0}").Append("&key=").Append(Constants.GOOGLE_MAP_API_KEY);
			strAutoCompleteQuery = builderLocationAutoComplete.ToString();
			builderLocationAutoComplete.Clear();
			builderLocationAutoComplete = null;
		}

		public override void ViewWillLayoutSubviews()
		{
			viewMapContent.LayoutIfNeeded();
			viewMapContent.Layer.CornerRadius = 5;
			viewMapContent.ClipsToBounds = true;

			if (mMapView != null && viewMapContent != null && viewMapContent.Window != null)
			{
				RepaintMap();
			}
		}

		void InitMapView()
		{
			mMapView = new MKMapView(RectangleF.Empty);
			mMapView.ShowsUserLocation = false;
			mMapView.ZoomEnabled = true;
			mMapView.ScrollEnabled = true;
			mMapView.MapType = MKMapType.Satellite;

			var customDelegate = new CustomMapViewDelegate();
			customDelegate.OnRegionChanged += TheMapView_OnRegionChanged;
			mMapView.Delegate = customDelegate;

			var lResult = LocationHelper.GetLocationResult();
			if ((int)lResult.Latitude == 0 || (int)lResult.Longitude == 0)
				SetMapPin(34.052235f, -118.243683f);
			else
				SetMapPin(lResult.Latitude, lResult.Longitude);
		}

		async void SearchTextChanged(object sender, UISearchBarTextChangedEventArgs e)
		{
			if (txtSearchBar.Text.Length > 2)
			{

				string strFullURL = string.Format(strAutoCompleteQuery, txtSearchBar.Text);
				objAutoCompleteLocationClass = await RestRequestClass.LocationAutoComplete(strFullURL);


				if (objAutoCompleteLocationClass != null && objAutoCompleteLocationClass.status == "OK")
				{
					if (objAutoCompleteLocationClass.predictions.Count > 0)
					{
						if (objLocationAutoCompleteTableSource != null)
						{
							objLocationAutoCompleteTableSource.LocationRowSelectedEventAction -= LocationSelectedFromAutoFill;
							objLocationAutoCompleteTableSource = null;
						}

						locationListTableView.Hidden = false;
						bgTableView.Hidden = false;
						objLocationAutoCompleteTableSource = new LocationAutoCompleteTableSource(this, objAutoCompleteLocationClass.predictions, bgTableView, SetMapPin);
						objLocationAutoCompleteTableSource.LocationRowSelectedEventAction += LocationSelectedFromAutoFill;
						locationListTableView.Source = objLocationAutoCompleteTableSource;
						locationListTableView.ReloadData();
					}
					else
					{
						locationListTableView.Hidden = true;
						bgTableView.Hidden = true;
					}
				}
			}
		}

		void LocationSelectedFromAutoFill(Prediction objPrediction)
		{
			Console.WriteLine(objPrediction.description);
			txtSearchBar.ResignFirstResponder();
		}

		public void RepaintMap()
		{
			foreach (var subview in viewMapContent.Subviews)
			{
				subview.RemoveFromSuperview();
			}

			viewMapContent.LayoutIfNeeded();
			var width = viewMapContent.Frame.Width;
			var height = viewMapContent.Frame.Height;
			mMapView.Frame = new CGRect(0, 0, width, height);

			viewMapContent.AddSubview(mMapView);
		}


		public  void TheMapView_OnRegionChanged(object sender, MKMapViewChangeEventArgs e)
		{
			ItemModel.Location_Lat = mMapView.Region.Center.Latitude;
			ItemModel.Location_Lnt = mMapView.Region.Center.Longitude;

			var geoCoder = new CLGeocoder();
			geoCoder.CancelGeocode();
			geoCoder.ReverseGeocodeLocation(new CLLocation(ItemModel.Location_Lat, ItemModel.Location_Lnt), (placemarks, error) =>
		   {
			   var placemark = placemarks[0];

			   try
			   {
				   var DName = placemark.Name;
				   var Street = placemark.AddressDictionary.ValueForKey(ABPersonAddressKey.Street).ToString();
				   var City = placemark.AddressDictionary.ValueForKey(ABPersonAddressKey.City).ToString();
				   var State = placemark.AddressDictionary.ValueForKey(ABPersonAddressKey.State).ToString();
				   var PostalCode = placemark.AddressDictionary.ValueForKey(ABPersonAddressKey.Zip).ToString();
				   txtSearchBar.Text = Street + ", " + City + ", " + State + ", " + PostalCode;
			   }
			   catch (Exception ex)
			   {
				   txtSearchBar.Text = "Search or Position Map";
			   }
		   });

		}

		void SetMapPin(double lat, double lng)
		{
			InvokeOnMainThread(() =>
			{
				CLLocationCoordinate2D mapCenter = new CLLocationCoordinate2D(lat, lng);
				MKCoordinateRegion mapRegion = MKCoordinateRegion.FromDistance(mapCenter, 1000, 1000);
				mMapView.CenterCoordinate = mapCenter;
				mMapView.Region = mapRegion;
			});
		}

		public class CustomMapViewDelegate : MKMapViewDelegate
		{
			public event EventHandler<MKMapViewChangeEventArgs> OnRegionChanged;

			public override void RegionChanged(MKMapView mapView, bool animated)
			{
				if (OnRegionChanged != null)
				{
					OnRegionChanged(mapView, new MKMapViewChangeEventArgs(animated));
				}
			}
		}
    }
}