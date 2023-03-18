using System;
using System.Collections.Generic;

public class EventWithPriority
{
    public string Name { get; set; }
    public int Priority { get; set; }
    public string Data { get; set; }
}


public class Publisher
{
    
    private Dictionary<int, List<Action<EventWithPriority>>> subscribers = new Dictionary<int, List<Action<EventWithPriority>>>();

    
    public void Subscribe(int priority, Action<EventWithPriority> subscriber)
    {
        if (!subscribers.ContainsKey(priority))
        {
            subscribers[priority] = new List<Action<EventWithPriority>>();
        }
        subscribers[priority].Add(subscriber);
    }

    
    public void Unsubscribe(int priority, Action<EventWithPriority> subscriber)
    {
        if (subscribers.ContainsKey(priority))
        {
            subscribers[priority].Remove(subscriber);
        }
    }

    
    public void Publish(EventWithPriority ev)
    {
        for (int priority = 0; priority <= ev.Priority; priority++)
        {
            if (subscribers.ContainsKey(priority))
            {
                foreach (Action<EventWithPriority> subscriber in subscribers[priority])
                {
                    subscriber(ev);
                }
            }
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        Publisher publisher = new Publisher();

        
        publisher.Subscribe(0, ev => Console.WriteLine($"Low priority event ({ev.Name}): {ev.Data}"));
        publisher.Subscribe(1, ev => Console.WriteLine($"Medium priority event ({ev.Name}): {ev.Data}"));
        publisher.Subscribe(2, ev => Console.WriteLine($"High priority event ({ev.Name}): {ev.Data}"));

        
        publisher.Publish(new EventWithPriority { Name = "Event 1", Priority = 0, Data = "Data for event 1" });
        publisher.Publish(new EventWithPriority { Name = "Event 2", Priority = 1, Data = "Data for event 2" });
        publisher.Publish(new EventWithPriority { Name = "Event 3", Priority = 2, Data = "Data for event 3" });

        Console.ReadKey();
    }
}