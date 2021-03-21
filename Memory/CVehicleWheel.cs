using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.Enums;

namespace FusionLibrary
{
    public class CVehicleWheel
    {
        public Vehicle Vehicle { get; }

        public WheelId WheelID { get; }
        public string BoneName { get; }
        public EntityBone Bone { get; }
        public VehicleBone BoneMemory { get; }

        public Vector3 Position => BoneMemory.OriginalTranslation;

        public bool Left { get; }
        public bool Front { get; }

        public CVehicleWheel(Vehicle vehicle, string boneName, WheelId wheelId)
        {
            Vehicle = vehicle;
            WheelID = wheelId;
            BoneName = boneName;
            Bone = Vehicle.Bones[BoneName];

            Left = wheelId == WheelId.FrontLeft | wheelId == WheelId.RearLeft;
            Front = wheelId == WheelId.FrontLeft | wheelId == WheelId.FrontRight;

            VehicleBone.TryGetForVehicle(vehicle, boneName, out VehicleBone vehicleBone);

            BoneMemory = vehicleBone;
        }

        public void Reset()
        {
            BoneMemory.ResetRotation();
            BoneMemory.ResetTranslation();
        }

        public bool Burst
        {
            get => Function.Call<bool>(Hash.IS_VEHICLE_TYRE_BURST, Vehicle, (int)WheelID, true);
            set
            {
                if (value)
                    Vehicle.Wheels[(int)WheelID].Burst();
                else
                    Vehicle.Wheels[(int)WheelID].Fix();
            }
        }

        /// <summary>
        /// Gets the position relative to the <see cref="Vehicle"/> of an offset relative this <see cref="CVehicleWheel"/>
        /// </summary>
        /// <param name="offset">The offset from this <see cref="EntityBone"/>.</param>
        public Vector3 GetRelativeOffsetPosition(Vector3 offset)
        {
            return BoneMemory.Matrix.TransformPoint(offset);
        }

        /// <summary>
        /// Gets the relative offset of this <see cref="CVehicleWheel"/> from an offset from the <see cref="Vehicle"/>
        /// </summary>
        /// <param name="entityOffset">The <see cref="Entity"/> offset.</param>
        public Vector3 GetRelativePositionOffset(Vector3 entityOffset)
        {
            return BoneMemory.Matrix.InverseTransformPoint(entityOffset);
        }

        /// <summary>
        /// Gets the position in world coordinates of an offset relative this <see cref="CVehicleWheel"/>
        /// </summary>
        /// <param name="offset">The offset from this <see cref="CVehicleWheel"/>.</param>
        public Vector3 GetOffsetPosition(Vector3 offset)
        {
            return Vehicle.Matrix.TransformPoint(BoneMemory.Matrix.TransformPoint(offset));
        }

        /// <summary>
        /// Gets the relative offset of this <see cref="CVehicleWheel"/> from a world coordinates position
        /// </summary>
        /// <param name="worldCoords">The world coordinates.</param>
        public Vector3 GetPositionOffset(Vector3 worldCoords)
        {
            return BoneMemory.Matrix.InverseTransformPoint(Vehicle.Matrix.InverseTransformPoint(worldCoords));
        }
    }

    public class CVehicleWheels : IEnumerable
    {
        private List<CVehicleWheel> Wheels { get; } = new List<CVehicleWheel>();

        public int Count => Wheels.Count;

        public int IndexOf(CVehicleWheel wheel) => Wheels.IndexOf(wheel);

        public CVehicleWheels(Vehicle vehicle)
        {
            foreach (string wheel in Utils.WheelsBonesNames)
                if (vehicle.Bones[wheel].Index > 0)
                    Wheels.Add(new CVehicleWheel(vehicle, wheel, Utils.ConvertWheelNameToID(wheel)));
        }

        public CVehicleWheel this[WheelId wheelId] => Wheels.Single(x => x.WheelID == wheelId);
        public CVehicleWheel this[string boneName] => Wheels.Single(x => x.BoneName == boneName);

        public IEnumerator GetEnumerator()
        {
            return new CVehicleWheelCollection(Wheels.ToArray());
        }

        public bool AnyBurst => Wheels.Any(x => x.Burst == true);

        public bool Burst
        {
            get => Wheels.TrueForAll(x => x.Burst);
            set => Wheels.ForEach(x => x.Burst = value);
        }
    }

    public class CVehicleWheelCollection : IEnumerator
    {
        private CVehicleWheel[] cVehicleWheels;

        private int position = -1;

        public CVehicleWheelCollection(CVehicleWheel[] cVehicleWheels)
        {
            this.cVehicleWheels = cVehicleWheels;
        }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            position++;
            return (position < cVehicleWheels.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        public CVehicleWheel Current
        {
            get
            {
                try
                {
                    return cVehicleWheels[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
    }
}
