using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using Ddd.Infrastructure;

namespace Ddd.Taxi.Domain
{
	public class DriversRepository
	{
		public Driver FillDriverToOrder(int driverId)
		{
			if (driverId == 15)
				return new Driver(driverId, new PersonName("Drive", "Driverson"), new Car("Baklazhan", "Lada sedan", "A123BT 66"));
			else
				throw new Exception("Unknown driver id " + driverId);
		}
	}

	public class TaxiApi : ITaxiApi<TaxiOrder>
	{
		private readonly DriversRepository driversRepo;
		private readonly Func<DateTime> currentTime;
		private int idCounter;

		public TaxiApi(DriversRepository driversRepo, Func<DateTime> currentTime)
		{
			this.driversRepo = driversRepo;
			this.currentTime = currentTime;
		}

		public TaxiOrder CreateOrderWithoutDestination(string firstName, string lastName, string street, string building) 
			=>new TaxiOrder(idCounter++, new PersonName(firstName, lastName), new Address(street, building), currentTime());

		public void UpdateDestination(TaxiOrder order, string street, string building) =>
			order.UpdateDestination(new Address(street, building));

		public void AssignDriver(TaxiOrder order, int driverId)=>
			order.AssignDriver(driversRepo.FillDriverToOrder(driverId),currentTime());

		public void UnassignDriver(TaxiOrder order) =>
			order.UnassignDriver();
		public void Cancel(TaxiOrder order) =>
		 order.Cancel(currentTime());

		public void StartRide(TaxiOrder order) =>
		 order.StartRide(currentTime());

		public void FinishRide(TaxiOrder order) =>
		 order.FinishRide(currentTime());

		public string GetDriverFullInfo(TaxiOrder order) =>
			  string.Join(" ",
				$"Id: {order.Driver.Id}",
				$"DriverName: {order.Driver.Name.FormatName()}",
				$"Color: {order.Driver.Car.Color}",
				$"CarModel: {order.Driver.Car.Model}",
				$"PlateNumber: {order.Driver.Car.PlateNumber}");

		public string GetShortOrderInfo(TaxiOrder order)
			=> string.Join(" ",
				$"OrderId: {order.Id}",
				$"Status: {order.Status}",
				$"Client: {order.ClientName.FormatName()}",
				$"Driver: {order.Driver?.Name.FormatName()}",
				$"From: {order.Start.FormatAddress()}",
				$"To: {order.Destination.FormatAddress()}",
				$"LastProgressTime: {order.GetLastProgressTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}");
	}

	public class TaxiOrder : Entity<int>
	{
		public PersonName ClientName { get; private set; }
		public Address Start { get; private set; }
		public Address Destination { get; private set; }
		public Driver Driver { get;  private set; }
		private List<Tuple<TaxiOrderStatus,DateTime>> Progress { get; }
		public TaxiOrderStatus Status { get => Progress.Last().Item1; }

		public TaxiOrder(int id,PersonName client, Address startAddres, DateTime creationTime):base(id)
        {
			Start = startAddres;
			ClientName = client;
			Progress = new List<Tuple<TaxiOrderStatus, DateTime>>() 
			{ new Tuple<TaxiOrderStatus, DateTime>(TaxiOrderStatus.WaitingForDriver, creationTime) };
        }

		public DateTime GetLastProgressTime() => Progress.Last().Item2;

		public void UpdateDestination(Address destantionAddres)=> Destination = destantionAddres;

		public void Cancel(DateTime cancelTime)
		{
			if (Status == TaxiOrderStatus.WaitingCarArrival || Status == TaxiOrderStatus.WaitingForDriver)
				Progress.Add(new Tuple<TaxiOrderStatus, DateTime>(TaxiOrderStatus.Canceled, cancelTime));
			else
				throw new InvalidOperationException(Status.ToString());
		}

		public void AssignDriver(Driver driver, DateTime assignTime)
		{
			if (Status == TaxiOrderStatus.WaitingForDriver || driver is null)
			{
				Driver = driver;
				Progress.Add(new Tuple<TaxiOrderStatus, DateTime>(TaxiOrderStatus.WaitingCarArrival, assignTime));
			}
			else
				throw new InvalidOperationException(Status.ToString());
		}

		public void UnassignDriver()
		{
			if (Status == TaxiOrderStatus.WaitingCarArrival && Driver != null)
			{
				Driver = null;
				Progress.Add(new Tuple<TaxiOrderStatus, DateTime>(TaxiOrderStatus.WaitingForDriver, Progress[0].Item2));
			}
			else
				throw new InvalidOperationException(Status.ToString());
		}

		public void StartRide(DateTime startTime)
		{
			if (Status == TaxiOrderStatus.WaitingCarArrival)
				Progress.Add(new Tuple<TaxiOrderStatus, DateTime>(TaxiOrderStatus.InProgress, startTime));
			else
				throw new InvalidOperationException(Status.ToString());
		}

		public void FinishRide(DateTime endTime) 
		{
			if (Status == TaxiOrderStatus.InProgress)
				Progress.Add(new Tuple<TaxiOrderStatus, DateTime>(TaxiOrderStatus.Finished, endTime));
			else
				throw new InvalidOperationException(Status.ToString());
		}
	}

	public class Driver : Entity<int>
	{
		public PersonName Name { get; private set; }
		public Car Car { get; private set; }

		public Driver(int id, PersonName taxistPersonName, Car car) : base(id)
		{
			Car = car;
			Name = taxistPersonName;
		}
	}

	public class Car : ValueType<Car>
	{
		public string Color { get; private set; }
		public string Model { get; private set; }
		public string PlateNumber { get; private set; }

		public Car(string color, string model, string plateNumber)
		{
			Color = color;
			Model = model;
			PlateNumber = plateNumber;
		}
	}

	public static class Services
    {
		public static string FormatName(this PersonName name)=> 
			name is null ? "" : name.FirstName + " " + name.LastName;
		public static string FormatAddress(this Address address)=> 
			address is null ? "" : address.Street + " " + address.Building;
	}
}