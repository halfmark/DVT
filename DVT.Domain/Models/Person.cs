using DVT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.Domain.Models
{
    public class Person: UniqueMarker
    {
        public Person(int currentlevel, int finallevel)
        {
            OriginalLevel = currentlevel;
            DestinationLevel = finallevel;
  
        }
        public int OriginalLevel { get; set; }
        public int DestinationLevel { get; set; }
        public Direction Direction
        {
            get
            {
                if (OriginalLevel < DestinationLevel) 
                    return Direction.Up;
                else 
                    return Direction.Down;
            }
        }
    }
}
