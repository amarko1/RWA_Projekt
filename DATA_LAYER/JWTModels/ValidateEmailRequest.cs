﻿namespace DATA_LAYER.JWTModels

{
    public class ValidateEmailRequest
    {
        public string Username { get; set; }
        public string B64SecToken { get; set; }
    }
}
