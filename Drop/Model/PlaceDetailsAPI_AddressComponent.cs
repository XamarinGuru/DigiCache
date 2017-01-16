using System;
using System.Collections.Generic;

namespace Drop
{
	public class PlaceDetailsAPI_AddressComponent
	{
		public string long_name { get; set; }
		public string short_name { get; set; }
		public List<string> types { get; set; }
	}

	public class PlaceDetailsAPI_Location
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class PlaceDetailsAPI_Geometry
	{
		public PlaceDetailsAPI_Location location { get; set; }
	}

	public class PlaceDetailsAPI_Photo
	{
		public int height { get; set; }
		public List<string> html_attributions { get; set; }
		public string photo_reference { get; set; }
		public int width { get; set; }
	}

	public class PlaceDetailsAPI_Aspect
	{
		public int rating { get; set; }
		public string type { get; set; }
	}

	public class PlaceDetailsAPI_Review
	{
		public List<PlaceDetailsAPI_Aspect> aspects { get; set; }
		public string author_name { get; set; }
		public string author_url { get; set; }
		public string language { get; set; }
		public int rating { get; set; }
		public string text { get; set; }
		public int time { get; set; }
	}

	public class PlaceDetailsAPI_Result
	{
		public List<PlaceDetailsAPI_AddressComponent> address_components { get; set; }
		public string adr_address { get; set; }
		public string formatted_address { get; set; }
		public string formatted_phone_number { get; set; }
		public PlaceDetailsAPI_Geometry geometry { get; set; }
		public string icon { get; set; }
		public string id { get; set; }
		public string international_phone_number { get; set; }
		public string name { get; set; }
		public List<PlaceDetailsAPI_Photo> photos { get; set; }
		public string place_id { get; set; }
		public double rating { get; set; }
		public string reference { get; set; }
		public List<PlaceDetailsAPI_Review> reviews { get; set; }
		public string scope { get; set; }
		public List<string> types { get; set; }
		public string url { get; set; }
		public int user_ratings_total { get; set; }
		public int utc_offset { get; set; }
		public string vicinity { get; set; }
		public string website { get; set; }
	}

	public class PlaceDetailsAPI_RootObject
	{
		public List<object> html_attributions { get; set; }
		public PlaceDetailsAPI_Result result { get; set; }
		public string status { get; set; }
	}
}
