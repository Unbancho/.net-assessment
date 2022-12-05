using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


internal class Program
{
    public class MarketingEngine
    {
        private List<Event> Events { get; }
        public static Dictionary<(string, string), int> Distances { get; } = new();

        public MarketingEngine(List<Event> events)
        {
            Events = events;
        }
        
        private static int GetDistance(string cityA, string cityB, int tries=1)
        {
            if (Distances.TryGetValue((cityA, cityB), out var distance)) return distance;
            while (tries > 0)
            {
                try
                {
                    distance = AlphebiticalDistance(cityA, cityB);
                    break;
                }
                catch (TimeoutException e)
                {
                    Console.WriteLine($"Got {e} with {cityA} and {cityB}");
                    if (tries < 2)
                        throw;
                }
            }
            Distances[(cityA, cityB)] = distance;
            Distances[(cityB, cityA)] = distance;

            return distance;
        }
        public static void SendCustomerNotifications(Customer customer, Event e)
        { 
            Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} in {e.City} at {e.Date}");
        }

        public void SendEventsInCity(Customer customer)
        {
            var events = Events.FindAll(e => e.City == customer.City);
            foreach (var e in events)
            {
                SendCustomerNotifications(customer, e);
            }
        }

        public void SendNearestEvents(Customer customer, int nEvents = 5)
        {
            var events = Events.OrderBy(e => GetDistance(customer.City, e.City)).Take(nEvents);
            foreach (var e in events)
            {
                SendCustomerNotifications(customer, e);
            }
        }

        public void SendEvents(Customer customer, Func<Event, int> func, int nEvents = 0)
        {
            var events = Events.OrderBy(func).Take(nEvents);
            foreach (var e in events)
            {
                SendCustomerNotifications(customer, e);
            }
        }

        public void SendNearestEventsToBirthday(Customer customer, int nEvents = 5)
        {
            var events = Events.OrderBy(e =>
            {
                var next = customer.BirthDate.AddYears(DateTime.Today.Year - customer.BirthDate.Year);
                if (next < DateTime.Today)
                    next = next.AddYears(1);
                return (next - e.Date).Duration();
            }).Take(nEvents);
            foreach (var e in events)
            {
                SendCustomerNotifications(customer, e);
            }
        }
    }
    
    private static int AlphebiticalDistance(string s, string t)
    {
        var result = 0;
        var i = 0;
        for(i = 0; i < Math.Min(s.Length, t.Length); i++)
        {
            result += Math.Abs(s[i] - t[i]);
        }
        for(; i < Math.Max(s.Length, t.Length); i++)
        {
            result += s.Length > t.Length ? s[i] : t[i];
        }
                    
        return result;
    }
    

    static void Main(string[] args)
    {
        var events = new List<Event>{
            new (1, "Phantom of the Opera", "New York", new(2023,12,23)),
            new (2, "Metallica", "Los Angeles", new(2023,12,02)),
            new (3, "Metallica", "New York", new(2023,12,06)),
            new (4, "Metallica", "Boston", new(2023,10,23)),
            new (5, "LadyGaGa", "New York", new(2023,09,20)),
            new (6, "LadyGaGa", "Boston", new(2023,08,01)),
            new (7, "LadyGaGa", "Chicago", new(2023,07,04)),
            new (8, "LadyGaGa", "San Francisco", new(2023,07,07)),
            new (9, "LadyGaGa", "Washington", new(2023,05,22)),
            new (10, "Metallica", "Chicago", new(2023,01,01)),
            new (11, "Phantom of the Opera", "San Francisco", new(2023,07,04)),
            new (12, "Phantom of the Opera", "Chicago", new(2024,05,15))
        };

        var customers = new List<Customer> {new()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new(1995, 05, 10)
            }, 
            new()
            {
                Id = 1,
                Name = "Bill",
                City = "Boston",
                BirthDate = new(1988, 02, 29)
            }
            , new()
            {
                Id = 1,
                Name = "William",
                City = "Washington",
                BirthDate = new(1997, 12, 11)
            }
        };

        var engine = new MarketingEngine(events);
        foreach (var customer in customers)
        {
            engine.SendEventsInCity(customer);
        }
        foreach (var customer in customers)
        {
            engine.SendNearestEvents(customer);
        }
        foreach (var customer in customers)
        {
            engine.SendNearestEventsToBirthday(customer);
        }
    }

    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }

        public Event(int id, string name, string city, DateTime date)
        {
            this.Id = id;
            this.Name = name;
            this.City = city;
            this.Date = date;
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public DateTime BirthDate { get; set; }
    }
}