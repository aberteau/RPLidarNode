using System;
using System.Collections.Generic;
using NetMQ;
using NetMQ.Sockets;
using RPLidarNode.Shared;

namespace Client.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string topic = String.Empty;
            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345");
                subSocket.Subscribe(topic);
                Console.WriteLine("Subscriber socket connecting...");
                while (true)
                {
                    byte[] bytes = subSocket.ReceiveFrameBytes();

                    IEnumerable<Point> points = SerializationHelper.Deserialize(bytes);

                    foreach (var point in points)
                    {
                        Console.WriteLine($"{point.Angle}\t{point.Distance}\t{point.Quality}");
                    }
                }
            }
        }
    }
}
