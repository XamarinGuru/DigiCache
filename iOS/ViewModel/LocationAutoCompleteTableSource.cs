using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using System.Threading.Tasks;
using System.Linq;

namespace Drop.iOS
{
	public class LocationAutoCompleteTableSource : UITableViewSource
	{
		public bool mIsPickUpLocation;
		const string strCellIdentifier = "Cell";
		readonly List<Prediction> lstLocations;
		internal event Action<Prediction> LocationRowSelectedEventAction;

		DropLocationViewController rootVC;
		Action<double, double> mCallback;
		UIView bgView;

		public LocationAutoCompleteTableSource(DropLocationViewController rootVC, List<Prediction> arrItems, UIView background, Action<double, double> callback)
		{
			this.rootVC = rootVC;
			lstLocations = arrItems;
			mCallback = callback;
			bgView = background;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return lstLocations.Count;
		}

		public override UIView GetViewForFooter(UITableView tableView, nint section)
		{
			return new UIView();
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 40.0f;
		}

		public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell(strCellIdentifier) ?? new UITableViewCell(UITableViewCellStyle.Default, strCellIdentifier);
			cell.TextLabel.Text = lstLocations[indexPath.Row].description;
			cell.TextLabel.Font = UIFont.SystemFontOfSize(12);
			cell.BackgroundColor = UIColor.Clear;
			return cell;
		}
		public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			tableView.Hidden = true;
			bgView.Hidden = true;

			if (LocationRowSelectedEventAction != null)
			{
				LocationRowSelectedEventAction(lstLocations[indexPath.Row]);
				var locationData = lstLocations[indexPath.Row];

				rootVC.ShowLoadingView("Getting location details ...");

				Task runSync = Task.Factory.StartNew(async (object inputObj) =>
				{
					var placeId = inputObj != null ? inputObj.ToString() : "";

					if (!String.IsNullOrEmpty(placeId))
					{
						var data = await GoogleService.GetPlaceDetails(placeId);

						rootVC.HideLoadingView();

						rootVC.ItemModel.Location_Lat = data.result.geometry.location.lat;
						rootVC.ItemModel.Location_Lnt = data.result.geometry.location.lng;
						mCallback(data.result.geometry.location.lat, data.result.geometry.location.lng);
					}
				}, locationData.place_id).Unwrap();
			}
			tableView.DeselectRow(indexPath, true);

		}
	}
}
