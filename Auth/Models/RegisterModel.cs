namespace Readible.Auth
{
    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int Birth { get; set; }
        public bool Male { get; set; }

        public bool UsernameConflict { get; set; }
        public bool EmailConflict { get; set; }
    }
}