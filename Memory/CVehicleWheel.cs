using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FusionLibrary
{
    public class CVehicleWheel
    {
        public Vehicle Vehicle { get; }

        public VehicleWheelBoneId WheelID { get; }
        public string BoneName { get; }
        public EntityBone Bone { get; }
        public VehicleBone BoneMemory { get; }

        public Vector3 Position => BoneMemory.OriginalTranslation;

        public bool Left { get; }
        public bool Front { get; }

        public CVehicleWheel(Vehicle vehicle, string boneName, VehicleWheelBoneId wheelId)
        {
            Vehicle = vehicle;
            WheelID = wheelId;
            BoneName = boneName;
            Bone = Vehicle.Bones[BoneName];

            Left = wheelId == VehicleWheelBoneId.WheelLeftFront | wheelId == VehicleWheelBoneId.WheelLeftRear;
            Front = wheelId == VehicleWheelBoneId.WheelLeftFront | wheelId == VehicleWheelBoneId.WheelRightFront;

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
            get => Vehicle.Wheels[WheelID].IsBursted;

            set
            {
                if (value)
                {
                    Vehicle.Wheels[WheelID].Burst();
                }
                else
                {
                    Vehicle.Wheels[WheelID].Fix();
                }
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

        private readonly Vehicle vehicle1;
        private readonly Decorator decorator;

        public int IndexOf(CVehicleWheel wheel)
        {
            return Wheels.IndexOf(wheel);
        }

        public CVehicleWheels(Vehicle vehicle)
        {
            vehicle1 = vehicle;
            decorator = vehicle.Decorator();

            foreach (string wheel in FusionUtils.WheelsBonesNames)
            {
                if (vehicle.Bones[wheel].Index > 0)
                {
                    Wheels.Add(new CVehicleWheel(vehicle, wheel, FusionUtils.ConvertWheelNameToID(wheel)));
                }
            }
        }

        public CVehicleWheel this[VehicleWheelBoneId wheelId] => Wheels.Single(x => x.WheelID == wheelId);

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

        public int ReduceGrip
        {
            get => decorator.Grip;

            set
            {
                if (value < 0)
                    value = 0;
                else if (value > 3)
                    value = 3;

                decorator.Grip = value;

                Function.Call(Hash.SET_VEHICLE_REDUCE_GRIP, vehicle1, value != 0);
                Function.Call(Hash.SET_VEHICLE_REDUCE_GRIP_LEVEL, vehicle1, value);
            }
        }
    }

    public class CVehicleWheelCollection : IEnumerator
    {
        private readonly CVehicleWheel[] cVehicleWheels;

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
