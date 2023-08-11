using DVT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.Domain.Models
{
    public class Elevator: UniqueMarker
    {
        public Elevator(int maxCapacity) 
        {
            NumberOfPeople = new List<Person>();
            CurrentLevel = 0;
            MaxCapacity= maxCapacity;

        }
     
        public int CurrentLevel { get; set; }
        public int SelectedLevel { get; set; }
        public List<Person> NumberOfPeople { get; set; }
        public List<Floor> Floors = new();
        public int MaxCapacity { get; set; }
        public int MaxLevel { get; set; }
        public Direction Direction {
            get
            {
                if (CurrentLevel == 0 || CurrentLevel < SelectedLevel)
                {
                    return Direction.Up;
                }
                else if (CurrentLevel == MaxLevel || CurrentLevel > SelectedLevel)
                {
                    return Direction.Down;
                }
                else
                {
                    return Direction.Idle;
                }
            }
        }
        
        public Status Status { get; set; } = Status.Waiting;
        public void AddFloors(List<Floor> floors)
        {
            foreach (var item in floors)
            {
                if (!Floors.Any(x => x.Id == item.Id))
                    Floors.Add(item);
            }
            
        }
    }
}
