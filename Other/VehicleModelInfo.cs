using GTA;
using GTA.Native;

namespace FusionLibrary
{
    public class VehicleModelInfo
    {
        public Hash Hash { get; }
        public Model Model { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public string MakeName { get; }
        public string DisplayMakeName { get; }
        public VehicleClass VehicleClass { get; }
        public VehicleType VehicleType { get; }

        public VehicleModelInfo(Hash hash)
        {
            Hash = hash;
            Model = new Model((int)hash);

            Name = Vehicle.GetModelDisplayName(Model);
            DisplayName = Game.GetLocalizedString(Name);

            MakeName = Vehicle.GetModelMakeName(Model);
            DisplayMakeName = Game.GetLocalizedString(MakeName);

            VehicleClass = Vehicle.GetModelClass(Model);
            VehicleType = Vehicle.GetModelType(Model);
        }

        public override bool Equals(object obj)
        {
            return Hash == ((VehicleModelInfo)obj).Hash;
        }

        public override string ToString()
        {
            return $"{Hash} = ({Name}) {DisplayName}, {DisplayMakeName} - {VehicleType} {VehicleClass}";
        }

        public override int GetHashCode()
        {
            return (int)Hash;
        }
    }
}
