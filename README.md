# .net-assessment

## Question 1
# 1. 
Joining events and customers by City.
# 2. 
Iterating through the joined (customer, event) pairs and calling the method.
# 3. 
Assuming **Customer{ Name = "John Smith", City = "New York"}**, email will contain the following events:
  Event{ Name = "Phantom of the Opera", City = "New York"},
  Event{ Name = "Metallica", City = "New York"},
  Event{ Name = "LadyGaGa", City = "New York"}.
# 4. 
 It might be better to use IEnumerable instead of List.
 
## Question 2
# 1. 
Create a dictionary with the events' cities as keys and a sorted (by distance to the key city) collection of all events as a value, pre-computed.
# 2. 
Take the 5 first events in the customer's city's list, as they're sorted by distance.
# 3. 
Assuming **Customer{ Name = "John Smith", City = "New York"}**, and assuming alphabetical distance, email will contain the following events:
Phantom of the Opera in New York
Metallica in New York
LadyGaGa in New York
Metallica in Los Angeles
LadyGaGa in San Francisco

## Question 3
Limiting the amount of times the operation is done by pre-computing the distances in a dictionary (hash), and checking if a city has already been computed instead of overwriting.

## Question 4
We can create a wrapper function that calls GetDistance, catches the exception, and treats it.

## Question 5
We would sort by all desired fields (such as by calling OrderBy and ThenBy), then take the desired amount from the result.
 
