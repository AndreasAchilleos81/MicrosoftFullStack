using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; } // This name must match the JSON key

        public string role { get; set; }

        public string userId { get; set; }

        public string expires { get; set; }
    }
}
