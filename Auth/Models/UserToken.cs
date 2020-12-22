namespace Readible.Auth
{
    public class UserToken
    {
		public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
    }
}