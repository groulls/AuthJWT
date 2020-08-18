using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthReg.Interface
{
    public interface IAuthenticationJWT
    {
        string Authenticate(string username, string password);
    }
}
