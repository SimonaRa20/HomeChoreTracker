namespace HomeChoreTracker.Api.Contracts.Auth
{
    public class UserRegisterResponse
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
