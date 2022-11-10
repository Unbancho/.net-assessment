internal class Event
{
    public string Name { get; set; }
    public string City { get; set; }
}

internal class Customer
{
    public string Name { get; set; }
    public string City { get; set; }
}

static class Program
{
    private static void AddToEmail(Customer customer, Event @event)
    {
        Console.WriteLine($"Sent email about {@event.Name} in [{@event.City}] to {customer.Name} in [{customer.City}]");
    }

    private static int GetDistance(string cityA, string cityB)
    {
        var distance = 0;
        for (var i = 0; i < Math.Min(cityA.Length, cityB.Length); i++)
        {
            distance += Math.Abs(cityA[0] - cityB[0]);
        }
        return distance;
    }

    // Question 4
    private static int GetDistanceSafe(string cityA, string cityB)
    {
        try
        {
            return GetDistance(cityA, cityB);
        }
        catch (Exception e)
        {
            return int.MaxValue;
        }
    }

    private static int GetPrice(Event @event) => @event.Name.Length;

    private static void Main(string[] args)
    {
        var events = new List<Event>{
            new() { Name = "Phantom of the Opera", City = "New York"},
            new() { Name = "Metallica", City = "Los Angeles"},
            new() { Name = "Metallica", City = "New York"},
            new() { Name = "Metallica", City = "Boston"},
            new() { Name = "LadyGaGa", City = "New York"},
            new() { Name = "LadyGaGa", City = "Boston"},
            new() { Name = "LadyGaGa", City = "Chicago"},
            new() { Name = "LadyGaGa", City = "San Francisco"},
            new() { Name = "LadyGaGa", City = "Washington"}
        };
        var customers = new List<Customer>{
            new (){ Name = "Nathan", City = "New York"},
            new (){ Name = "Bob", City = "Boston"},
            new (){ Name = "Cindy", City = "Chicago"},
            new (){ Name = "Lisa", City = "Los Angeles"},
            
            new (){ Name = "John Smith", City = "New York"}
        };
        
        Console.WriteLine("Question 1:");
        Question1(events, customers);
        Console.WriteLine("Question 2:");
        Question2(events, customers);
        Console.WriteLine("Question 5:");
        Question5(events, customers);
    }

    private static void Question1(List<Event> events, List<Customer> customers)
    {
        var pairs = customers
            .Join(events, customer => customer.City, @event => @event.City,
            (customer, @event) => (customer, @event));

        foreach (var pair in pairs)
        {
            AddToEmail(pair.customer, pair.@event);
        }
    }

    private static void Question2(List<Event> events, List<Customer> customers)
    {
        const int nEvents = 5;
        var nearestEvents = new Dictionary<string, IEnumerable<Event>>();
        foreach (var @event in events)
        {
            if (nearestEvents.ContainsKey(@event.City))
                continue;
            nearestEvents[@event.City] = events
                .OrderBy(e => GetDistanceSafe(@event.City, e.City))
                .Take(nEvents);
        }
        var pairs = customers
            .SelectMany(customer => nearestEvents[customer.City], (customer, @event) => new {customer, @event});
        foreach (var pair in pairs)
        {
            AddToEmail(pair.customer, pair.@event);
        }
    }

    private static void Question5(List<Event> events, List<Customer> customers)
    {
        const int nEvents = 5;
        var nearestEvents = new Dictionary<string, IEnumerable<Event>>();
        foreach (var @event in events)
        {
            if (nearestEvents.ContainsKey(@event.City))
                continue;
            nearestEvents[@event.City] = events
                .OrderBy(e => GetDistanceSafe(@event.City, e.City))
                .ThenBy(GetPrice)
                //.ThenBy(GetField) (...)
                .Take(nEvents);
        }
        var pairs = customers
            .SelectMany(customer => nearestEvents[customer.City], (customer, @event) => new {customer, @event});
        foreach (var pair in pairs)
        {
            AddToEmail(pair.customer, pair.@event);
        }
    }
}
