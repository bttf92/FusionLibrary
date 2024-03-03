using GTA;
using System;
using System.IO;
using System.Xml.Serialization;

namespace FusionLibrary
{
    public class TrafficHandler : Script
    {
        public static ModelSwaps ModelSwaps { get; } = new ModelSwaps();

        public static bool Enabled { get; set; } = true;

        private const string ModelSwapFile = "ModelSwaps.xml";

        private static readonly XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModelSwaps));

        public TrafficHandler()
        {
            Tick += TrafficHandler_Tick;
        }

        internal static void Init()
        {
            if (File.Exists(ModelSwapFile))
                Load();

            var models = Vehicle.GetAllModelValues();

            for (int i = 0; i < models.Length; i++)
                FusionUtils.AllVehiclesModels.Add(new VehicleModelInfo((GTA.Native.Hash)models[i]));
        }

        private void TrafficHandler_Tick(object sender, EventArgs e)
        {
            if (Game.WasCheatStringJustEntered("savetraffic"))
                Save();

            if (!Enabled)
                return;

            foreach (ModelSwap modelSwap in ModelSwaps)
                modelSwap.Process();
        }

        public static void Save(string path = ModelSwapFile)
        {
            if (File.Exists(path))
                File.Move(path, path.Replace(".xml", $"_{DateTime.Now:dd_MM_yyyy_HH_mm_ss}.xml"));

            TextWriter writer = new StreamWriter(path);
            xmlSerializer.Serialize(writer, ModelSwaps);
            writer.Close();
        }

        public static void Load(string path = ModelSwapFile)
        {
            TextReader reader = new StreamReader(path);
            ModelSwaps modelSwaps = (ModelSwaps)xmlSerializer.Deserialize(reader);
            reader.Close();

            foreach (ModelSwap modelSwap in modelSwaps)
            {
                if (!ModelSwaps.Contains(modelSwap))
                    ModelSwaps.Add(modelSwap);
            }
        }
    }
}
