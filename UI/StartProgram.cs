using DVT.BL.Implemantation;
using DVT.BL.Interfaces;
using DVT.BL.Models;
using DVT.Domain.Enums;
using DVT.Domain.Models;
using JasperFx.Core;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Interface;

namespace UI
{
    public class StartProgram: IStartProgram
    {
        private readonly IControls _controller;
        private List<Floor> _floors = new();
        public StartProgram(IControls controller) { 
            _controller = controller;
        
        }
       
        public void ConfigureThenRun()
        {
            Console.OutputEncoding = Encoding.UTF8; // make sure we can print out UTF8 characters
            Console.WriteLine("Stating");


            //var opt = Console.ReadLine();
            Options option;

            do
            {
                DisplayOptions();
                if (Enum.TryParse(Console.ReadLine(), out option)) 
                {
                    switch (option)
                    {
                        case Options.AddElevator:
                            if (!_floors.Any())
                            {
                                Console.WriteLine("How many floors would like to create? eg 5");
                                var numberoffloors = InputToNumber();
                                AddFloor(numberoffloors);

                            }
                            AddElevator();
                            break;
                        case Options.Start:
                            if (!_floors.Any())
                            {
                                Console.WriteLine("How many floors would like to create? eg 5");
                                var numberoffloors = InputToNumber();
                                AddFloor(numberoffloors);
                            }

                            if (!Elevators.Any())
                            {
                                AddElevator();
                            }
                            break;
                        case Options.Quit:
                            break;
                        default:
                            break;
                    }

                }
               

            } while (option != Options.Quit && option != Options.Start);


            if (option == Options.Quit)
                Console.Clear();
            else
                Run();
        }
        public List<Elevator> Elevators { get; set; } = new();
        private bool _isRunning;
        public void Run()
        {
            if (Elevators.Count == 0)
            {
                _isRunning = false;
                return;
            }

            _isRunning = true;
            List<IElevatorService> services = new();

            foreach (Elevator elevator in Elevators)
            {
                services.Add(new ElevatorService(elevator));
            }

            _controller.Initialize(_floors ,services) ;

            Console.Clear();

            ImpersonatePerson();

          
        }

        private void ImpersonatePerson()
        {
            while (_isRunning)
            {
              
                Console.WriteLine("What floor are you requesting from? e.g 3");
                var currentFloor = InputToNumber();
                var directions = _controller.GetDirectionsByFloor(currentFloor);
                Console.WriteLine("How many are you? from the floor requesting e.g 7");
                var people = InputToNumber();
                
                var remainingPeople =  Elevators.Max(x => x.MaxCapacity) < people ? people-Elevators.Max(x => x.MaxCapacity) : 0;

                (List<Person>  group,int adjustedPeople) = CreateRequesters(people,currentFloor);

                var destinationFloor = group.Where(x => x.OriginalLevel == currentFloor).OrderByDescending(c=>c.DestinationLevel).Select(x => x.DestinationLevel).ToList();
                
                if (directions.HasFlag(Direction.Up | Direction.Down))
                {
                   directions = WhichDirectionToGoFirst(destinationFloor,currentFloor);
                }
                
                Console.WriteLine("Requesting Elevator...");
                
                _controller.RequestElevator(new ElevatorRequest(destinationFloor, adjustedPeople, directions, group));

                if (remainingPeople > 0)
                {
                    Console.WriteLine($"Processing Queue Requesting Elevator... for remaining people - {remainingPeople}");
                    (List<Person> groupQ, int adjustedPeopleQ) = CreateRequesters(remainingPeople, currentFloor);

                    var destinationFloorQ = groupQ.Where(x => x.OriginalLevel == currentFloor).OrderByDescending(c => c.DestinationLevel).Select(x => x.DestinationLevel).ToList();

                    if (directions.HasFlag(Direction.Up | Direction.Down))
                    {
                        directions = WhichDirectionToGoFirst(destinationFloorQ, currentFloor);
                    }
                    _controller.AddQueue(new ElevatorRequest(destinationFloorQ, adjustedPeopleQ, directions, groupQ));
                }
            }

        }

        private Direction WhichDirectionToGoFirst(List<int> destinationFloor,int currentFloor)
        {
            try
            {
                var shortest = new Dictionary<int, int>();
                foreach (var item in destinationFloor)
                {
                    var i = Math.Abs(currentFloor - item);
                    shortest.Add(i, item);
                }
                var s = shortest.Min(x => x.Key);
                if (shortest.FirstOrDefault(x => x.Key == s).Value > currentFloor)
                    return Direction.Up;

                return Direction.Down;
            }
            catch (ArgumentException)
            {
                return Direction.Up;//choose to go up
            }
            
        }

        private (List<Person> group,int adjustedPeople) CreateRequesters(int adjustedPeople, int currentFloor)
        {
            if (Elevators.Where(x => x.MaxCapacity <= adjustedPeople).Any())
            {

                Console.WriteLine($"No of the Elevators can accomodate {adjustedPeople} people. The Elevator will take only {Elevators.Max(x => x.MaxCapacity)}. And remaining will be added to a queue");
                adjustedPeople = Elevators.Max(x => x.MaxCapacity);

            }
            var requesters = new List<Person>();

            Console.WriteLine(string.Format("Available floors for selection - {0} from {1} to {2}", _floors.Count, _floors.Min(x=>x.Level), _floors.Max(x => x.Level)));

            for (int i = 0; i < adjustedPeople; i++) {
                Console.WriteLine($"Which floor is person {i+1} going to? e.g 2");
                var destination = InputToNumber();

                if (destination <= _floors.Max(x => x.Level) && destination >= _floors.Min(x => x.Level))
                    requesters.Add(new Person(currentFloor, destination));
                else
                    i--;
            }

            return (requesters, adjustedPeople);
        }

        private static int InputToNumber()
        {

            string input = Console.ReadLine();

            _ = int.TryParse(input, out int val);

            return val;
        }
        private static void DisplayOptions()
        {
            foreach (Options opt in Enum.GetValues(typeof(Options)))
            {
                Console.WriteLine(string.Format("{0}) {1}", (int)opt, opt));
            }
        }
        private void AddElevator()
        {
            var capacity = 3;
            var newElevator = new Elevator(capacity);
            
            Elevators.Add(newElevator);
            Console.WriteLine($"New elevator has been added with capacity of {capacity}");

            AddFloorsToNewElevator(newElevator);
        }

        private void AddFloorsToNewElevator(Elevator newElevator)
        {
            if (_floors.Count > 0) {

                newElevator.AddFloors(_floors);
                newElevator.MaxLevel = _floors.Max(x => x.Level);
            }
            Console.WriteLine($"{_floors.Count} floors added to Elevator {newElevator.Id}.");
        }

        private void AddFloor(int floorNumber)
        {
            for (int i = 0; i < floorNumber; i++)
            {
                Floor floor = new()
                {
                    Name = $"Floor number - {i}",
                    Level = i
                };

                _floors.Add(floor);
            }
            Console.WriteLine($"{floorNumber} floors created.");
        }
    }
}
