using GTA;
using System;
using System.IO;
using System.Xml.Serialization;

namespace FusionLibrary
{
    public class TrafficHandler : Script
    {
        public static ModelSwaps ModelSwaps { get; } = new ModelSwaps();

        public static bool Enabled { get; set; }

        public TrafficHandler()
        {
            Tick += TrafficHandler_Tick;
        }

        private void TrafficHandler_Tick(object sender, EventArgs e)
        {
            if (!Enabled)
                return;

            foreach (ModelSwap modelSwap in ModelSwaps)
                modelSwap.Process();
        }

        public static void Save(string path = "ModelSwaps.xml")
        {
            XmlSerializer x = new XmlSerializer(typeof(ModelSwaps));
            TextWriter writer = new StreamWriter(path);
            x.Serialize(writer, ModelSwaps);
            writer.Close();
        }

        public static void Load(string path = "ModelSwaps.xml")
        {
            XmlSerializer x = new XmlSerializer(typeof(ModelSwaps));
            TextReader reader = new StreamReader(path);
            ModelSwaps modelSwaps = (ModelSwaps)x.Deserialize(reader);
            reader.Close();

            foreach (ModelSwap modelSwap in modelSwaps)
                if (!ModelSwaps.Contains(modelSwap))
                    ModelSwaps.Add(modelSwap);
        }
    }
}
