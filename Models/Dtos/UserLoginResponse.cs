﻿namespace UserAuthAPI.Models.Dtos
{
    public class UserLoginResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
