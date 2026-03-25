namespace Shared.Models.DTO
{
    public class LoginResponse
    {
        public Value value {get; set;}

        public string executionMs { get; set; }
    }

    public class Value
    {
        public string Token { get; set; } // This name must match the JSON key

        public string role { get; set; }

        public string userId { get; set; }

        public string expires { get; set; }
    }
}
