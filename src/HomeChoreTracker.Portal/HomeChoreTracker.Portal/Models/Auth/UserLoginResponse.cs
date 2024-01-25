namespace HomeChoreTracker.Portal.Models.Auth
{
    public class UserLoginResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
