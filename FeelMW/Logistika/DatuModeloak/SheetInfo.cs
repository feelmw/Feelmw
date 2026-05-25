using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeelmwLogistika.Logistika.DatuModeloak
{
    public class SheetInfo
    {
        public string Id { get; set; } = "";
        public string Nombre { get; set; } = "";

        public override string ToString()
        {
            return Nombre;
        }
    }
}
