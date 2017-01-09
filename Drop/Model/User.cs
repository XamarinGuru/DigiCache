namespace Drop
{
	public class User
	{
		public User(string email, string password)
		{
			this.Email = email;
			this.Password = password;
		}
		public string Email { get; internal set; }
		public string Password { get; internal set; }
	}
}
