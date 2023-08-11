using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.Domain.Models
{
    public abstract class UniqueMarker
    {
        private Guid _id = Guid.NewGuid();
        public Guid Id
        {
            get {
                return _id;
            }
            set
            {
                _id = value;
            }          
        }
    }
}
