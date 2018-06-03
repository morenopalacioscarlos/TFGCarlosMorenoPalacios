using System;
using System.Text;

namespace WebMia.Services
{
    public class InfoLowStockSave<T1, T2>
    {

        public StringBuilder sb = null;
        public int id = 0;

        public InfoLowStockSave(Tuple<StringBuilder, int> tuple)
        {
            this.sb = tuple.Item1;
            this.id = tuple.Item2;
        }
    }
}