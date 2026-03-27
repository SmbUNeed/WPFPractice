using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFMaster;
public static class AuthService
{
    public static bool Auth(string login, string password)
    {
        Users user = Core.Context.Users.FirstOrDefault(u => u.Login == login);
        return user?.Password == password;
    }
}