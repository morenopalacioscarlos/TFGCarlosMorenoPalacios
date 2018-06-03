

using System.Collections.Generic;
using WebMia.Models;

namespace WebMia.Interfaces
{
    public interface ILogged
    {
        bool IsUserLogged();
        string GetUserLogued();
        int GetUserIdLogged();
        List<string> IsAllowed();
        int GetUserRolId();
        Admin GetActualUser();
        string getNewToken();

    }
}
