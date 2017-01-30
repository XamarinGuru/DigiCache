
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
	[Activity(Label = "PlayVideoActivity")]
	public class PlayVideoActivity : BaseActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.PlayVideoLayout);

			FindViewById(Resource.Id.ActionBack).Click += delegate
			{
				base.OnBackPressed();
				OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
			};

			var videoView = FindViewById<VideoView>(Resource.Id.videoView);

			var uri = Android.Net.Uri.Parse(Global.selectedDrop.VideoURL.ToString());

			videoView.SetVideoURI(uri);
			videoView.RequestFocus();
			videoView.SeekTo(200);
			//videoView.SetMediaController(new MediaController(this));
			videoView.Start();
		}
	}
}
