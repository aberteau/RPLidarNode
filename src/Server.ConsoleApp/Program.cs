using System;
using System.Collections.Generic;
using System.Linq;
using NetMQ;
using NetMQ.Sockets;
using RPLidar4Net.IO;

namespace RPLidarNode.Server.ConsoleApp
{
    class Program
    {
        private static RPLidarSerialDevice _rpLidar;
        private static PublisherSocket _pubSocket;

        static void Main(string[] args)
        {
            _pubSocket = new PublisherSocket();

            Console.WriteLine("Publisher socket binding...");
            _pubSocket.Options.SendHighWatermark = 1000;
            _pubSocket.Bind("tcp://*:12345");

            StartScan();
        }

        private static void StartScan()
        {
            _rpLidar = new RPLidarSerialDevice("com3");
            //Connect RPLidar
            _rpLidar.Connect();
            //Reset - Not really sure how this is supposed to work, reconnecting USB works too
            //_rpLidar.Reset();
            //Stop motor
            _rpLidar.StopMotor();
            //Get Data Event
            _rpLidar.NewScan += RPLidar_NewScan;
            //Start Scan Thread
            _rpLidar.StartScan();
        }

        private static string GetChrono()
        {
            string chrono = DateTime.Now.ToString("hh:mm:ss.ffff");
            return chrono;
        }

        private static void ConsoleLog(string message)
        {
            string chrono = GetChrono();
            string value = $"{chrono} {message}";
            Console.WriteLine(value);
        }

        private static void RPLidar_NewScan(object sender, NewScanEventArgs eventArgs)
        {
            ConsoleLog("RPLidar_NewScan");

            PublishScan(eventArgs.Points);
        }

        private static void PublishScan(IEnumerable<RPLidar4Net.Core.Point> points)
        {
            ConsoleLog("PublishScan");
            IEnumerable<Shared.Point> nPoints = ToPoints(points);
            PublishScan(nPoints);
        }

        private static IEnumerable<Shared.Point> ToPoints(IEnumerable<RPLidar4Net.Core.Point> points)
        {
            return points.Select(p => ToPoint(p)).ToList();
        }

        private static Shared.Point ToPoint(RPLidar4Net.Core.Point point)
        {
            Shared.Point nPoint = new Shared.Point();
            nPoint.Angle = point.Angle;
            nPoint.Distance = point.Distance;
            nPoint.Quality = point.Quality;
            nPoint.StartFlag = point.StartFlag;
            return nPoint;
        }

        private static void PublishScan(IEnumerable<Shared.Point> points)
        {
            byte[] bytes = Shared.SerializationHelper.Serialize(points);
            _pubSocket.SendFrame(bytes);
        }
    }
}
