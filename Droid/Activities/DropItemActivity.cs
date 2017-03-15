
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
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
	public class DropItemActivity : BaseActivity, ActivityCompat.IOnRequestPermissionsResultCallback
	{
		LayoutInflater controlInflater = null;

		const int Location_Request_Code = 0;
		const int HomeFragment_Id = 0;

		private ItemModel ItemModel { get; set; }

		LinearLayout btnActionItem;

		EditText txtName, txtDescription, txtPassword;

		ImageView btnDropTextSymbol, btnDropImageSymbol, btnDropVideoSymbol, btnDropLinkSymbol;
		ImageView imgDropIcon;

		TextView lblLocationLat, lblLocationLog;

		CheckBox checkVisibilityEveryone, checkVisibilityOnlyMe, checkVisibilitySpecificUser;
		CheckBox checkModifyEveryone, checkModifyOnlyMe, checkModifySpecificUser;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			//SetContentView(Resource.Layout.DropItemActivity);

			CrossMedia.Current.Initialize();

			ItemModel = new ItemModel();

			SetUIVariablesAndActions();
			SetInputBinding();
		}

		private void SetUIVariablesAndActions()
		{
			//controlInflater = LayoutInflater.From(BaseContext);
			//var viewControl = controlInflater.Inflate(Resource.Layout.DropItemActivity, null);
			//var layoutParamsControl = new ActionBar.LayoutParams(ActionBar.LayoutParams.MatchParent, ActionBar.LayoutParams.MatchParent);
			//this.AddContentView(viewControl, layoutParamsControl);

			SetContentView(Resource.Layout.DropItemActivity);
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

			checkVisibilityEveryone = FindViewById<CheckBox>(Resource.Id.ActionVisibilityEveryone);
			checkVisibilityOnlyMe = FindViewById<CheckBox>(Resource.Id.ActionVisibilityOnlyMe);
			checkVisibilitySpecificUser = FindViewById<CheckBox>(Resource.Id.ActionVisibilitySpecificUser);

			checkModifyEveryone = FindViewById<CheckBox>(Resource.Id.ActionModifyEveryone);
			checkModifyOnlyMe = FindViewById<CheckBox>(Resource.Id.ActionModifyOnlyMe);
			checkModifySpecificUser = FindViewById<CheckBox>(Resource.Id.ActionModifySpecificUser);

			btnActionItem = FindViewById<LinearLayout>(Resource.Id.ActionItem);

			#endregion

			//Actions
			FindViewById<LinearLayout>(Resource.Id.ActionCollpseName).Click += ActionCollpse;
			FindViewById<LinearLayout>(Resource.Id.ActionCollpseIcon).Click += ActionCollpse;
			FindViewById<LinearLayout>(Resource.Id.ActionCollpseLocation).Click += ActionCollpse;
			FindViewById<LinearLayout>(Resource.Id.ActionCollpsePermission).Click += ActionCollpse;
			FindViewById<LinearLayout>(Resource.Id.ActionCollpsePassword).Click += ActionCollpse;
			FindViewById<LinearLayout>(Resource.Id.ActionCollpseModify).Click += ActionCollpse;
			FindViewById<LinearLayout>(Resource.Id.ActionCollpseExpire).Click += ActionCollpse;

			FindViewById(Resource.Id.ActionItem).Click += ActionItem;

			FindViewById(Resource.Id.ActionDefaultIcon1).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon2).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon3).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon4).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon5).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon6).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon7).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon8).Click += ActionDefaultIcon;
			FindViewById(Resource.Id.ActionDefaultIcon9).Click += ActionDefaultIcon;

			FindViewById(Resource.Id.ActionCustomLocation).Click += ActionCustomLocation;

			checkVisibilityEveryone.Click += ActionVisibility;
			checkVisibilityOnlyMe.Click += ActionVisibility;
			checkVisibilitySpecificUser.Click += ActionVisibility;

			checkModifyEveryone.Click += ActionModify;
			checkModifyOnlyMe.Click += ActionModify;
			checkModifySpecificUser.Click += ActionModify;

			FindViewById(Resource.Id.ActionPassword).Click += ActionPassword;
			FindViewById<LinearLayout>(Resource.Id.ActionDropItem).Click += ActionDropItem;
			FindViewById(Resource.Id.ActionBack).Click += ActionBack;

			CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewName));
			CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewICON));
			CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewLocation));
			CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewPermission));
			CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewModify));
			CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewPassword));
			CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewExpiry));

			var contentView = FindViewById<LinearLayout>(Resource.Id.contentView);
			var childs = GetAllChildren(contentView);
			for (int i = 0; i < childs.Count; i++)
			{
				if (childs[i] is EditText)
				{
					((EditText)childs[i]).TextChanged += (s, e) => { };
				}
			}
		}

		List<View> GetAllChildren(View view)
		{
			if (!(view is ViewGroup))
			{
				List<View> viewArrayList = new List<View>();
				viewArrayList.Add(view);
				return viewArrayList;
			}

			List<View> result = new List<View>();

			ViewGroup vg = (ViewGroup)view;
			for (int i = 0; i < vg.ChildCount; i++)
			{
				View child = vg.GetChildAt(i);
				List<View> viewArrayList = new List<View>();
				viewArrayList.Add(view);
				viewArrayList.AddRange(GetAllChildren(child));
				result.AddRange(viewArrayList);
			}
			return result;
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
			switch (int.Parse(((LinearLayout)sender).Tag.ToString()))
			{
				case Constants.TAG_COLLEPS_NAME:
					CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewName));
					break;
				case Constants.TAG_COLLEPS_ICON:
					CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewICON));
					break;
				case Constants.TAG_COLLEPS_LOCATION:
					CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewLocation));
					break;
				case Constants.TAG_COLLEPS_PERMISSION:
					CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewPermission));
					break;
				case Constants.TAG_COLLEPS_MODIFY:
					CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewModify));
					break;
				case Constants.TAG_COLLEPS_PASSWORD:
					CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewPassword));
					break;
				case Constants.TAG_COLLEPS_EXPIRY:
					CollepseAnimation(FindViewById<RelativeLayout>(Resource.Id.viewExpiry));
					break;
				default:
					break;
			}
		}
		async void ActionDropItem(object sender, EventArgs e)
		{
			if (!ItemModel.IsValidDrop())
			{
				ShowMessageBox(null, Constants.STR_DROP_INVALID);
				return;
			}

			ShowLoadingView(Constants.STR_LOADING);

			var result = await ParseService.AddDropItem(ItemModel.parseItem);

			HideLoadingView();

			if (result == Constants.STR_STATUS_SUCCESS)
			{
				ShowMessageBox(null, Constants.STR_DROP_SUCCESS_MSG);
			}
			else
			{
				ShowMessageBox(null, result);
			}
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
					MyInputDialog dropTextDialog = MyInputDialog.newInstance(Constants.STR_ATTACH_TEXT_TITLE, SetDropText);
					dropTextDialog.Show(FragmentManager, "Diag");
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
					MyInputDialog dropLinkDialog = MyInputDialog.newInstance(Constants.STR_ATTACH_OTHER_TITLE, SetOtherLink);
					dropLinkDialog.Show(FragmentManager, "Diag");
					break;
			}
		}

		void SetDropText(string text)
		{
			ItemModel.Text = text;
			btnDropTextSymbol.SetImageResource(Resource.Drawable.icon_text_sel);
		}

		void SetOtherLink(string text)
		{
			ItemModel.OtherLink = text;
			btnDropLinkSymbol.SetImageResource(Resource.Drawable.icon_link_sel);
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
			imgDropIcon.SetImageDrawable(((ImageView)sender).Drawable);

			ItemModel.Icon = ByteDataFromImage(imgDropIcon);
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
			checkVisibilityEveryone.Checked = false;
			checkVisibilityOnlyMe.Checked = false;
			checkVisibilitySpecificUser.Checked = false;

			((CheckBox)sender).Checked = true;

			ItemModel.Visibility = int.Parse(((CheckBox)sender).Tag.ToString());
		}

		void ActionModify(object sender, EventArgs e)
		{
			checkModifyEveryone.Checked = false;
			checkModifyOnlyMe.Checked = false;
			checkModifySpecificUser.Checked = false;

			((CheckBox)sender).Checked = true;

			ItemModel.Modify = int.Parse(((CheckBox)sender).Tag.ToString());
		}

		void ActionPassword(object sender, EventArgs e)
		{
			txtPassword.Enabled = ((CheckBox)sender).Checked;
			if (!((CheckBox)sender).Checked)
				txtPassword.Text = string.Empty;
		}

		void ActionShare(object sender, EventArgs e)
		{
			Bitmap bitmap = BitmapFactory.DecodeByteArray(ItemModel.Icon.fileData, 0, ItemModel.Icon.fileData.Length);

			var tempFilename = "test.png";
			var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
			var filePath = System.IO.Path.Combine(sdCardPath, tempFilename);
			using (var os = new FileStream(filePath, FileMode.Create))
			{
				bitmap.Compress(Bitmap.CompressFormat.Png, 100, os);
			}
			bitmap.Dispose();

			var imageUri = Android.Net.Uri.Parse($"file://{sdCardPath}/{tempFilename}");
			var sharingIntent = new Intent();
			sharingIntent.SetAction(Intent.ActionSend);
			sharingIntent.SetType("image/*");

			var dropContent = string.Format("Drop Name:\n" + ItemModel.Name + "\n\n" +
											"Drop Description:\n" + ItemModel.Description + "\n\n" +
											"Drop Location:\n http://maps.google.com/?ll={0},{1}", ItemModel.Location_Lat, ItemModel.Location_Lnt);
			
			sharingIntent.PutExtra(Intent.ExtraText, dropContent);
			sharingIntent.PutExtra(Intent.ExtraStream, imageUri);
			sharingIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
			StartActivity(Intent.CreateChooser(sharingIntent, "Share your drop."));
		}

		void ActionBack(object sender, EventArgs e)
		{
			base.OnBackPressed();
			OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
		}

		#endregion

		void CollepseAnimation(RelativeLayout content)
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

		private ValueAnimator slideAnimator(int start, int end, RelativeLayout content)
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
	}
}
