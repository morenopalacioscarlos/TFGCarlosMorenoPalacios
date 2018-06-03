using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMia.Models;

namespace WebMia.Interfaces
{
    public interface ILoggingTime
    {
        void NewUserLogin(int userId);
        List<Tuple<int, string>> GetUserLogin();
    }
}
