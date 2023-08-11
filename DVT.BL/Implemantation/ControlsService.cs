using DVT.BL.Interfaces;
using DVT.BL.Models;
using DVT.Domain.Enums;
using DVT.Domain.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.BL.Implemantation
{
    public class ControlsService : IControls
    {
        private List<IElevatorService> _elevatorServices = new();
        private Queue<ElevatorRequest> _Queue = new();
        private List<Floor> _floors = new();
        public void Initialize(List<Floor> floors,List<IElevatorService> elevatorServices)
        {
            _elevatorServices = elevatorServices;
            _floors = floors;
            _Queue = new Queue<ElevatorRequest>();
        }

        public void ProcessElevatorRequests()
        {
            while (_Queue.Count > 0)
            {
                ElevatorRequest request = _Queue.Dequeue();

                IElevatorService elevatorService = GetElevator(request);

                if (elevatorService != null)
                {
                    Console.WriteLine($"Elevator Id {elevatorService.GetElevatorId()} is the closest.");
                    elevatorService.AddNumberofPeople(request.Requesters);
                    elevatorService.MoveElevator(request);
                }
                else
                {
                    EnqueueRequest(request.Floor, request.NumberOfPeople, request.Direction,request.Requesters);
                }
                
            }
        }
        private IElevatorService GetElevator(ElevatorRequest request)
        {
            // Get all elevators. Exclude out of order elevators
            var eligibleElevators = _elevatorServices.Where(x =>
                x.GetStatus() != Status.NotWorking &&             
                x.HasCapacity(request.Requesters.Count())).ToList();

            // First, is there a moving elevator, close by, that has enough capacity
            // and is still going towards the people that requested the elevator?
            var moving = eligibleElevators.Where(x =>
                x.GetStatus() == Status.Moving &&
                x.GetDirection() == request.Direction &&
                (
                    (request.Direction == Direction.Up && x.GetCurrentLevel() < request.Floor.Min()) ||
                    (request.Direction == Direction.Down && x.GetCurrentLevel() > request.Floor.Max())
                ));

            // Secondly, give me the stationary elevators, and then check if one is perhaps
            // closer. It may be that one is ready on the same floor.
            var stationary = eligibleElevators.Where(x => x.GetStatus() == Status.Waiting);

            if(stationary.Count()>0)
                return stationary.OrderBy(item => Math.Abs(request.Requesters.FirstOrDefault().OriginalLevel - item.GetCurrentLevel())).First();


            return eligibleElevators.FirstOrDefault();
        }

        public void RequestElevator(ElevatorRequest request)
        {

            var elevatorService = GetElevator(request);

            if (elevatorService is not null)
            {
                Console.WriteLine($"Elevator Id {elevatorService.GetElevatorId()} is the closest.");
                elevatorService.AddNumberofPeople(request.Requesters);
                elevatorService.MoveElevator(request);
            }
            else
            {
                AddToQueue(request);
            }

        }

        public void AddQueue(ElevatorRequest request)
        {
            AddToQueue(request);
        }

        private void AddToQueue(ElevatorRequest request) 
        {
            if (EnqueueRequest(request.Floor, request.NumberOfPeople, request.Direction, request.Requesters))
            {

                ProcessElevatorRequests();
            }
        }
        private bool EnqueueRequest(List<int> floor, int people, Direction direction,List<Person> requesters)
        {
            int maximumCapacity = _elevatorServices.Max(x => x.GetCapacity());
            if (people > maximumCapacity)
            {

                var elevatorNeedCount = people / maximumCapacity;
                for (int i = 0; i < elevatorNeedCount; i++)
                {
                    _Queue.Enqueue(new ElevatorRequest(floor, maximumCapacity, direction, requesters));
                    people -= maximumCapacity;
                }

                if (people % maximumCapacity > 0)
                {
                    _Queue.Enqueue(new ElevatorRequest(floor, people, direction, requesters));
                }

                return true;
            }
            else
            {
                _Queue.Enqueue(new ElevatorRequest(floor, people, direction, requesters));

                return true;
            }
        }

        public Direction GetDirectionsByFloor(int floor)
        {
            var toplevel = _floors.Max(x => x.Level);
           
            var bottomlevel = _floors.Min(y => y.Level);

            if (floor == bottomlevel) 
            {
                return Direction.Up; 
            }
            else if (floor == toplevel)
            {  
                return Direction.Down; 
            }
            else if (floor > bottomlevel && floor < toplevel) 
            {
                return Direction.Up | Direction.Down; 
            }
            else
            {
                return Direction.Idle;
            }
        }
    }
}
