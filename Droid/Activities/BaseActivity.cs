using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using Android.Graphics;
using Android.Support.V4.App;

using AndroidHUD;

using Xamarin.InAppBilling;
using Xamarin.InAppBilling.Utilities;

using Camera = Android.Hardware.Camera;
using Android.Locations;
using Android.Content;

namespace Drop.Droid
{
	[Activity(Label = "BaseActivity")]
	public class BaseActivity : FragmentActivity, TextureView.ISurfaceTextureListener, ILocationListener
	{
		readonly string[] PermissionsCamera =
		{
			Manifest.Permission.Camera
		};
		readonly string[] PermissionsLocation =
		{
		  Manifest.Permission.AccessCoarseLocation,
		  Manifest.Permission.AccessFineLocation
		};

		const int RequestCameraId = 0;
		const int RequestLocationId = 1;

		public LocationManager _locationManager;

		public Action LocationResultPermissionCallback = null;
		public Action<Location> LocationChangedCallback = null;

		public Camera _camera;
		public TextureView _textureView;

		public Product _selectedProduct;
		public InAppBillingServiceConnection _serviceConnection;
		public IList<Product> _products; // contains three items

		AlertDialog.Builder alert;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			StartSetup();
		}

		protected override void OnResume()
		{
			base.OnResume();

			CheckCameraPermission();
		}

