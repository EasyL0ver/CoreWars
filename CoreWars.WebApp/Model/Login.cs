using System.ComponentModel;

namespace CoreWars.WebApp.Model
{
    public class Login
    {
        public Login(string username, string password)
        {
            Username = username;
            Password = password;
        }

        [DefaultValue("admin@example.com")]
        public string Username { get; }
        
        [DefaultValue("pass")]
        public string Password { get; }
    }
}