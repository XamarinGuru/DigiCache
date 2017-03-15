using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentValidation;
using Newtonsoft.Json;
using Parse;

namespace Drop
{
	public class RootObject : AbstractValidator<ParseItem>, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public class ItemModel : RootObject
	{
		public ItemModel()
		{
			parseItem = new ParseItem();
		}
		public ParseItem parseItem { get; set; }

		public string ID { get { return parseItem.ID; } set { parseItem.ID = value; } }
		public string Username { get { return parseItem.Username; } set { parseItem.Username = value; } }
		public string Name { get { return parseItem.Name; } set { parseItem.Name = value; } }
		public string Description { get { return parseItem.Description; } set { parseItem.Description = value; } }
		public string Text { get { return parseItem.Text; } set { parseItem.Text = value; } }
		public string OtherLink { get { return parseItem.OtherLink; } set { parseItem.OtherLink = value; } }
		public double Location_Lnt { get { return parseItem.Location_Lnt; } set { parseItem.Location_Lnt = value; } }
		public double Location_Lat { get { return parseItem.Location_Lat; } set { parseItem.Location_Lat = value; } }
		public int Visibility { get { return parseItem.Visibility; } set { parseItem.Visibility = value; } }
		public int Modify { get { return parseItem.Modify; } set { parseItem.Modify = value; } }
		public string Password { get { return parseItem.Password; } set { parseItem.Password = value; } }
		public string ExpiryDate { get { return parseItem.ExpiryDate; } set { parseItem.ExpiryDate = value; } }
		public List<string> Favorite
		{
			get
			{
				try
				{
					return parseItem.Favorite == "" ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(parseItem.Favorite);
				}
				catch
				{
					return new List<string>();
				}
			}
			set
			{
				parseItem.Favorite = JsonConvert.SerializeObject(value);
			}
		}
		public MediaFile Image { get { return parseItem.Image; } set { parseItem.Image = value; } }
		public MediaFile Video { get { return parseItem.Video; } set { parseItem.Video = value; } }
		public MediaFile Icon { get { return parseItem.Icon; } set { parseItem.Icon = value; } }

		public bool IsValidDrop()
		{
			if (Name == null || Name == "" || Icon == null || (Location_Lnt == 0 && Location_Lat == 0) || ExpiryDate == "" || ExpiryDate == null)
				return false;
			//if (OtherLink != null || OtherLink != "" || !Uri.IsWellFormedUriString(OtherLink, UriKind.RelativeOrAbsolute))
			//	return false;

			return true;
		}
	}

	public class MediaFile
	{
		public MediaFile(string fileName, byte[] fileByte)
		{
			this.fileName = fileName;
			this.fileData = fileByte;
		}
		public string fileName { get; internal set; }
		public byte[] fileData { get; internal set; }
	}

	public class ParseItem
	{
		public ParseObject _pObject { get; internal set; }
		public string ID { get; internal set; }
		public string Username { get; internal set; }
		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public string Text { get; internal set; }
		public string OtherLink { get; internal set; }
		public double Location_Lnt { get; internal set; }
		public double Location_Lat { get; internal set; }
		public int Visibility { get; internal set; }
		public int Modify { get; internal set; }
		public string Password { get; internal set; }
		public string ExpiryDate { get; internal set; }
		public string Favorite { get; internal set; }

		public MediaFile Image { get; internal set; }
		public MediaFile Video { get; internal set; }
		public MediaFile Icon { get; internal set; }
		public Uri ImageURL { get; internal set; }
		public Uri VideoURL { get; internal set; }
		public Uri IconURL { get; internal set; }
	}
}
