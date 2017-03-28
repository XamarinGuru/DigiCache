
using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using UniversalImageLoader.Core;

namespace Drop.Droid
{
	class FavoriteAdapter : BaseAdapter
	{
		FavoriteActivity _rootVC;
		List<ParseItem> _drops;

		Action<ParseItem> _removeCallback;
		Action<ParseItem> _mapCallback;

		public FavoriteAdapter(Context context, List<ParseItem> drops, Action<ParseItem> removeCallback, Action<ParseItem> mapCallback)
		{
			_rootVC = context as FavoriteActivity;
			_drops = drops;
			_removeCallback = removeCallback;
			_mapCallback = mapCallback;
		}

		public override int Count
		{
			get
			{
				return _drops.Count;
			}
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return null;
		}

		public ParseItem GetDrop(int position)
		{
			return _drops[position];
		}

		override public long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			convertView = LayoutInflater.From(_rootVC).Inflate(Resource.Layout.ItemFavorite, null);

			var drop = GetDrop(position);

			var imgIcon = convertView.FindViewById<ImageView>(Resource.Id.imgIcon);
			ImageLoader imageLoader = ImageLoader.Instance;
			imageLoader.DisplayImage(drop.IconURL.ToString(), imgIcon);

			convertView.FindViewById<TextView>(Resource.Id.lblName).Text = drop.Name;
			convertView.FindViewById<TextView>(Resource.Id.lblLat).Text = "LAT: " + drop.Location_Lat.ToString("F6");
			convertView.FindViewById<TextView>(Resource.Id.lblLong).Text = "LONG: " + drop.Location_Lnt.ToString("F6");

			var btnClose = convertView.FindViewById(Resource.Id.ActionClose);
			var btnMap = convertView.FindViewById(Resource.Id.ActionMap);
			btnClose.Click += ActionClose;
			btnMap.Click += ActionMap;
			btnClose.Tag = position;
			btnMap.Tag = position;

			return convertView;
		}

		void ActionMap(object sender, EventArgs e)
		{
			var drop = _drops[(int)((LinearLayout)sender).Tag];
			_mapCallback(drop);
		}

		void ActionClose(object sender, EventArgs e)
		{
			var drop = _drops[(int)((ImageView)sender).Tag];
			_removeCallback(drop);
		}
	}
}
