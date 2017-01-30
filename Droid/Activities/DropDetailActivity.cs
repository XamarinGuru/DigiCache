
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Drop.Droid
{
	[Activity(Label = "DropDetailActivity")]
	public class DropDetailActivity : BaseActivity
	{
		public ParseItem parseItem;

		private ItemModel ItemModel { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.DropDetailLayout);

			parseItem = Global.selectedDrop;

			var imgImage = FindViewById<ImageView>(Resource.Id.imgImage);
			if (parseItem.ImageURL != null)
			{
				var imageBitmap = GetImageBitmapFromUrl(parseItem.ImageURL.ToString());
				imgImage.SetImageBitmap(imageBitmap);
			}

			FindViewById<TextView>(Resource.Id.lblName).Text = parseItem.Name;
			FindViewById<TextView>(Resource.Id.lblText).Text = parseItem.Text;

			FindViewById(Resource.Id.ActionPlayVideo).Click += ActionPlayVideo;
			FindViewById(Resource.Id.ActionGotoLink).Click += ActionGotoLink;

			FindViewById(Resource.Id.ActionBack).Click += delegate
			{
				base.OnBackPressed();
				OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
			};
		}

		void ActionPlayVideo(object sender, EventArgs e)
		{
			if (parseItem.VideoURL != null)
			{
				var uri = Android.Net.Uri.Parse(parseItem.VideoURL.ToString());
				var intent = new Intent(Intent.ActionView, uri);
				StartActivity(intent);
			}
			//if (parseItem.VideoURL != null)
			//{
			//	var nextActivity = new Intent(this, typeof(PlayVideoActivity));
			//	StartActivity(nextActivity);
			//	OverridePendingTransition(Resource.Animation.fromLeft, Resource.Animation.toRight);
			//}
		}
		void ActionGotoLink(object sender, EventArgs e)
		{
			if (parseItem.OtherLink != null && parseItem.OtherLink != "")
			{
				var uri = Android.Net.Uri.Parse(parseItem.OtherLink);
				var intent = new Intent(Intent.ActionView, uri);
				StartActivity(intent);
			}
		}
	}
}
