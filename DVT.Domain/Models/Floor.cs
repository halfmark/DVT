using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.Domain.Models
{
    public class Floor: UniqueMarker
    {
        public string Name { get; set; }
        public int Level { get; set; }
    }
}
