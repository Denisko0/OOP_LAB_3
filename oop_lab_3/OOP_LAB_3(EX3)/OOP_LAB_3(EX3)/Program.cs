using System;
using System.Threading;
using System.Timers;


    class Publisher
    {

        public delegate void EventHandler(object sender, EventArgs e);


        public event EventHandler HighPriorityEvent;


        public event EventHandler NormalPriorityEvent;


        public void DoSomething()
        {
            Console.WriteLine("Generating events...");
            Thread.Sleep(1000);


            if (HighPriorityEvent != null)
            {
                HighPriorityEvent(this, EventArgs.Empty);
            }


            if (NormalPriorityEvent != null)
            {
                NormalPriorityEvent(this, EventArgs.Empty);
            }
        }
    }


    class HighPrioritySubscriber
    {
        private readonly Publisher publisher;
        private readonly Random random = new Random();

        public HighPrioritySubscriber(Publisher publisher)
        {
            this.publisher = publisher;

            publisher.HighPriorityEvent += HandleHighPriorityEvent;
        }


        private void HandleHighPriorityEvent(object sender, EventArgs e)
        {
            Console.WriteLine("High priority event handled by {0}", this.GetType().Name);

            if (random.Next(0, 10) < 3)
            {
                throw new Exception("Error handling high priority event");
            }
        }
    }


    class NormalPrioritySubscriber
    {
        private readonly Publisher publisher;
        private readonly Random random = new Random();

        public NormalPrioritySubscriber(Publisher publisher)
        {
            this.publisher = publisher;

            publisher.NormalPriorityEvent += HandleNormalPriorityEvent;
        }


        private void HandleNormalPriorityEvent(object sender, EventArgs e)
        {
            Console.WriteLine("Normal priority event handled by {0}", this.GetType().Name);

            if (random.Next(0, 10) < 3)
            {
                throw new Exception("Error handling normal priority event");
            }
        }
    }
}