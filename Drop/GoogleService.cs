using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Drop
{
	public class GoogleService
	{
		public static async Task<PlaceDetailsAPI_RootObject> GetPlaceDetails(String placeid)
		{
			PlaceDetailsAPI_RootObject ro = null;
			try
			{
				using (var httpClient = new HttpClient())
				{
					string url = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeid + "&key=" + Constants.GOOGLE_MAP_API_KEY;
					var json = await httpClient.GetStringAsync(url);
					ro = JsonConvert.DeserializeObject<PlaceDetailsAPI_RootObject>(json);
				}
			}
			catch (Exception e)
			{
				//TODO: better logging
				Debug.WriteLine("GetPlaceDetails failed: " + e.Message);
			}
			return ro;
		}
	}
}
