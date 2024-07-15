namespace BlazorApp1.Models
{
    public class Server
    {
        public int KeyID { get; set; }
        public string UserName { get; set; } // username
        public string Port { get; set; }
        public string Login { get; set; } // This is login of user, that logged to programm, may be this need take a groupname                                           
        public string Password { get; set; }
        public string Database { get; set; }
        public string Host { get; set; }
    }
}
