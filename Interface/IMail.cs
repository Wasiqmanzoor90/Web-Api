using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Interface
{
    public interface IMail
    {
        Task SendEmailAsync(string to, string subject, string body, bool ishtml=false);
    }
}