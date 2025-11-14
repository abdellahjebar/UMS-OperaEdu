using System;
using UMS.Core.Entities.Base;
using UMS.Core.Enums;

namespace UMS.Core.Entities.Identity
{
    public class User : BaseEntity
    {
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}