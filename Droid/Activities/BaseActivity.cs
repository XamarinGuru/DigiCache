
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using AndroidHUD;

namespace Drop.Droid
{
	[Activity(Label = "BaseActivity")]
	public class BaseActivity : FragmentActivity, ISurfaceHolderCallback
	{
		string[] PermissionsCamera =
		{
			Manifest.Permission.Camera
		};
		const int RequestCameraId = 0;

		Android.Hardware.Camera camera;
		SurfaceView surfaceView;
		ISurfaceHolder surfaceHolder;

		AlertDialog.Builder alert;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.LiveCameraLayout);

			surfaceView = FindViewById<SurfaceView>(Resource.Id.liveCamera);

			CheckCameraPermission();
		}

		#region grant Camera access permission
		private void CheckCameraPermission()
		{
			if ((int)Build.VERSION.SdkInt < 23)
			{
				StartLiveCamera();
			}
			else {
				RequestCameraPermission();

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
				AlertDialog.Builder alert = new AlertDialog.Builder(this);
				alert.SetTitle("");
				alert.SetMessage("Camera access is required to show live camera.");
				alert.SetPositiveButton("Cancel", (senderAlert, args) =>
				{
				});
				alert.SetNegativeButton("OK", (senderAlert, args) =>
				{
					ActivityCompat.RequestPermissions(this, PermissionsCamera, RequestCameraId);
				});
				RunOnUiThread(() =>
				{
					alert.Show();
				});

				return;
			}

			ActivityCompat.RequestPermissions(this, PermissionsCamera, RequestCameraId);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			switch (requestCode)
			{
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

		void StartLiveCamera()
		{
			Window.SetFormat(Android.Graphics.Format.Unknown);
			//surfaceView = FindViewById<SurfaceView>(Resource.Id.liveCamera);
			surfaceHolder = surfaceView.Holder;
			surfaceHolder.AddCallback(this);
			surfaceHolder.SetType(SurfaceType.PushBuffers);
		}
		#endregion

		#region ISurfaceHolderCallback

		public async void SurfaceCreated(ISurfaceHolder holder)
		{
			try
			{
				var cameraId = FindBackCamera();
				camera = Android.Hardware.Camera.Open(cameraId);
				AddContentView(new View(this), new ActionBar.LayoutParams(ActionBar.LayoutParams.MatchParent, ActionBar.LayoutParams.MatchParent));
			}
			catch (Exception ex)
			{

			}
		}
		public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Android.Graphics.Format format, int width, int height)
		{
			if (camera != null)
			{
				try
				{
					camera.SetPreviewDisplay(surfaceHolder);

					var display = this.WindowManager.DefaultDisplay;
					if (display.Rotation == SurfaceOrientation.Rotation0)
						camera.SetDisplayOrientation(90);
					else
						camera.SetDisplayOrientation(180);
					camera.StartPreview();
				}
				catch (Exception ex)
				{
					Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
				}
			}
		}
		public void SurfaceDestroyed(ISurfaceHolder holder)
		{
			if (camera != null)
			{
				camera.Release();
			}

			camera = null;
		}
		#endregion

		int FindBackCamera()
		{
			int cameraId = -1;
			for (int i = 0; i < Android.Hardware.Camera.NumberOfCameras; i++)
			{
				Android.Hardware.Camera.CameraInfo info = new Android.Hardware.Camera.CameraInfo();
				Android.Hardware.Camera.GetCameraInfo(i, info);
				if (info.Facing == Android.Hardware.Camera.CameraInfo.CameraFacingBack)
				{
					cameraId = i;
					break;
				}
			}
			return cameraId;
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
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
	}
}
