using DVT.BL.Interfaces;
using DVT.Domain.Enums;
using DVT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.BL.Models
{
    public class ElevatorRequest : IElevatorRequest
    {
        public ElevatorRequest(List<int> destination, int people, Direction direction, List<Person> requesters)
        {
            Floor = destination;
            Direction = direction;
            NumberOfPeople = people;
            Requesters = requesters;

        }
        public Direction Direction { get; }
        public List<int> Floor { get; }
        public int NumberOfPeople { get; set; }
        public List<Person> Requesters { get; set; }
    }
}
