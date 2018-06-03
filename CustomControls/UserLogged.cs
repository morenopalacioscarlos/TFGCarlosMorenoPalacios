using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMia.CustomControls
{
    public class UserLogged
    {
        private static UserLogged userLoggedSingleton = null;
        public string User;

        private UserLogged(string user)
        {
            this.User = user;
        }

        public static UserLogged GetInstance(string user)
        {
            if (userLoggedSingleton == null)
                userLoggedSingleton = new UserLogged(user);

            return userLoggedSingleton;
        }

        public static void Dispose()
        {
            userLoggedSingleton = null;
        }

    }
}
