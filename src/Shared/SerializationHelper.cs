using System.Collections.Generic;
using System.IO;

namespace RPLidarNode.Shared
{
    public class SerializationHelper
    {
        public static void Serialize(BinaryWriter writer, IEnumerable<Point> points)
        {
            foreach (Point point in points)
                Serialize(writer, point);
        }

        public static void Serialize(BinaryWriter writer, Point point)
        {
            writer.Write(point.Angle);
            writer.Write(point.Distance);
            writer.Write(point.Quality);
        }

        public static byte[] Serialize(IEnumerable<Point> points)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(memoryStream))
            {
                Serialize(bw, points);
                byte[] bytes = memoryStream.ToArray();
                return bytes;
            }
        }

        public static Point DeserializePoint(BinaryReader reader)
        {
            Point point = new Point();
            point.Angle = reader.ReadSingle();
            point.Distance = reader.ReadSingle();
            point.Quality = reader.ReadByte();
            return point;
        }


        public static IEnumerable<Point> Deserialize(BinaryReader reader)
        {
            IList<Point> points = new List<Point>();
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Point point = DeserializePoint(reader);
                points.Add(point);
            }

            return points;
        }

        public static IEnumerable<Point> Deserialize(byte[] bytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                IEnumerable<Point> points = Deserialize(reader);
                return points;
            }
        }
    }
}
