namespace Viagogo;

public class Event
{
    public string Name{ get; set; }
    public string City{ get; set; }
}
public class Customer
{
    public string Name{ get; set; }
    public string City{ get; set; }
}
    
public static class Solution
{
    private static readonly Dictionary<(string, string), int> Distances = new();


    // Wrapper method for GetDistance. It takes care of the caching and the retrying/exception handling.
    static int GetDistanceWrapper(string cityA, string cityB, int tries=1)
    {
        if (!Distances.TryGetValue((cityA, cityB), out var distance) 
            && !Distances.TryGetValue((cityB, cityA), out distance))
        {
            while (tries > 0)
            {
                try
                {
                    distance = GetDistance(cityA, cityB);
                    break;
                }
                catch (TimeoutException)
                {
                    Console.WriteLine("Retrying...");
                    tries--;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Got {e} with {cityA} and {cityB}");
                    throw;
                }
            }
            Distances[(cityA, cityB)] = distance;
        }

        return distance;
    }

    private static void Main()
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
        var customers = new List<Customer>()
        {
            new() { Name = "Bob", City = "Boston"},
            new() { Name = "Nathan", City = "New York"},
            new() { Name = "Cindy", City = "Chicago"},
            new() { Name = "Lisa", City = "Los Angeles"},
            new() {Name = "John Smith", City = "New York"}
        };
        
        //1. Add events that will happen in the city of each customer to email
        var pairs1 = customers
            .Join(events, customer => customer.City, @event => @event.City, (customer, @event) => (customer, @event));
        foreach(var item in pairs1)
        {
            AddToEmail(item.customer, item.@event);
        }
        
        // 2. Add 5 nearest events to each customer to email
        var pairs2 = customers
            .SelectMany(customer => events
                .OrderBy(@event =>
                    GetDistanceWrapper(customer.City, @event.City, 3))
                .Take(5)
                .Select(@event => (customer, @event)));
        foreach (var pair in pairs2)
        {
            AddToEmail(pair.customer, pair.@event, GetPrice(pair.@event));
        }
    
        // 3. Same as 2. but sort by price instead.
        var pairs3 = customers
            .SelectMany(customer => events
                .OrderBy(GetPrice)
                .Take(5)
                .Select(@event => (customer, @event)));
        foreach (var pair in pairs3)
        {
            AddToEmail(pair.customer, pair.@event, GetPrice(pair.@event));
        }
    }

    // You do not need to know how these methods work
    static void AddToEmail(Customer c, Event e, int? price = null)
    {
        var distance = GetDistance(c.City, e.City);
        Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
                              + (distance > 0 ? $" ({distance} miles away)" : "")
                              + (price.HasValue ? $" for ${price}" : ""));
    }

    static int GetPrice(Event e)
    {
        return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
    }

    static int GetDistance(string fromCity, string toCity)
    {
        return AlphebiticalDistance(fromCity, toCity);
    }

    private static int AlphebiticalDistance(string s, string t)
    {
        var result = 0;
        var i = 0;
        for(i = 0; i < Math.Min(s.Length, t.Length); i++)
        {
            // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
            result += Math.Abs(s[i] - t[i]);
        }
        for(; i < Math.Max(s.Length, t.Length); i++)
        {
            // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
            result += s.Length > t.Length ? s[i] : t[i];
        }
        return result;
    }
}
