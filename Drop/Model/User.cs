namespace Drop
{
	public class User
	{
		public User()
		{
		}
		public User(string email, string password, string firstname, string lastname, string photoURL)
		{
			this.Firstname = firstname;
			this.Lastname = lastname;
			this.Email = email;
			this.Password = password;
			this.PhotoURL = photoURL;
		}
		public string Firstname { get; internal set; }
		public string Lastname { get; internal set; }
		public string Email { get; internal set; }
		public string Password { get; internal set; }
		public string PhotoURL { get; internal set; }
		public int Type { get; internal set; }
	}
}
