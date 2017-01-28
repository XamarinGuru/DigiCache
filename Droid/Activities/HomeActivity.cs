
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Drop.Droid
{
	[Activity(Label = "HomeActivity")]
	public class HomeActivity : BaseActivity
	{
		ImageView aniBgView;
		LinearLayout aniContentView;

		int nInitialAniviewHeight;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.HomeActivity);

			SetUIVariablesAndActions();
		}

		private void SetUIVariablesAndActions()
		{
			#region UI Variables
			aniBgView = FindViewById<ImageView>(Resource.Id.aniBgView);
			aniContentView = FindViewById<LinearLayout>(Resource.Id.aniContentView);
			#endregion

			nInitialAniviewHeight = aniBgView.Height;

			#region Actions
			FindViewById<ImageView>(Resource.Id.ActionHome).Click += ActionHome;
			FindViewById<LinearLayout>(Resource.Id.ActionDropItem).Click += ActionDrop;
			FindViewById<LinearLayout>(Resource.Id.ActionDropNearby).Click += ActionDrop;
			FindViewById<LinearLayout>(Resource.Id.ActionDropSetting).Click += ActionDrop;
			#endregion
		}

		void ActionHome(object sender, EventArgs e)
		{
			HomeButtonAnimation(aniBgView);
			HomeButtonAnimation(aniContentView);
		}

		void ActionDrop(object sender, EventArgs e)
		{
			Intent dropAC = new Intent(); 
			switch (int.Parse(((LinearLayout)sender).Tag.ToString()))
			{
				case Constants.TAG_DROP_ITEM:
					dropAC = new Intent(this, typeof(DropItemActivity));
					break;
				case Constants.TAG_DROP_NEARBY:
					dropAC = new Intent(this, typeof(NearbyActivity));
					break;
				case Constants.TAG_DROP_SETTING:
					dropAC = new Intent(this, typeof(NearbyActivity));
					break;
				default:
					break;
			}

			StartActivity(dropAC);
		}

		void HomeButtonAnimation(View aniView)
		{
			if (aniView.Visibility.Equals(ViewStates.Gone))
			{
				aniView.Visibility = ViewStates.Visible;

				ValueAnimator mAnimator = slideAnimator(0, nInitialAniviewHeight, aniView);
				mAnimator.Start();
			}
			else {
				nInitialAniviewHeight = aniView.Height;

				ValueAnimator mAnimator = slideAnimator(nInitialAniviewHeight, 0, aniView);
				mAnimator.Start();
				mAnimator.AnimationEnd += (object IntentSender, EventArgs arg) =>
				{
					aniView.Visibility = ViewStates.Gone;
				};
			}
		}

		private ValueAnimator slideAnimator(int start, int end, View content)
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
