using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Interface
{
    public interface IToken
    {
        string CreateToken(string userid, string username, string email);
    }
}