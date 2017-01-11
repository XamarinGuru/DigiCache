using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentValidation;

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

		public string Name { get { return parseItem.Name; } set { parseItem.Name = value; } }
		public string Description { get { return parseItem.Description; } set { parseItem.Description = value; } }
		public string Text { get { return parseItem.Text; } set { parseItem.Text = value; } }
		public byte[] Image { get { return parseItem.Image; } set { parseItem.Image = value; } }
		public byte[] Video { get { return parseItem.Video; } set { parseItem.Video = value; } }
		public string Other { get { return parseItem.Other; } set { parseItem.Other = value; } }
		public byte[] Icon { get { return parseItem.Icon; } set { parseItem.Icon = value; } }
		public string Permission { get { return parseItem.Permission; } set { parseItem.Permission = value; } }
		public string Password { get { return parseItem.Password; } set { parseItem.Password = value; } }
		public string ExpiryDate { get { return parseItem.ExpiryDate; } set { parseItem.ExpiryDate = value; } }
	}

	public class ParseItem
	{
		public string Name { get; internal set; }
		public string Description { get; internal set; }
		public string Text { get; internal set; }
		public byte[] Image { get; internal set; }
		public byte[] Video { get; internal set; }
		public string Other { get; internal set; }
		public byte[] Icon { get; internal set; }
		public string Permission { get; internal set; }
		public string Password { get; internal set; }
		public string ExpiryDate { get; internal set; }
	}
}
