
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using AndroidHUD;

namespace Drop.Droid
{
	[Activity(Label = "BaseActivity")]
	public class BaseActivity : Activity
	{
		AlertDialog.Builder alert;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			Window.RequestFeature(WindowFeatures.NoTitle);
			base.OnCreate(savedInstanceState);
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

		protected MediaFile ByteDataFromImage(int resourceId)
		{
			var bitmap = BitmapFactory.DecodeResource(Resources, resourceId);
			Bitmap newBitmap = scaleDown(bitmap, 50, true);

			var stream = new MemoryStream();

			newBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
			var fileBytes = stream.ToArray();

			var fileName = "Image_" + DateTime.Now.ToString("dd-mm-yy_hh-mm-ss") + ".png";

			return new MediaFile(fileName, fileBytes);
		}

		//public override void OnActivityResult(int requestCode, int resultCode, Intent data)
		//{
		//	base.OnActivityResult(requestCode, resultCode, data);
		//	try
		//	{
		//		if (resultCode == (int)Result.Ok)
		//		{
		//			Bitmap mewbm = NGetBitmap(data.Data);

		//			Bitmap newBitmap = scaleDown(mewbm, 200, true);
		//			using (var stream = new MemoryStream())
		//			{
		//				newBitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
		//				bitmapByteData = stream.ToArray();
		//			}
		//			ExportBitmapAsPNG(GetRoundedCornerBitmap(newBitmap, 400));
		//		}
		//	}
		//	catch (Exception err)
		//	{
		//		Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
		//	}
		//}

		//void ExportBitmapAsPNG(Bitmap bitmap)
		//{
		//	try
		//	{
		//		var sdCardPath = Android.OS.Environment.DataDirectory.AbsolutePath;
		//		var filePath = System.IO.Path.Combine(sdCardPath, "data/goheja.gohejanitro/files/me.png");
		//		var stream = new FileStream(filePath, FileMode.Create);

		//		bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);// Bitmap.CompressFormat.Png, 100, stream);
		//		stream.Close();
		//		var s2 = new FileStream(filePath, FileMode.Open);

		//		Bitmap bitmap2 = BitmapFactory.DecodeFile(filePath);
		//		imgProfile.SetImageBitmap(bitmap2);
		//		s2.Close();
		//		GC.Collect();
		//	}
		//	catch (Exception err)
		//	{
		//		Toast.MakeText(Activity, err.ToString(), ToastLength.Long).Show();
		//	}
		//}
		//private Bitmap NGetBitmap(Android.Net.Uri uriImage)
		//{
		//	Bitmap mBitmap = null;
		//	mBitmap = MediaStore.Images.Media.GetBitmap(Activity.ContentResolver, uriImage);
		//	return mBitmap;
		//}
		//private static Bitmap GetRoundedCornerBitmap(Bitmap bitmap, int pixels)
		//{
		//	Bitmap output = null;

		//	try
		//	{
		//		output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
		//		Canvas canvas = new Canvas(output);

		//		Color color = new Color(66, 66, 66);
		//		Paint paint = new Paint();
		//		Rect rect = new Rect(0, 0, bitmap.Width * 5 / 5, bitmap.Height * 5 / 5);
		//		RectF rectF = new RectF(rect);
		//		float roundPx = pixels;

		//		paint.AntiAlias = true;
		//		canvas.DrawARGB(0, 0, 0, 0);
		//		paint.Color = color;
		//		canvas.DrawRoundRect(rectF, roundPx, roundPx, paint);

		//		paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
		//		canvas.DrawBitmap(bitmap, rect, rect, paint);
		//	}
		//	catch
		//	{
		//		return null;
		//	}

		//	return output;
		//}

		public static Bitmap scaleDown(Bitmap realImage, float maxImageSize, bool filter)
		{
			float ratio = Math.Min((float)maxImageSize / realImage.Width, (float)maxImageSize / realImage.Width);
			int width = (int)Math.Round((float)ratio * realImage.Width);
			int height = (int)Math.Round((float)ratio * realImage.Width);

			Bitmap newBitmap = Bitmap.CreateScaledBitmap(realImage, width, height, filter);
			return newBitmap;
		}
	}
}
