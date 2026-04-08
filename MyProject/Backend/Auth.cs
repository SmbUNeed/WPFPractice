using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyProject.Data;

namespace MyProject.Backend
{
    public static class Auth
    {
        public static Users CurrentUser;
        public static bool IsAuthenticated => CurrentUser != null;

        public static bool Register(string login, string fullname, string email, string phoneNumber, string password)
        {
            if (Core.Context.Users.Any(u => u.Login == login || u.Email == email)) { return false; }
            Users user = new Users
            {
                Login = login,
                Fullname = fullname,
                Email = email,
                PhoneNumber = phoneNumber,
                Password = password,
                RoleId = Core.Context.Roles.First(r => r.Name == "Client").Id
            };
            Core.Context.Users.Add(user);
            Core.Context.SaveChanges();
            return true;
        }

        public static bool Authorize(string login, string password)
        {
            Users user = Core.Context.Users.FirstOrDefault(u => u.Login == login || u.Password == password);
            if (user == null) { return false; }
            CurrentUser = user;
            Console.WriteLine($"Текущий пользователь: {user.Fullname}");
            return true;
        }
    }
}
