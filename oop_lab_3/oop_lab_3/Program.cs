using System;
using System.Collections.Generic;
using System.Threading;

namespace EventDrivenProgramming
{
    public class EventBus
    {

        private readonly Dictionary<Type, List<Delegate>> _handlers = new Dictionary<Type, List<Delegate>>();


        private readonly Dictionary<Type, DateTime> _lastSentTime = new Dictionary<Type, DateTime>();


        private readonly int _eventRateLimitMs;


        public EventBus(int eventRateLimitMs)
        {
            _eventRateLimitMs = eventRateLimitMs;
        }


        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (!_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers = new List<Delegate>();
                _handlers.Add(typeof(TEvent), handlers);
            }

            handlers.Add(handler);
        }


        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
            {
                handlers.Remove(handler);
            }
        }


        public void Publish<TEvent>(TEvent @event)
        {
            var eventType = typeof(TEvent);


            if (_lastSentTime.TryGetValue(eventType, out var lastSentTime))
            {
                var timeSinceLastSent = DateTime.UtcNow - lastSentTime;
                if (timeSinceLastSent.TotalMilliseconds < _eventRateLimitMs)
                {
                    return;
                }
            }


            if (_handlers.TryGetValue(eventType, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    ((Action<TEvent>)handler)(@event);
                }
            }


            _lastSentTime[eventType] = DateTime.UtcNow;
        }
    }

    public class OrderCreatedEvent
    {
        public int OrderId
        {
            get;

        }

        public OrderCreatedEvent(int orderId)
        {
            OrderId = orderId;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            
            var eventBus = new EventBus(eventRateLimitMs: 1000);

            
            eventBus.Subscribe<OrderCreatedEvent>(HandleOrderCreatedEvent1);
            eventBus.Subscribe<OrderCreatedEvent>(HandleOrderCreatedEvent2);

            
            for (int i = 1; i <= 10; i++)
            {
                eventBus.Publish(new OrderCreatedEvent(i));
                Thread.Sleep(500);
            }

            
            eventBus.Unsubscribe<OrderCreatedEvent>(HandleOrderCreatedEvent2);

            
            for (int i = 11; i <= 20; i++)
            {
                eventBus.Publish(new OrderCreatedEvent(i));
                Thread.Sleep(500);
            }
        }

        
        static void HandleOrderCreatedEvent1(OrderCreatedEvent @event)
        {
            Console.WriteLine($"Order created: {@event.OrderId}, handled by Handler 1");
        }

        static void HandleOrderCreatedEvent2(OrderCreatedEvent @event)
        {
            Console.WriteLine($"Order created: {@event.OrderId}, handled by Handler 2");
        }
    }
}
