using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using Parse;

namespace Drop.iOS
{
	public partial class FavoriteViewController : BaseViewController
    {
		List<ParseItem> mfDrops = new List<ParseItem>();

		public FavoriteViewController(IntPtr handle) : base(handle, Constants.STR_iOS_VCNAME_FAVORITE)
		{
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			GetFavoriteDrops();
		}

		void GetFavoriteDrops()
		{
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

				HideLoadingView();

				var tblDataSource = new FavoritesTableViewSource(mfDrops, this);
				this.InvokeOnMainThread(delegate
				{
					TableView.Source = tblDataSource;
					TableView.ReloadData();
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
			UIViewController pvc = GetVCWithIdentifier(Constants.STR_iOS_VCNAME_NEARBY);
			NavigationController.PushViewController(pvc, true);
		}

		#region FavoritesTableViewSource

		class FavoritesTableViewSource : UITableViewSource
		{
			List<ParseItem> favorites;
			FavoriteViewController favoriteVC;

			public FavoritesTableViewSource(List<ParseItem> favorites, FavoriteViewController favoriteVC)
			{
				this.favorites = new List<ParseItem>();

				if (favorites == null) return;

				this.favorites = favorites;
				this.favoriteVC = favoriteVC;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 100.0f;
			}

			public override nint RowsInSection(UITableView tableView, nint section)
			{
				return favorites.Count;
			}
			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				FavoriteCell cell = tableView.DequeueReusableCell("FavoriteCell") as FavoriteCell;
				cell.SetCell(favorites[indexPath.Row], favoriteVC.RemoveCallBack, favoriteVC.MapCallBack);

				return cell;
			}
		}
		#endregion
	}
}