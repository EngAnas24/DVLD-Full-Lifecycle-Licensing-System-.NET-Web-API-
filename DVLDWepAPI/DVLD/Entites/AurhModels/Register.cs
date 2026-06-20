using System;

namespace Entites.AurhModels
{
    public class Register
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? RePassword { get; set; }

    }

}