
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Parse;

namespace Drop.Droid
{
	[Activity(Label = "FavoriteActivity")]
	public class FavoriteActivity : BaseActivity
	{
		List<ParseItem> mfDrops = new List<ParseItem>();

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.FavoriteLayout);

			FindViewById(Resource.Id.ActionBack).Click += delegate
			{
				base.OnBackPressed();
				OverridePendingTransition(Resource.Animation.fromRight, Resource.Animation.toLeft);
			};

			GetFavoriteDrops();
		}

		void GetFavoriteDrops()
		{
			var listView = FindViewById<ListView>(Resource.Id.listView);

			System.Threading.ThreadPool.QueueUserWorkItem(delegate
			{
				ShowLoadingView(Constants.STR_F_DROPS_LOADING);

				mfDrops = new List<ParseItem>();

				var drops = ParseService.GetDropItems();

				foreach (var drop in drops)
				{
					ItemModel ItemModel = new ItemModel();
					ItemModel.parseItem = drop;
					var favoriteList = ItemModel.Favorite;
					foreach (var favoriteID in favoriteList)
					{
						if (favoriteID.Equals(ParseUser.CurrentUser.ObjectId))
							mfDrops.Add(drop);
					}
				}

				var adapter = new FavoriteAdapter(this, mfDrops, RemoveCallBack, MapCallBack);
				RunOnUiThread(() =>
				{
					listView.Adapter = adapter;
					adapter.NotifyDataSetChanged();
					HideLoadingView();
				});
			});
		}

		async void RemoveCallBack(ParseItem drop)
		{
			ItemModel ItemModel = new ItemModel();
			ItemModel.parseItem = drop;
			var favoriteList = ItemModel.Favorite;

			for (int i = 0; i < favoriteList.Count; i++)
			{
				if (favoriteList[i].Equals(ParseUser.CurrentUser.ObjectId))
					favoriteList.RemoveAt(i);
			}
			ItemModel.Favorite = favoriteList;

			ShowLoadingView(Constants.STR_LOADING);
			var result = await ParseService.UpdateFavorite(ItemModel.parseItem);
			HideLoadingView();

			GetFavoriteDrops();
		}

		void MapCallBack(ParseItem drop)
		{
			Intent dropAC = new Intent(this, typeof(NearbyActivity));
			StartActivity(dropAC);
		}
	}
}
