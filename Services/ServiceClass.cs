
using System;

namespace Services
{
    public class ServiceClass : MarshalByRefObject
    {
        public void New()
        {
            Console.WriteLine("ServiceClass created.");
        }

        public string VoidCall()
        {
            Console.WriteLine("VoidCall called.");
            return "You are calling the void call on the ServiceClass.";
        }

        public int GetServiceCode()
        {
            return this.GetHashCode();
        }

        public string TimeConsumingRemoteCall()
        {
            Console.WriteLine("TimeConsumingRemoteCall called.");
            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine("Counting: " + i.ToString());
            }

            return "This is a time-consuming call.";
        }
    }
}
