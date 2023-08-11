using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVT.BL.Implemantation;
using DVT.BL.Interfaces;
using DVT.BL.Models;
using DVT.BL.Tests.Ioc;
using DVT.Domain.Enums;
using DVT.Domain.Models;
using Lamar;
using IContainer = Lamar.IContainer;

namespace DVT.BL.Tests
{
    public class ControlTests
    {
        private IContainer _container;
        private  IControls _controller;
        public List<Elevator> Elevators { get; set; } = new();
        [OneTimeSetUp]
        public void INIT()
        {
            _container = IoCContainer.Init();
            _controller = _container.GetInstance<IControls>();

            List<IElevatorService> services = new();
            List<Floor> floors = AddFloor(10);

            for (int i = 0; i < 2; i++)
            {
                Elevators.Add(new Elevator(3));

                Elevators[i].AddFloors(floors);
                
                services.Add(new ElevatorService(Elevators[i]));
            }

            _controller.Initialize( floors,services);
        }

        [Test]
        public void Request_Elevator()
        {
            int firstRequestFloor = 4, poeple = 1;
            var direction = Direction.Up;
            var requesters = new List<Person>();
            var destination = new List<int>();
            requesters.Add(new Person(0, 4));
            destination.Add(4);

            // Act
            _controller.RequestElevator(new ElevatorRequest(destination, poeple, direction, requesters));

            // Assert
            Assert.That(Elevators.Count(x => x.CurrentLevel == firstRequestFloor+1), Is.EqualTo(1));
        }
        [Test]
        public void AddRequestToQueueAndProcessQueue_Elevator()
        {
           
            int firstRequestFloor = 9, poeple = 2;
            var direction = Direction.Up;
            var requesters = new List<Person>();
            var destination = new List<int>();
            requesters.Add(new Person(0, 4));
            requesters.Add(new Person(0, 8));
            destination.Add(4);
            destination.Add(8);

            // Act
            _controller.AddQueue(new ElevatorRequest(destination, poeple, direction, requesters));

            // Assert
            Assert.That(Elevators.Count(x => x.CurrentLevel == firstRequestFloor), Is.EqualTo(1));
        }
        private List<Floor> AddFloor(int floorNumber)
        {
            List<Floor> floors = new();
            for (int i = 0; i < floorNumber; i++)
            {
                Floor floor = new()
                {
                    Name = $"Floor number - {i}",
                    Level = i
                };

                floors.Add(floor);
            }
            return floors;
        }
    }
}
