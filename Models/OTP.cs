using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class OTP
    {
        public required string Pass{get;set;}
         public required string Confpass{get;set;}
          public required int Otp{get;set;}
             public DateTime? DateModified { get; set; }
    }
}