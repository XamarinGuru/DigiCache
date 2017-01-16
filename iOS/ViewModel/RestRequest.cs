using System;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace Drop.iOS
{
	public class RestRequestClass
	{
		static async Task<string> CallService(string strURL)
		{
			WebClient client = new WebClient();
			string strResult;
			try
			{
				strResult = await client.DownloadStringTaskAsync(new Uri(strURL));
			}
			catch
			{
				strResult = "Exception";
			}
			finally
			{
				client.Dispose();
				client = null;
			}
			return strResult;
		}

		internal static async Task<LocationPrediction> LocationAutoComplete(string strFullURL)
		{
			LocationPrediction objLocationPredictClass = null;
			string strResult = await CallService(strFullURL);
			if (strResult != "Exception")
			{
				objLocationPredictClass = JsonConvert.DeserializeObject<LocationPrediction>(strResult);
			}
			return objLocationPredictClass;
		}
	}
}
