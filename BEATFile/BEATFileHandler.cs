using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BEATLib
{
    public static class BEATFile
    {
        public static void SaveBEATEvents(string filePath, List<BEATEvent> events)
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            using (var writer = new BinaryWriter(stream, Encoding.UTF8))
            {
                // Write metadata or header if needed
                writer.Write("BEAT"); // Example header

                writer.Write(events.Count); // Number of events

                foreach (var e in events)
                {
                    writer.Write(e.GetStartTime());
                    writer.Write(e.GetTotalDuration());
                    writer.Write(e.TrackID);
                    writer.Write(e.ObjectID);
                    writer.Write(e.ActionID);
                    writer.Write(e.ActionDetails);
                    writer.Write(e.GroupID);
                }
            }
        }

        public static List<BEATEvent> LoadBEATEvents(string filePath)
        {
            var events = new List<BEATEvent>();

            using (var stream = new FileStream(filePath, FileMode.Open))
                events=LoadBEATEvents(stream);
            return events;
            //using (var reader = new BinaryReader(stream, Encoding.UTF8))
            //{
            //    // Read metadata or header if needed
            //    string header = reader.ReadString();
            //    if (header != "BEAT")
            //    {
            //        throw new InvalidDataException("Invalid BEAT file format.");
            //    }

            //    int eventCount = reader.ReadInt32();

            //    for (int i = 0; i < eventCount; i++)
            //    {
            //        int startTime = reader.ReadInt32();
            //        int duration = reader.ReadInt32();
            //        int trackID = reader.ReadInt32();
            //        int objectID = reader.ReadInt32();
            //        int actionID = reader.ReadInt32();
            //        string actionDetails = reader.ReadString();
            //        int groupID = reader.ReadInt32();

            //        events.Add(new BEATEvent(startTime, duration, trackID, objectID, actionID, actionDetails, groupID));
            //    }
            //}

            //return events;
        }

        public static List<BEATEvent> LoadBEATEvents(Stream stream)
        {
            var events = new List<BEATEvent>();
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                // Read metadata or header if needed
                string header = reader.ReadString();
                if (header != "BEAT")
                {
                    throw new InvalidDataException("Invalid BEAT file format.");
                }

                int eventCount = reader.ReadInt32();

                for (int i = 0; i < eventCount; i++)
                {
                    int startTime = reader.ReadInt32();
                    int duration = reader.ReadInt32();
                    int trackID = reader.ReadInt32();
                    int objectID = reader.ReadInt32();
                    int actionID = reader.ReadInt32();
                    string actionDetails = reader.ReadString();
                    int groupID = reader.ReadInt32();

                    events.Add(new BEATEvent(startTime, duration, trackID, objectID, actionID, actionDetails, groupID));
                }
            }

            return events;
        }
    }
}
