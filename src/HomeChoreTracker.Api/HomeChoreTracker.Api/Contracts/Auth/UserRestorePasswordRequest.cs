namespace HomeChoreTracker.Api.Contracts.Auth
{
    public class UserRestorePasswordRequest
    {
        public string? Password { get; set; }
        public string? Token { get; set; }
    }
}
