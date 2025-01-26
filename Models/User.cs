using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace WebApplication1.Models
{
    public class User
    {
         public ObjectId Id { get; set; }
        public required string Username { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }


          public string? Phone { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string? Role { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public string? Otp { get; set; } // OTP for password reset (nullable)

          public DateTime? OtpExpiry { get; set; } // Nullable for users without active OTP
    }
}