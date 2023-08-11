using DVT.BL.Interfaces;
using DVT.BL.Models;
using DVT.Domain.Enums;
using DVT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.BL.Implemantation
{
    public class ElevatorService : IElevatorService
    {
        private readonly Elevator _elevator;
        public List<Person> PeopleCallingElevator { get; set; }
        public int count = 0;
        public ElevatorService(Elevator elevator)
        {
            _elevator = elevator;
            PeopleCallingElevator = new List<Person>();
        }

        public void FindHighestDestinationFloor()
        {
            foreach (var person in _elevator.NumberOfPeople)
            {
                _elevator.SelectedLevel = Math.Max(_elevator.SelectedLevel, person.DestinationLevel);
            }

        }

        public void FindLowestDestinationFloor()
        {
            foreach (var person in _elevator.NumberOfPeople)
            {
                _elevator.SelectedLevel = Math.Min(_elevator.SelectedLevel, person.DestinationLevel);
            }
        }

        public Direction GetDirection()
        {
            return _elevator.Direction;
        }

        public Status GetStatus()
        {
            return _elevator.Status;
        }
        public void LoadSheddingStop()
        {

            _elevator.Status = Status.NotWorking;
        }
    
        public void MoveElevator(ElevatorRequest request)
        {
            LogCallingElevatorUp(request);
            LogCallingElevatorDown(request);

            if (request.Requesters.Count > 0 && request.Direction == Direction.Up)
                ElevatorMustTakePeopleUp();
            else if (request.Requesters.Count > 0 && request.Direction == Direction.Down)
                ElevatorMustTakePeopleDown();


            if (_elevator.NumberOfPeople.Count > 0)
            {
                PeopleCallingElevator.Clear();

                if (_elevator.CurrentLevel > _elevator.MaxLevel)
                {
                    _elevator.CurrentLevel = _elevator.MaxLevel;
                    ElevatorMustTakePeopleDown();
                }

                if (_elevator.CurrentLevel < 0)
                {
                    _elevator.CurrentLevel = 0;
                    ElevatorMustTakePeopleUp();
                }

                if (_elevator.CurrentLevel < _elevator.MaxLevel && _elevator.CurrentLevel > 0)
                {
                    if (_elevator.NumberOfPeople.Any())
                        if (_elevator.NumberOfPeople.FirstOrDefault().Direction == Direction.Down)
                            //go down
                            ElevatorMustTakePeopleDown();
                        else
                            //go up
                            ElevatorMustTakePeopleUp();

                }

            }

        }

        private void LogCallingElevatorDown(ElevatorRequest request)
        {
            if (_elevator.CurrentLevel > request.Requesters.FirstOrDefault().OriginalLevel)
            {
                var x = (_elevator.CurrentLevel > _elevator.MaxLevel) ? _elevator.CurrentLevel-- : _elevator.CurrentLevel;
                for (int i = x; i > request.Requesters.FirstOrDefault().OriginalLevel; i--)
                {
                    _elevator.Status = Status.Moving;
                    Console.WriteLine($"Elevator currently moving down...current floor = {i} to Requesters floor {request.Requesters.FirstOrDefault().OriginalLevel}");
                }
                _elevator.CurrentLevel = request.Requesters.FirstOrDefault().OriginalLevel;
            }
        }

        private void LogCallingElevatorUp(ElevatorRequest request)
        {
            if (_elevator.CurrentLevel < request.Requesters.FirstOrDefault().OriginalLevel)
            {
                var x = (_elevator.CurrentLevel == 0) ? _elevator.CurrentLevel : _elevator.CurrentLevel++;
                for (int i = x; i < request.Requesters.FirstOrDefault().OriginalLevel; i++)
                {
                    _elevator.Status = Status.Moving;
                    Console.WriteLine($"Elevator currently moving up...current floor = {i} to Requesters floor {request.Requesters.FirstOrDefault().OriginalLevel}");
                }
                _elevator.CurrentLevel = request.Requesters.FirstOrDefault().OriginalLevel;
            }
        }

        private void ElevatorMustTakePeopleUp()
        {
           
            do
                MoveUp();
            while (_elevator.CurrentLevel <= _elevator.SelectedLevel);

            if (_elevator.CurrentLevel == _elevator.MaxLevel)
                FindLowestDestinationFloor();


            if (_elevator.NumberOfPeople.Count == 0)
            {
                foreach (Person person in PeopleCallingElevator)
                {
                    _elevator.SelectedLevel = Math.Min(_elevator.SelectedLevel, person.OriginalLevel);
                }
                PeopleCallingElevator.Clear();
            }
        }

        private void ElevatorMustTakePeopleDown()
        {
          
            do
                MoveDown();
            while (_elevator.CurrentLevel >= _elevator.SelectedLevel);

            if (_elevator.CurrentLevel == 0)
                FindHighestDestinationFloor();

            if (_elevator.NumberOfPeople.Count == 0)
            {
                foreach (Person person in PeopleCallingElevator)
                {
                    _elevator.SelectedLevel = Math.Max(_elevator.SelectedLevel, person.OriginalLevel);
                }
                PeopleCallingElevator.Clear();
            }
        }

        public void MoveDown()
        {
            
            Unload();
            Load(); 
            FindLowestDestinationFloor();
            _elevator.CurrentLevel--;

            if (_elevator.CurrentLevel == _elevator.SelectedLevel)
            {
                _elevator.Status = Status.Waiting;
                Console.WriteLine($"Elevator has arrived to its final floor currently idling waiting for the next command...current floor = {_elevator.CurrentLevel}");
            }
            else if(_elevator.CurrentLevel > _elevator.SelectedLevel)
            {
                _elevator.Status = Status.Moving;
                Console.WriteLine($"Elevator currently moving down...with {_elevator.NumberOfPeople.Count()} people...current floor = {_elevator.CurrentLevel}");
            }
            else
            {
                _elevator.Status = Status.Waiting;
            }

        }

        public void MoveUp()
        {
            
            Unload();
            Load();
            FindHighestDestinationFloor();

            ++_elevator.CurrentLevel;
            if (_elevator.CurrentLevel == _elevator.SelectedLevel)
            {
                _elevator.Status = Status.Waiting;
                Console.WriteLine($"Elevator has arrived to its final floor currently idling waiting for the next command...current floor = {_elevator.CurrentLevel}");
            }
            else if(_elevator.CurrentLevel < _elevator.SelectedLevel)
            {
                _elevator.Status = Status.Moving;
                Console.WriteLine($"Elevator currently moving up...with {_elevator.NumberOfPeople.Count()} people...current floor = {_elevator.CurrentLevel}");
            }
            else
            {
                _elevator.Status = Status.Waiting;
                
            }
            
        }

        public List<Person> Unload()
        {
           
            _elevator.Status = Status.Unloading;

            List<Person> templist = new(_elevator.NumberOfPeople);
            foreach (Person person in templist)
            {
                if (person.DestinationLevel == _elevator.CurrentLevel)
                {
                    Console.WriteLine($"unloading person {person.Id} - {person.DestinationLevel}");

                     _elevator.NumberOfPeople.Remove(person);
                    Console.WriteLine($"People remaining in the elevator count is {_elevator.NumberOfPeople.Count}");
                }
            }
           
            return _elevator.NumberOfPeople;
        }
        public List<Person> Load()
        {

            _elevator.Status = Status.Loading;
            List<Person> templist = new(PeopleCallingElevator);
            foreach (Person person in templist)
            {
                if (_elevator.NumberOfPeople.Count < _elevator.MaxCapacity && person.OriginalLevel == _elevator.CurrentLevel)
                {
                    Console.WriteLine($"loading person {person.Id} - going to floor {person.DestinationLevel}");

                    _elevator.NumberOfPeople.Add(person);
                    Console.WriteLine($"People loaded in the elevator is count is {_elevator.NumberOfPeople.Count}");
                }
            }
           
            return _elevator.NumberOfPeople;
        }
        public bool HasCapacity(int people)
        {
            return _elevator.NumberOfPeople.Count() + people <= _elevator.MaxCapacity;
        }

        public int GetCurrentLevel()
        {
            return _elevator.CurrentLevel;
        }

        public int GetCapacity()
        {
            return _elevator.MaxCapacity;
        }

        public void AddNumberofPeople(List<Person> people)
        {

           PeopleCallingElevator.AddRange(people);
            
        }

        public Guid GetElevatorId()
        {
            return _elevator.Id;
        }
    }
}
