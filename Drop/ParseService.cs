using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Parse;

namespace Drop
{
	public static class ParseService
	{
		static ParseService()
		{
			ParseClient.Initialize(new ParseClient.Configuration
			{
				ApplicationId = Constants.PARSE_APP_ID,
				WindowsKey = Constants.PARSE_NET_KEY,
				Server = Constants.PARSE_SERVER_URL,
			});
			ParseFacebookUtils.Initialize(Constants.FACEBOOK_APP_ID);
		}

		public static async Task<string> SignUp(User user)
		{
			var userObject = new ParseUser()
			{
				Username = user.Email,
				Email = user.Email,
				Password = user.Password
			};

			userObject["Firstname"] = user.Firstname;
			userObject["Lastname"] = user.Lastname;
			userObject["PhotoURL"] = user.PhotoURL;

			try
			{
				await userObject.SignUpAsync();
				return Constants.STR_STATUS_SUCCESS;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
				var response = await Login(user);
				return response;
			}
		}

		public static async Task<string> Login(User user)
		{
			try
			{
				var parseUser = await ParseUser.LogInAsync(user.Email, user.Password);
				return Constants.STR_STATUS_SUCCESS;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public static async Task<string> FacebookSignUp(string fbUserID, string accessToken, DateTime expiration)
		{
			try
			{
				ParseUser user = await ParseFacebookUtils.LogInAsync(fbUserID, accessToken, expiration);

				if (user == null)
					return Constants.STR_STATUS_FAIL;
				else
					return Constants.STR_STATUS_SUCCESS;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

		public static void Logout()
		{
			ParseUser.LogOut();
		}

		public static async Task<string> AddDropItem(ParseItem item)
		{
			try
			{
				var dropItem = new ParseObject(Constants.STR_TABLE_DROP_ITEM);

				dropItem[Constants.STR_FIELD_USERID] = ParseUser.CurrentUser == null ? Constants.STR_UNKNOWN_USER : ParseUser.CurrentUser.Username;
				dropItem[Constants.STR_FIELD_NAME] = item.Name;
				dropItem[Constants.STR_FIELD_DESCRIPTION] = item.Description;
				dropItem[Constants.STR_FIELD_TEXT] = item.Text;
				dropItem[Constants.STR_FIELD_IMAGE] = item.Image == null ? null : new ParseFile(item.Image.fileName, item.Image.fileData);
				dropItem[Constants.STR_FIELD_VIDEO] = item.Video == null ? null : new ParseFile(item.Video.fileName, item.Video.fileData);
				dropItem[Constants.STR_FIELD_OTHER] = item.Other;
				dropItem[Constants.STR_FIELD_ICON] = item.Icon == null ? null : new ParseFile(item.Icon.fileName, item.Icon.fileData);
				dropItem[Constants.STR_FIELD_LOCATION_LNT] = item.Location_Lnt;
				dropItem[Constants.STR_FIELD_LOCATION_LAT] = item.Location_Lat;
				dropItem[Constants.STR_FIELD_VISIBILITY] = item.Visibility;
				dropItem[Constants.STR_FIELD_PASSWORD] = item.Password;
				dropItem[Constants.STR_FIELD_EXPIRY] = DateTime.Parse(item.ExpiryDate);

				await dropItem.SaveAsync();
			}
			catch (Exception e)
			{
				return e.Message;
			}

			return Constants.STR_STATUS_SUCCESS;
		}

		public static IList<ParseItem> GetDropItems()
		{
			List<ParseItem> results = new List<ParseItem>();
			var query = ParseObject.GetQuery(Constants.STR_TABLE_DROP_ITEM).OrderBy("Name");
			IEnumerable<ParseObject> drops = query.FindAsync().GetAwaiter().GetResult();
			foreach (ParseObject drop in drops)
			{
				var dropItem = new ParseItem();

				dropItem.Username = drop.Get<string>(Constants.STR_FIELD_USERID);
				dropItem.Name = drop.Get<string>(Constants.STR_FIELD_NAME);
				dropItem.Text = drop.Get<string>(Constants.STR_FIELD_TEXT);
				dropItem.Location_Lnt = drop.Get<double>(Constants.STR_FIELD_LOCATION_LNT);
				dropItem.Location_Lat = drop.Get<double>(Constants.STR_FIELD_LOCATION_LAT);
				dropItem.Visibility = drop.Get<int>(Constants.STR_FIELD_VISIBILITY);
				dropItem.Password = drop.Get<string>(Constants.STR_FIELD_PASSWORD);

				ParseFile imageObject = drop.Get<ParseFile>(Constants.STR_FIELD_IMAGE);
				ParseFile videoObject = drop.Get<ParseFile>(Constants.STR_FIELD_VIDEO);
				ParseFile iconObject = drop.Get<ParseFile>(Constants.STR_FIELD_ICON);

				dropItem.ImageURL = imageObject != null ? drop.Get<ParseFile>(Constants.STR_FIELD_IMAGE).Url : null;
				dropItem.VideoURL = videoObject != null ? drop.Get<ParseFile>(Constants.STR_FIELD_VIDEO).Url : null;
				dropItem.IconURL = iconObject != null ? drop.Get<ParseFile>(Constants.STR_FIELD_ICON).Url : null;

				if (IsVisibility(dropItem))
				{
					results.Add(dropItem);
				}
			}
			return results;
		}

		public static bool IsVisibility(ParseItem drop)
		{
			bool isVisible = false;
			switch (drop.Visibility)
			{
				case Constants.TAG_VISIBLE_EVERY:
					isVisible = true;
					break;
				case Constants.TAG_VISIBLE_SPECIFIC:
					isVisible = ParseUser.CurrentUser == null ? false : true;
					break;
				case Constants.TAG_VISIBLE_ME:
					isVisible = ParseUser.CurrentUser != null && ParseUser.CurrentUser.Username == drop.Username ? true : false;
					break;
				default:
					isVisible = false;
					break;
			}

			return isVisible;
		}
		//static ParseObject GetTaskObject(int id)
		//{
		//	var query = from taskQuery in ParseObject.GetQuery("Task")
		//				where taskQuery.Get<int>("ID") == id
		//				select taskQuery;
		//	return query.FirstOrDefaultAsync().GetAwaiter().GetResult();
		//}

		//public static TodoItem GetTask(int id)
		//{
		//	ParseObject task = GetTaskObject(id);
		//	if (task != null)
		//	{
		//		return new TodoItem
		//		{
		//			ID = task.Get<int>("ID"),
		//			Name = task.Get<string>("Name"),
		//			Notes = task.Get<string>("Notes"),
		//			Done = task.Get<bool>("Done")
		//		};
		//	}
		//	else {
		//		return null;
		//	}
		//}



		//static int SaveObject(ParseObject task, TodoItem item)
		//{
		//	task["Name"] = item.Name;
		//	task["Notes"] = item.Notes;
		//	task["Done"] = item.Done;
		//	task.SaveAsync().GetAwaiter().GetResult();
		//	return 1;
		//}

		//public static int SaveTask(TodoItem item)
		//{
		//	if (item.ID <= 0)
		//	{
		//		ParseObject task = new ParseObject("Task");
		//		task["ID"] = (new Random()).Next(1, int.MaxValue);
		//		return SaveObject(task, item);
		//	}
		//	else {
		//		ParseObject task = GetTaskObject(item.ID);
		//		if (task != null)
		//		{
		//			return SaveObject(task, item);
		//		}
		//		else {
		//			return 0;
		//		}
		//	}
		//}

		//public static int DeleteTask(int id)
		//{
		//	ParseObject task = GetTaskObject(id);
		//	if (task != null)
		//	{
		//		task.DeleteAsync().GetAwaiter().GetResult();
		//		return 1;
		//	}
		//	else {
		//		return 0;
		//	}
		//}
	}
}
