
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using Plugin.Media;

namespace Drop.Droid
{
	[Activity(Label = "DropItemActivity")]
	public class DropItemActivity : BaseActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
	{
		const int Location_Request_Code = 0;
		const int HomeFragment_Id = 0;

		Location _currentLocation;
		LocationManager _locationManager;

		private ItemModel ItemModel { get; set; }

		ImageView btnActionItem;

		EditText txtName, txtDescription, txtPassword;

		ImageView btnDropTextSymbol, btnDropImageSymbol, btnDropVideoSymbol, btnDropLinkSymbol;
		ImageView imgDropIcon;

		TextView lblLocationLat, lblLocationLog;

		CheckBox checkEveryone, checkOnlyMe, checkSpecificUser;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.DropItemActivity);

			_locationManager = GetSystemService(Context.LocationService) as LocationManager;

			CrossMedia.Current.Initialize();

			ItemModel = new ItemModel();

			SetUIVariablesAndActions();
			//SetInputBinding();
		}

		private void SetUIVariablesAndActions()
		{
			#region UI Variables
			txtName = FindViewById<EditText>(Resource.Id.txtName);
			txtDescription = FindViewById<EditText>(Resource.Id.txtDescription);
			txtPassword = FindViewById<EditText>(Resource.Id.txtPassword);

			btnDropTextSymbol = FindViewById<ImageView>(Resource.Id.btnDropTextSymbol);
			btnDropImageSymbol = FindViewById<ImageView>(Resource.Id.btnDropImageSymbol);
			btnDropVideoSymbol = FindViewById<ImageView>(Resource.Id.btnDropVideoSymbol);
			btnDropLinkSymbol = FindViewById<ImageView>(Resource.Id.btnDropLinkSymbol);

			imgDropIcon = FindViewById<ImageView>(Resource.Id.imgDropIcon);

			lblLocationLat = FindViewById<TextView>(Resource.Id.lblLocationLat);
			lblLocationLog = FindViewById<TextView>(Resource.Id.lblLocationLog);

			checkEveryone = FindViewById<CheckBox>(Resource.Id.ActionEveryone);
			checkOnlyMe = FindViewById<CheckBox>(Resource.Id.ActionOnlyMe);
			checkSpecificUser = FindViewById<CheckBox>(Resource.Id.ActionSpecificUser);

			btnActionItem = FindViewById<ImageView>(Resource.Id.ActionItem);
			#endregion

			//Actions
			FindViewById<ImageView>(Resource.Id.ActionCollpseName).Click += ActionCollpse;
			FindViewById<ImageView>(Resource.Id.ActionCollpseIcon).Click += ActionCollpse;
			FindViewById<ImageView>(Resource.Id.ActionCollpseLocation).Click += ActionCollpse;
			FindViewById<ImageView>(Resource.Id.ActionCollpsePermission).Click += ActionCollpse;
			FindViewById<ImageView>(Resource.Id.ActionCollpsePassword).Click += ActionCollpse;
			FindViewById<ImageView>(Resource.Id.ActionCollpseExpire).Click += ActionCollpse;
			FindViewById<ImageView>(Resource.Id.ActionCollpseShare).Click += ActionCollpse;

			FindViewById(Resource.Id.ActionItem).Click += ActionItem;

			FindViewById(Resource.Id.ActionDefaultIcon1).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon2).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon3).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon4).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon5).Click += ActionDefaultIcon;

			FindViewById(Resource.Id.ActionCurrentLocation).Click += ActionCurrentLocation;
			FindViewById(Resource.Id.ActionCustomLocation).Click += ActionCustomLocation;

			checkEveryone.Click += ActionVisibility;
			checkOnlyMe.Click += ActionVisibility;
			checkSpecificUser.Click += ActionVisibility;

			FindViewById(Resource.Id.ActionPassword).Click += ActionPassword;

			FindViewById<LinearLayout>(Resource.Id.ActionDropItem).Click += ActionDropItem;

			FindViewById(Resource.Id.ActionBack).Click += ActionBack;
		}



		private void SetInputBinding()
		{
			this.SetBinding(() => ItemModel.Name, () => txtName.Text, BindingMode.TwoWay);
			this.SetBinding(() => ItemModel.Description, () => txtDescription.Text, BindingMode.TwoWay);

			this.SetBinding(() => ItemModel.Password, () => txtPassword.Text, BindingMode.TwoWay);
		}


		#region Actions
		void ActionCollpse(object sender, EventArgs e)
		{
			switch (int.Parse(((ImageView)sender).Tag.ToString()))
			{
				case Constants.TAG_D_COLLEPS_NAME:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewName));
					break;
				case Constants.TAG_D_COLLEPS_ICON:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewICON));
					break;
				case Constants.TAG_D_COLLEPS_LOCATION:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewLocation));
					break;
				case Constants.TAG_D_COLLEPS_PERMISSION:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewPermission));
					break;
				case Constants.TAG_D_COLLEPS_PASSWORD:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewPassword));
					break;
				case Constants.TAG_D_COLLEPS_EXPIRY:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewExpiry));
					break;
				case Constants.TAG_D_COLLEPS_SHARE:
					CollepseAnimation(FindViewById<LinearLayout>(Resource.Id.viewShare));
					break;
				default:
					break;
			}
		}
		async void ActionDropItem(object sender, EventArgs e)
		{
			ItemModel.Name = txtName.Text;
			ItemModel.Description = txtDescription.Text;
			ItemModel.Password = txtPassword.Text;

			if (!ItemModel.IsValidDrop())
			{
				ShowMessageBox(null, Constants.STR_DROP_INVALID);
				return;
			}

			ShowLoadingView(Constants.STR_LOADING);

			var result = await ParseService.AddDropItem(ItemModel.parseItem);

			HideLoadingView();

			if (result == Constants.STR_STATUS_SUCCESS)
				ShowMessageBox(null, Constants.STR_DROP_SUCCESS_MSG);
			else
				ShowMessageBox(null, result);
		}

		void ActionItem(object sender, EventArgs e)
		{
			PopupMenu popup = new PopupMenu(this, btnActionItem);
			popup.Menu.Add(1, Constants.INDEX_ANDROID_TEXT, 1, Constants.TYPE_ATTACH_DROID[0]);
			popup.Menu.Add(1, Constants.INDEX_ANDROID_IMAGE_FROM_LIBRARY, 2, Constants.TYPE_ATTACH_DROID[1]);
			popup.Menu.Add(1, Constants.INDEX_ANDROID_VIDEO_FROM_LIBRARY, 3, Constants.TYPE_ATTACH_DROID[2]);
			popup.Menu.Add(1, Constants.INDEX_ANDROID_IMAGE_FROM_CAMERA, 4, Constants.TYPE_ATTACH_DROID[3]);
			popup.Menu.Add(1, Constants.INDEX_ANDROID_VIDEO_FROM_CAMERA, 5, Constants.TYPE_ATTACH_DROID[4]);
			popup.Menu.Add(1, Constants.INDEX_ANDROID_OTHER, 6, Constants.TYPE_ATTACH_DROID[5]);

			popup.MenuItemClick += SelectedAttachType;

			popup.Show();
		}
		void SelectedAttachType(object sender, PopupMenu.MenuItemClickEventArgs e)
		{
			switch (e.Item.ItemId)
			{
				case Constants.INDEX_ANDROID_TEXT:
					//CrossMedia.Current.PickPhotoAsync();
					break;
				case Constants.INDEX_ANDROID_IMAGE_FROM_LIBRARY:
					SelectAttachFile("library", "image");
					break;
				case Constants.INDEX_ANDROID_VIDEO_FROM_LIBRARY:
					SelectAttachFile("library", "video");
					break;
				case Constants.INDEX_ANDROID_IMAGE_FROM_CAMERA:
					SelectAttachFile("camera", "image");
					break;
				case Constants.INDEX_ANDROID_VIDEO_FROM_CAMERA:
					SelectAttachFile("camera", "video");
					break;
				case Constants.INDEX_ANDROID_OTHER:
					break;
			}
		}
		async void SelectAttachFile(string fromWhere, string type)
		{
			Plugin.Media.Abstractions.MediaFile file = null;
			if (type == "image")
			{
				if (fromWhere == "library")
					file = await CrossMedia.Current.PickPhotoAsync();
				else if (fromWhere == "camera")
				{
					if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
						return;
					file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
					{
						Directory = "Drop",
						Name = "Image_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".png"
					});
				}

				if (file == null) return;

				while (System.IO.File.ReadAllBytes(file.Path).Length == 0)
				{
					System.Threading.Thread.Sleep(1);
				}

				var fileBytes = System.IO.File.ReadAllBytes(file.Path);
				var fileName = "Image_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".png";

				ItemModel.Image = new MediaFile(fileName, fileBytes);

				btnDropImageSymbol.SetImageResource(Resource.Drawable.icon_image_sel);
			}
			else if (type == "video")
			{
				if (fromWhere == "library")
					file = await CrossMedia.Current.PickVideoAsync();
				else if (fromWhere == "camera")
				{
					if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsPickPhotoSupported)
						return;
					file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
					{
						Directory = "Drop",
						Quality = Plugin.Media.Abstractions.VideoQuality.Medium,
						Name = "Video_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".mp4",
					});
				}

				if (file == null) return;

				byte[] fileBytes1;
				using (var memoryStream = new MemoryStream())
				{
					file.GetStream().CopyTo(memoryStream);
					fileBytes1 = memoryStream.ToArray();
				}

				var fileName = "Video_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".mp4";
				
				ItemModel.Video = new MediaFile(fileName, fileBytes1);

				btnDropVideoSymbol.SetImageResource(Resource.Drawable.icon_video_sel);
			}
		}


		void ActionDefaultIcon(object sender, EventArgs e)
		{
			int strIconName = 0;
			switch (int.Parse(((ImageView)sender).Tag.ToString()))
			{
				case Constants.TAG_DEFAILT_ICON1:
					strIconName = Resource.Drawable.icon_drop6;
					break;
				case Constants.TAG_DEFAILT_ICON2:
					strIconName = Resource.Drawable.icon_drop7;
					break;
				case Constants.TAG_DEFAILT_ICON3:
					strIconName = Resource.Drawable.icon_drop8;
					break;
				case Constants.TAG_DEFAILT_ICON4:
					strIconName = Resource.Drawable.icon_drop9;
					break;
				case Constants.TAG_DEFAILT_ICON5:
					strIconName = Resource.Drawable.icon_drop10;
					break;
				default:
					break;
			}
			imgDropIcon.SetImageResource(strIconName);

			ItemModel.Icon = ByteDataFromImage(strIconName);
		}

		void ActionCurrentLocation(object sender, EventArgs e)
		{
			var checkBox = sender as CheckBox;
			checkBox.Selected = !checkBox.Selected;

			if (checkBox.Selected)
			{
				var lResult = GetGPSLocation();
				ItemModel.Location_Lat = lResult.Latitude;
				ItemModel.Location_Lnt = lResult.Longitude;
				lblLocationLat.Text = "Lat: " + ItemModel.Location_Lat.ToString("F2");
				lblLocationLog.Text = "Log: " + ItemModel.Location_Lnt.ToString("F2");
			}
			else
			{
				ItemModel.Location_Lat = Constants.LOCATION_AUSTRALIA[0];
				ItemModel.Location_Lnt = Constants.LOCATION_AUSTRALIA[1];
				lblLocationLat.Text = "Lat: " + ItemModel.Location_Lat.ToString("F2");
				lblLocationLog.Text = "Log: " + ItemModel.Location_Lnt.ToString("F2");
			}
		}

		private void ActionCustomLocation(object sender, EventArgs e)
		{
			var nextActivity = new Intent(this, typeof(DropLocationActivity));
			StartActivityForResult(nextActivity, HomeFragment_Id);
			OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
		}
		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (resultCode == Result.Ok)
			{
				ItemModel.Location_Lat = data.GetDoubleExtra("Latitude", Constants.LOCATION_AUSTRALIA[0]);
				ItemModel.Location_Lnt = data.GetDoubleExtra("Longitude", Constants.LOCATION_AUSTRALIA[1]);
				lblLocationLat.Text = "Lat: " + ItemModel.Location_Lat.ToString("F2");
				lblLocationLog.Text = "Log: " + ItemModel.Location_Lnt.ToString("F2");
			}
		}

		void ActionVisibility(object sender, EventArgs e)
		{
			checkEveryone.Checked = false;
			checkOnlyMe.Checked = false;
			checkSpecificUser.Checked = false;

			((CheckBox)sender).Checked = true;

			ItemModel.Visibility = int.Parse(((CheckBox)sender).Tag.ToString());
		}

		void ActionPassword(object sender, EventArgs e)
		{
			txtPassword.Enabled = ((CheckBox)sender).Checked;
			if (!((CheckBox)sender).Checked)
				txtPassword.Text = string.Empty;
		}

		void ActionBack(object sender, EventArgs e)
		{
			base.OnBackPressed();
			OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
		}

		#endregion

		void CollepseAnimation(LinearLayout content)
		{
			if (content.Visibility.Equals(ViewStates.Gone))
			{
				content.Visibility = ViewStates.Visible;

				int widthSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				int heightSpec = View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
				content.Measure(widthSpec, heightSpec);

				ValueAnimator mAnimator = slideAnimator(0, content.MeasuredHeight, content);
				mAnimator.Start();
			}
			else {
				int finalHeight = content.Height;

				ValueAnimator mAnimator = slideAnimator(finalHeight, 0, content);
				mAnimator.Start();
				mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
				{
					content.Visibility = ViewStates.Gone;
				};
			}
		}

		private ValueAnimator slideAnimator(int start, int end, LinearLayout content)
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
			_currentLocation = location;
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
			_currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
		}

		public void OnStatusChanged(string provider, Availability status, Bundle extras)
		{
		}

		private Location GetGPSLocation()
		{
			_locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 2000, 1, this);
			_currentLocation = _locationManager.GetLastKnownLocation(LocationManager.GpsProvider);
			_locationManager.RemoveUpdates(this);

			if (_currentLocation == null)
			{
				_currentLocation.Latitude = Constants.LOCATION_AUSTRALIA[0];
				_currentLocation.Longitude = Constants.LOCATION_AUSTRALIA[1];
			}
			return _currentLocation;
		}
		#endregion
	}
}