		protected override void OnPause()
		{
			base.OnPause();

			try
			{
				_camera.StopPreview();
				_camera.Release();
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}



		#region in-app billing
		private async Task GetInventory()
		{
			// Ask the open connection's billing handler to return a list of avilable products for the 
			// given list of items.
			// NOTE: We are asking for the Reserved Test Product IDs that allow you to test In-App
			// Billing without actually making a purchase.
			_products = await _serviceConnection.BillingHandler.QueryInventoryAsync(new List<string> {
				"dropdistantcache",
				"noexpirydrop",
				"opendistantcache",
				ReservedTestProductIDs.Canceled
			}, ItemType.Product);

			// Were any products returned?
			if (_products == null)
			{
				// No, abort
				return;
			}
		}

		public void StartSetup()
		{
			// A Licensing and In-App Billing public key is required before an app can communicate with
			// Google Play, however you DON'T want to store the key in plain text with the application.
			// The Unify command provides a simply way to obfuscate the key by breaking it into two or
			// or more parts, specifying the order to reassemlbe those parts and optionally providing
			// a set of key/value pairs to replace in the final string. 
			string value = Security.Unify(Constants.IN_APP_BILLING_PUBLIC_KEY, new int[] { 0, 1, 2, 3 });

			// Create a new connection to the Google Play Service
			_serviceConnection = new InAppBillingServiceConnection(this, value);
			_serviceConnection.OnConnected += () =>
			{
				// Attach to the various error handlers to report issues
				_serviceConnection.BillingHandler.OnGetProductsError += (int responseCode, Bundle ownedItems) =>
				{
					Console.WriteLine("Error getting products");
				};

				_serviceConnection.BillingHandler.OnInvalidOwnedItemsBundleReturned += (Bundle ownedItems) =>
				{
					Console.WriteLine("Invalid owned items bundle returned");
				};

				_serviceConnection.BillingHandler.OnProductPurchasedError += (int responseCode, string sku) =>
				{
					Console.WriteLine("Error purchasing item {0}", sku);
				};

				_serviceConnection.BillingHandler.OnPurchaseConsumedError += (int responseCode, string token) =>
				{
					Console.WriteLine("Error consuming previous purchase");
				};

				_serviceConnection.BillingHandler.InAppBillingProcesingError += (message) =>
				{
					Console.WriteLine("In app billing processing error {0}", message);
				};

				_serviceConnection.BillingHandler.OnProductPurchased += (int response, Purchase purchase, string purchaseData, string purchaseSignature) =>
				{

					AlertDialog.Builder alert = new AlertDialog.Builder(this);
					alert.SetTitle("Your Operation successed!");
					alert.SetMessage("Purchased drop --- .");
					alert.SetPositiveButton("Ok", (senderAlert, args) =>
					{
						Toast.MakeText(this, "OK!", ToastLength.Short).Show();
					});

					alert.SetNegativeButton("Cancel", (senderAlert, args) =>
					{
						Toast.MakeText(this, "Cancelled!", ToastLength.Short).Show();
					});

					Dialog dialog = alert.Create();
					dialog.Show();
				};

				GetInventory();
			};

			// Attempt to connect to the service
			_serviceConnection.Connect();
		}
		#endregion

		#region grant Camera & Location access permission
		public void CheckCameraPermission()
		{
			if ((int)Build.VERSION.SdkInt < 23)
			{
				StartLiveCamera();

				if (LocationResultPermissionCallback != null)
					LocationResultPermissionCallback();
			}
			else {
				RequestCameraPermission();
				RequestLocationPermission();
			}
		}
		void RequestCameraPermission()
		{
			const string cPermission = Manifest.Permission.Camera;
			if (CheckSelfPermission(cPermission) == (int)Permission.Granted)
			{
				StartLiveCamera();
				return;
			}

			if (ShouldShowRequestPermissionRationale(cPermission))
			{
				ShowMessageBox(null, "Camera access is required to show live camera.", "Cancel", new[] { "OK" }, SendingCameraPermissionRequest);
				return;
			}

			SendingCameraPermissionRequest();
		}
		void RequestLocationPermission()
		{
			const string permission = Manifest.Permission.AccessFineLocation;
			if (CheckSelfPermission(permission) == (int)Permission.Granted)
			{
				if (LocationResultPermissionCallback != null)
					LocationResultPermissionCallback();
				
				return;
			}
			
			if (ShouldShowRequestPermissionRationale(permission))
			{
				ShowMessageBox(null, "Location access is required to determine your location for drops.", "Cancel", new[] { "OK" }, SendingPermissionRequest);
				return;
			}
			SendingPermissionRequest();
		}
		void SendingCameraPermissionRequest()
		{
			ActivityCompat.RequestPermissions(this, PermissionsCamera, RequestCameraId);
		}
		void SendingPermissionRequest()
		{
			ActivityCompat.RequestPermissions(this, PermissionsLocation, RequestLocationId);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
			switch (requestCode)
			{
				case RequestLocationId:
					{
						if (grantResults.Length > 0 && grantResults[0] == (int)Permission.Granted)
						{
							if (LocationResultPermissionCallback != null)
								LocationResultPermissionCallback();
						}
						else
						{
						}
					}
					break;
				case RequestCameraId:
					{
						if (grantResults[0] == Permission.Granted)
						{
							StartLiveCamera();
						}
						else
						{
						}
					}
					break;
			}
		}


		#endregion

		#region location 
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

		public void OnLocationChanged(Location location)
		{
			if (LocationChangedCallback != null)
				LocationChangedCallback(location);
		}
		public void OnProviderEnabled(string provider){}
		public void OnStatusChanged(string provider, Availability status, Bundle extras){}
		#endregion
		#region camera
		public void OnSurfaceTextureAvailable(SurfaceTexture surface, int w, int h)
		{
			_camera = Camera.Open();

			_textureView.LayoutParameters = new RelativeLayout.LayoutParams(w, h);

			try
			{
				_camera.SetPreviewTexture(surface);
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
			if (_camera != null)
			{
				try
				{
					var display = this.WindowManager.DefaultDisplay;
					if (display.Rotation == SurfaceOrientation.Rotation0)
						_camera.SetDisplayOrientation(90);
					else
						_camera.SetDisplayOrientation(180);
					_camera.StartPreview();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
		{
			return true;
		}
		#endregion



		void StartLiveCamera()
		{
			if (this.Class.Name == "SplashActivity") return;

			_textureView = FindViewById<TextureView>(Resource.Id.textureCamera);
			_textureView.SurfaceTextureListener = this;
		}

		public Location GetGPSLocation()
		{
			Location currentLocation = null;
			if (_locationManager.IsProviderEnabled(LocationManager.GpsProvider))
			{
				_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
				currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
				_locationManager.RemoveUpdates(this);
			}
			else if (_locationManager.IsProviderEnabled(LocationManager.NetworkProvider))
			{
				_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
				currentLocation = _locationManager.GetLastKnownLocation(LocationManager.NetworkProvider);
				_locationManager.RemoveUpdates(this);
			}

			if (currentLocation == null)
			{
				currentLocation = new Location("");
				currentLocation.Latitude = Constants.LOCATION_AUSTRALIA[0];
				currentLocation.Longitude = Constants.LOCATION_AUSTRALIA[1];
			}
			return currentLocation;
		}





		public void ShowLoadingView(string title)
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Show(this, title, -1, MaskType.Black);
			});
		}

		public void HideLoadingView()
		{
			RunOnUiThread(() =>
			{
				AndHUD.Shared.Dismiss(this);
			});
		}

		public void ShowMessageBox(string title, string message, bool isFinish = false)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetCancelable(false);
			alert.SetPositiveButton("OK", delegate { if (isFinish) Finish(); });
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		public void ShowMessageBox(string title, string message, string cancelButton, string[] otherButtons, Action successHandler)
		{
			alert = new AlertDialog.Builder(this);
			alert.SetTitle(title);
			alert.SetMessage(message);
			alert.SetPositiveButton("Cancel", (senderAlert, args) =>
			{
			});
			alert.SetNegativeButton("OK", (senderAlert, args) =>
			{
				successHandler();
			});
			RunOnUiThread(() =>
			{
				alert.Show();
			});
		}

		protected MediaFile ByteDataFromImage(ImageView imageView)
		{
			imageView.BuildDrawingCache(true);
			Bitmap bmap = imageView.GetDrawingCache(true);
			imageView.SetImageBitmap(bmap);
			Bitmap bitmap = Bitmap.CreateBitmap(imageView.GetDrawingCache(true));

			//var bitmap = BitmapFactory.DecodeResource(Resources, resourceId);
			Bitmap newBitmap = scaleDown(bitmap, Constants.MDROP_MAX_SIZE, true);

			var stream = new MemoryStream();

			newBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
			var fileBytes = stream.ToArray();

			var fileName = "Image_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".png";

			return new MediaFile(fileName, fileBytes);
		}

		protected Bitmap GetImageBitmapFromUrl(string url)
		{
			Bitmap imageBitmap = null;

			using (var webClient = new WebClient())
			{
				var imageBytes = webClient.DownloadData(url);
				if (imageBytes != null && imageBytes.Length > 0)
				{
					imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
				}
			}

			return imageBitmap;
		}

		public static Bitmap scaleDown(Bitmap realImage, float maxImageSize, bool filter)
		{
			float ratio = Math.Min((float)maxImageSize / realImage.Width, (float)maxImageSize / realImage.Height);
			int width = (int)Math.Round((float)ratio * realImage.Width);
			int height = (int)Math.Round((float)ratio * realImage.Height);

			Bitmap newBitmap = Bitmap.CreateScaledBitmap(realImage, width, height, filter);
			return newBitmap;
		}

		public long SetMaxDate(int months)
		{
			DateTime _dt_now = DateTime.Now;
			DateTime _start = new DateTime(1970, 1, 1);
			TimeSpan ts = (_dt_now - _start);

			//Add Days to SetMax Days;
			int noOfDays = ts.Days + months * 30;
			return (long)(TimeSpan.FromDays(noOfDays).TotalMilliseconds);
		}

		public long SetMinDate()
		{
			DateTime _dt_now = DateTime.Now;
			DateTime _start = new DateTime(1970, 1, 1);
			TimeSpan ts = (_dt_now - _start);

			//Add Days to SetMax Days;
			int noOfDays = ts.Days;
			return (long)(TimeSpan.FromDays(noOfDays).TotalMilliseconds);
		}
	}
}
