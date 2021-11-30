namespace CarsInfoApp.Utils
{
    using CarsInfoApp.Models;
    using System;

    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
        public Guid? ValidateJwtToken(string token);
    }
}
