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
				dropItem[Constants.STR_FIELD_EXPIRY] = DateTime.Now;

				await dropItem.SaveAsync();
			}
			catch (Exception e)
			{
				return e.Message;
			}

			return Constants.STR_STATUS_SUCCESS;
		}

		static ParseObject GetTaskObject(int id)
		{
			var query = from taskQuery in ParseObject.GetQuery("Task")
						where taskQuery.Get<int>("ID") == id
						select taskQuery;
			return query.FirstOrDefaultAsync().GetAwaiter().GetResult();
		}

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

		//public static IList<TodoItem> GetTasks()
		//{
		//	List<TodoItem> results = new List<TodoItem>();
		//	var query = ParseObject.GetQuery("Task").OrderBy("Name");
		//	IEnumerable<ParseObject> tasks = query.FindAsync().GetAwaiter().GetResult();
		//	foreach (ParseObject task in tasks)
		//	{
		//		results.Add(new TodoItem
		//		{
		//			ID = task.Get<int>("ID"),
		//			Name = task.Get<string>("Name"),
		//			Notes = task.Get<string>("Notes"),
		//			Done = task.Get<bool>("Done")
		//		});
		//	}
		//	return results;
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
