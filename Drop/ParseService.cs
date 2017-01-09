using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;
using UIKit;

using Facebook;

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
				Password = user.Password,
				Email = user.Email
			};

			try
			{
				await userObject.SignUpAsync();
				return "OK";
			}
			catch (Exception e)
			{
				return e.Message;
			}

		}

		public static async Task<string> FacebookSignUp()
		{
			UIViewController vc;
			try
			{
				ParseUser user = await ParseFacebookUtils.LogInAsync(Constants.FACEBOOK_APP_ID, ParseFacebookUtils.AccessToken, DateTime.Now);
				return "OK";
			}
			catch (Exception e)
			{
				return e.Message;
			}

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
