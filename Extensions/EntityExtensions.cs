using FusionLibrary.Memory;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using static FusionLibrary.FusionEnums;
#pragma warning disable CS0618 // Type or member is obsolete

namespace FusionLibrary.Extensions
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Checks if given <paramref name="entity"/> is not <c>null</c> and exists in game's world.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="entity"/> is not <c>null</c> and exists; otherwise <see langword="false"/></returns>
        public static bool NotNullAndExists(this Entity entity)
        {
            return entity != null && entity.Exists();
        }

        /// <summary>
        /// Returns the <see cref="FusionLibrary.Decorator"/> object of the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <returns>Instance of the <see cref="FusionLibrary.Decorator"/> of <paramref name="entity"/></returns>
        public static Decorator Decorator(this Entity entity)
        {
            return new Decorator(entity);
        }

        /// <summary>
        /// Returns the <see cref="Vector3"/>'s relative velocity of the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <returns><see cref="Vector3"/>'s relative velocity of <paramref name="entity"/></returns>
        public static Vector3 RelativeVelocity(this Entity entity)
        {
            return Function.Call<Vector3>(Hash.GET_ENTITY_SPEED_VECTOR, entity, true);
        }

        /// <summary>
        /// Gets the <paramref name="entity"/> running direction.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <returns><see cref="FusionEnums.RunningDirection"/></returns>
        public static RunningDirection RunningDirection(this Entity entity)
        {
            float vel = entity.RelativeVelocity().Y;

            if (vel > 0)
            {
                return FusionEnums.RunningDirection.Forward;
            }
            else if (vel < 0)
            {
                return FusionEnums.RunningDirection.Backward;
            }

            return FusionEnums.RunningDirection.Stop;
        }

        /// <summary>
        /// Returns the 2D squared distance between <paramref name="src"/> and <paramref name="entity"/>.
        /// </summary>
        /// <param name="src">Instance of an <see cref="Entity"/>.</param>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <returns>Distance in <c>float</c> between the entities</returns>
        public static float DistanceToSquared2D(this Entity src, Entity entity)
        {
            return FusionUtils.DistanceToSquared2D(src, entity);
        }

        /// <summary>
        /// Returns the 2D squared distance between <paramref name="src"/> and <paramref name="boneName"/> of <paramref name="entity"/>.
        /// </summary>
        /// <param name="src">Instance of an <see cref="Entity"/>.</param>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <param name="boneName">Bone's name.</param>
        /// <returns>Distance in <c>float</c> between the entities</returns>
        public static float DistanceToSquared2D(this Entity src, Entity entity, string boneName)
        {
            return src.Position.DistanceToSquared2D(entity.Bones[boneName].Position);
        }

        /// <summary>
        /// Returns the 2D squared distance between <paramref name="src"/> and <paramref name="worldPosition"/>.
        /// </summary>
        /// <param name="src">Instance of an <see cref="Entity"/>.</param>
        /// <param name="worldPosition">Instance of a <see cref="Vector3"/>.</param>
        /// <returns>Distance in <c>float</c></returns>
        public static float DistanceToSquared2D(this Entity src, Vector3 worldPosition)
        {
            return src.Position.DistanceToSquared2D(worldPosition);
        }

        /// <summary>
        /// Checks if the distance between <paramref name="src"/> and <paramref name="entity"/> is less or equal than <paramref name="maxDistance"/>.
        /// </summary>
        /// <param name="src">Instance of an <see cref="Entity"/>.</param>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <param name="maxDistance">Max distance allowed.</param>
        /// <returns><see langword="true"/> if distance is less or equal than <paramref name="maxDistance"/>; otherwise <see langword="false"/></returns>
        public static bool DistanceToSquared2D(this Entity src, Entity entity, float maxDistance)
        {
            return DistanceToSquared2D(src, entity) <= maxDistance * maxDistance;
        }

        /// <summary>
        /// Checks if the distance between <paramref name="src"/> and <paramref name="boneName"/> of <paramref name="entity"/> is less or equal than <paramref name="maxDistance"/>.
        /// </summary>
        /// <param name="src">Instance of an <see cref="Entity"/>.</param>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <param name="boneName">Bone's name.</param>
        /// <param name="maxDistance">Max distance allowed.</param>
        /// <returns><see langword="true"/> if distance is less or equal than <paramref name="maxDistance"/>; otherwise <see langword="false"/></returns>
        public static bool DistanceToSquared2D(this Entity src, Entity entity, string boneName, float maxDistance)
        {
            return DistanceToSquared2D(src, entity, boneName) <= maxDistance * maxDistance;
        }

        /// <summary>
        /// Checks if the distance between <paramref name="src"/> and <paramref name="worldPosition"/> is less or equal than <paramref name="maxDistance"/>.
        /// </summary>
        /// <param name="src">Instance of an <see cref="Entity"/>.</param>
        /// <param name="worldPosition">Instance of a <see cref="Vector3"/>.</param>
        /// <param name="maxDistance">Max distance allowed.</param>
        /// <returns><see langword="true"/> if distance is less or equal than <paramref name="maxDistance"/>; otherwise <see langword="false"/></returns>
        public static bool DistanceToSquared2D(this Entity src, Vector3 worldPosition, float maxDistance)
        {
            return DistanceToSquared2D(src, worldPosition) <= maxDistance * maxDistance;
        }

        /// <summary>
        /// Checks if <paramref name="ped"/> is not <c>null</c>, <seealso cref="Ped.Exists()"/> and also is alive.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="ped"/> is not <c>null</c>, <seealso cref="Ped.Exists()"/> and also is alive; otherwise <see langword="false"/></returns>
        public static bool ExistsAndAlive(this Ped ped)
        {
            return ped.NotNullAndExists() && ped.IsAlive;
        }

        /// <summary>
        /// Checks if <paramref name="ped"/> has been damaged.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="ped"/> has been damaged; otherwise <see langword="false"/></returns>
        public static bool HasBeenDamaged(this Ped ped)
        {
            return Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_OBJECT, ped) || Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_PED, ped) || Function.Call<bool>(Hash.HAS_ENTITY_BEEN_DAMAGED_BY_ANY_VEHICLE, ped);
        }

        /// <summary>
        /// Returns the <see cref="AlphaLevel"/> of the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <returns><see cref="AlphaLevel"/> of <paramref name="entity"/></returns>
        public static AlphaLevel GetAlpha(this Entity entity)
        {
            int value = Function.Call<int>(Hash.GET_ENTITY_ALPHA, entity);

            if (value < (int)AlphaLevel.L1)
            {
                return AlphaLevel.L0;
            }

            if (value < (int)AlphaLevel.L2)
            {
                return AlphaLevel.L1;
            }

            if (value < (int)AlphaLevel.L3)
            {
                return AlphaLevel.L2;
            }

            if (value < (int)AlphaLevel.L4)
            {
                return AlphaLevel.L3;
            }

            if (value < (int)AlphaLevel.L5)
            {
                return AlphaLevel.L4;
            }

            return AlphaLevel.L5;
        }

        /// <summary>
        /// Sets the <see cref="AlphaLevel"/> of the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <param name="level">Desired <see cref="AlphaLevel"/>.</param>
        public static void SetAlpha(this Entity entity, AlphaLevel level)
        {
            Function.Call(Hash.SET_ENTITY_ALPHA, entity, (int)level);
        }

        public static AlphaLevel DecreaseAlpha(this Entity entity)
        {
            AlphaLevel alpha = entity.GetAlpha();

            if (alpha == AlphaLevel.L0)
                return alpha;

            alpha = alpha.Prev();

            entity.SetAlpha(alpha);

            return alpha;
        }

        public static AlphaLevel IncreaseAlpha(this Entity entity)
        {
            AlphaLevel alpha = entity.GetAlpha();

            if (alpha == AlphaLevel.L5)
                return alpha;

            alpha = alpha.Next();

            entity.SetAlpha(alpha);

            return alpha;
        }

        /// <summary>
        /// Checks if <paramref name="taskType"/> is running for <paramref name="ped"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <param name="taskType">A <see cref="TaskType"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="taskType"/> is active; otherwise <see langword="false"/>.</returns>
        public static bool IsTaskActive(this Ped ped, TaskType taskType)
        {
            return Function.Call<bool>(Hash.GET_IS_TASK_ACTIVE, ped, (int)taskType);
        }

        /// <summary>
        /// Checks if <paramref name="ped"/> has any task active.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns><see langword="true"/> if a task is active; otherwise <see langword="false"/>.</returns>
        public static bool IsAnyTaskActive(this Ped ped)
        {
            foreach (TaskType taskType in Enum.GetValues(typeof(TaskType)).Cast<TaskType>().ToList())
            {
                if (ped.IsTaskActive(taskType))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a list of current active tasks of <paramref name="ped"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns>List of <see cref="TaskType"/>.</returns>
        public static List<TaskType> GetActiveTasks(this Ped ped)
        {
            List<TaskType> ret = new List<TaskType>();

            foreach (TaskType taskType in Enum.GetValues(typeof(TaskType)).Cast<TaskType>().ToList())
            {
                if (ped.IsTaskActive(taskType))
                {
                    ret.Add(taskType);
                }
            }

            return ret;
        }

        /// <summary>
        /// Rerturns the closest <see cref="Vehicle"/> in <paramref name="radius"/> of <paramref name="ped"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <param name="radius">Radius of the area to check.</param>
        /// <returns>Instance of the closest <see cref="Vehicle"/>.</returns>
        public static Vehicle GetClosestVehicle(this Ped ped, float radius = 10)
        {
            return World.GetClosestVehicle(ped.Position, radius);
        }

        /// <summary>
        /// Returns the <see cref="Vehicle"/> that the <paramref name="ped"/> is currently entering.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns>Instance of the <see cref="Vehicle"/>.</returns>
        public static Vehicle GetEnteringVehicle(this Ped ped)
        {
            return Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_ENTERING, ped);
        }

        /// <summary>
        /// Returns the <see cref="Vehicle"/> that the <paramref name="ped"/> is currently using.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns>Instance of the <see cref="Vehicle"/>.</returns>
        public static Vehicle GetUsingVehicle(this Ped ped)
        {
            return Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_USING, ped);
        }

        /// <summary>
        /// Whether the vehicle is on front of any other vehicle
        /// </summary>
        /// <param name="veh"></param>
        /// <param name="vehicle"></param>
        /// <param name="by"></param>
        /// <returns><see langword="true"/> if vehicle is on front; otherwise <see langword="false"/>.</returns>
        public static bool IsBehind(this Vehicle veh, Vehicle vehicle, float by = 5)
        {
            bool sameDirection = veh.SameDirection(vehicle, by);

            Vector3 offset = veh.GetPositionOffset(vehicle.Position);

            if (sameDirection)
            {
                return offset.Y > 0;
            }
            else
            {
                return offset.Y < 0;
            }
        }

        /// <summary>
        /// To be used mostly with trains or any vehicles on tracks. Checks if a vehicle is going torwards a second vehicle.
        /// </summary>
        /// <param name="veh">First vehicle.</param>
        /// <param name="vehicle">Second vehicle.</param>
        /// <param name="by">Delta permitted when checking direction of vehicles.</param>
        /// <returns><see langword="true"/> if <paramref name="veh"/> is goiung torwards <paramref name="vehicle"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsGoingTorwards(this Vehicle veh, Vehicle vehicle, float by = 5)
        {
            if (veh.SameDirection(vehicle, by))
            {
                if (veh.IsBehind(vehicle))
                {
                    return veh.RunningDirection() == FusionEnums.RunningDirection.Forward;
                }
                else
                {
                    return veh.RunningDirection() == FusionEnums.RunningDirection.Backward;
                }
            }

            if (veh.IsBehind(vehicle))
            {
                return veh.RunningDirection() == FusionEnums.RunningDirection.Backward;
            }

            return veh.RunningDirection() == FusionEnums.RunningDirection.Forward;
        }

        /// <summary>
        /// Returns the wheel positions of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns>List of <see cref="Vector3"/>.</returns>
        internal static List<Vector3> GetWheelsPositions(this Vehicle vehicle)
        {
            return FusionUtils.WheelsBonesNames.Select(x => vehicle.Bones[x].Position).ToList();
        }

        /// <summary>
        /// Checks if <paramref name="vehicle"/> is on rail tracks.
        /// </summary>
        /// <param name="vehicle">Instance of <see cref="Vehicle"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="vehicle"/> is on rail tracks; otherwise <see langword="false"/>.</returns>
        public static bool IsOnTracks(this Vehicle vehicle)
        {
            return vehicle.Wheels.Select(x => x.LastContactPosition).ToList().TrueForAll(x => FusionUtils.IsWheelOnTracks(x, vehicle));
        }

        /// <summary>
        /// Check if any door of the <paramref name="vehicle"/> is open.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns><see langword="true"/> if there is at least a door open; otherwise <see langword="false"/>.</returns>
        public static bool IsAnyDoorOpen(this Vehicle vehicle)
        {
            foreach (VehicleDoor door in vehicle.Doors)
            {
                if (door.IsOpen)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Replaces the <paramref name="vehicle"/>'s <see cref="Model"/> with new <paramref name="model"/>. WARNING: It does a looped <see cref="Script.Yield"/> in order to wait the new spawned <see cref="Vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instace of an existing vehicle</param>
        /// <param name="model">Target model.</param>
        /// <returns>Instance of the new spawned <see cref="Vehicle"/>.</returns>
        public static Vehicle Replace(this Vehicle vehicle, Model model)
        {
            VehicleReplica vehicleReplica = new VehicleReplica(vehicle);

            vehicle.DeleteCompletely();

            vehicleReplica.Model = model;

            Vehicle newVehicle = vehicleReplica.Spawn(SpawnFlags.Default | SpawnFlags.NoWheels);

            while (!newVehicle.NotNullAndExists())
                Script.Yield();

            newVehicle.PlaceOnGround();

            //newVehicle.AddBlip();

            if (newVehicle.Driver.NotNullAndExists())
                newVehicle.Driver.Task.CruiseWithVehicle(newVehicle, 30);

            foreach (Ped ped in newVehicle.Occupants)
                ped?.MarkAsNoLongerNeeded();

            newVehicle.MarkAsNoLongerNeeded();

            return newVehicle;
        }

        /// <summary>
        /// Sets wheel with <paramref name="id"/> of <paramref name="vehicle"/> at given <paramref name="height"/>.
        /// </summary>
        /// <param name="vehicle"><see cref="Vehicle"/> owner of the wheel.</param>
        /// <param name="id"><see cref="VehicleWheelBoneId"/> of the wheel.</param>
        /// <param name="height">Height of the wheel.</param>
        public static void LiftUpWheel(this Vehicle vehicle, VehicleWheelBoneId id, float height)
        {
            Function.Call(Hash.SET_HYDRAULIC_SUSPENSION_RAISE_FACTOR, vehicle, vehicle.Wheels[id].Index, height);
        }

        /// <summary>
        /// Sets <paramref name="vehicleWheel"/> at given <paramref name="height"/>.
        /// </summary>        
        /// <param name="vehicleWheel">Instance of a <see cref="VehicleWheel"/>.</param>
        /// <param name="height">Height of the wheel.</param>
        public static void LiftUpWheel(this VehicleWheel vehicleWheel, float height)
        {
            Function.Call(Hash.SET_HYDRAULIC_SUSPENSION_RAISE_FACTOR, vehicleWheel.Vehicle, vehicleWheel.Index, height);
        }

        /// <summary>
        /// Attraches <paramref name="vehicle"/> to <paramref name="trailer"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="trailer">Instance of a trailer <see cref="Vehicle"/>.</param>
        /// <param name="radius">Radius for the attach.</param>
        public static void AttachToTrailer(this Vehicle vehicle, Vehicle trailer, float radius)
        {
            Function.Call(Hash.ATTACH_VEHICLE_TO_TRAILER, vehicle, trailer, radius);
        }

        /// <summary>
        /// Gets the street's informations from <see cref="Vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns><see cref="Hash"/> of the street and crossing, and names of them.</returns>
        public static (Hash Street, string StreetName, Hash Crossing, string CrossingStreetName) GetStreetInfo(this Vehicle vehicle)
        {
            Hash street;
            Hash cross;

            unsafe
            {
                Function.Call(Hash.GET_STREET_NAME_AT_COORD, vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, &street, &cross);
            }

            string streetName = World.GetStreetName(vehicle.Position, out string crossName);

            return (street, streetName, cross, crossName);
        }

        /// <summary>
        /// (DO NOT USE) Kept for legacy reasons. Use instead <see cref="EntityExtensions.GetStreetInfo(Vehicle)"/>.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        [Obsolete]
        public static Hash GetStreetHash(this Vehicle vehicle)
        {
            Hash street;
            Hash cross;

            unsafe
            {
                Function.Call(Hash.GET_STREET_NAME_AT_COORD, vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, &street, &cross);
            }

            return street;
        }

        /// <summary>
        /// (DO NOT USE) Kept for legacy reasons. Use instead <see cref="EntityExtensions.GetStreetInfo(Vehicle)"/>.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <returns></returns>
        [Obsolete]
        public static Hash GetCrossingHash(this Vehicle vehicle)
        {
            Hash street;
            Hash cross;

            unsafe
            {
                Function.Call(Hash.GET_STREET_NAME_AT_COORD, vehicle.Position.X, vehicle.Position.Y, vehicle.Position.Z, &street, &cross);
            }

            return cross;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FusionLibrary.TaskDrive"/> of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns>New <see cref="FusionLibrary.TaskDrive"/> instance.</returns>
        public static TaskDrive TaskDrive(this Vehicle vehicle)
        {
            return new TaskDrive(vehicle);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FusionLibrary.TaskDrive"/> of <paramref name="ped"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns>New <see cref="FusionLibrary.TaskDrive"/> instance.</returns>
        public static TaskDrive TaskDrive(this Ped ped)
        {
            return new TaskDrive(ped);
        }

        /// <summary>
        /// Creates a new instance of <see cref="FusionLibrary.TaskDrive"/> of <paramref name="ped"/> with <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns>New <see cref="FusionLibrary.TaskDrive"/> instance.</returns>
        public static TaskDrive TaskDrive(this Ped ped, Vehicle vehicle)
        {
            return new TaskDrive(ped, vehicle);
        }

        /// <summary>
        /// Tasks <paramref name="ped"/> to go to <paramref name="position"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <param name="position">Destination of the task.</param>
        /// <param name="speed">Speed.</param>
        /// <param name="heading">End heading of the <paramref name="ped"/>.</param>
        /// <param name="timeout">Timeout of the task. -1 is without timeout.</param>
        /// <param name="distanceToSlide">Margin accepted for arrival at <paramref name="position"/>.</param>
        public static void TaskGoStraightTo(this Ped ped, Vector3 position, float speed, float heading, int timeout = -1, float distanceToSlide = 0)
        {
            Function.Call(Hash.TASK_GO_STRAIGHT_TO_COORD, ped, position.X, position.Y, position.Z, speed, timeout, heading, distanceToSlide);
        }

        /// <summary>
        /// Checks if <paramref name="ped"/> is inside a <see cref="Vehicle"/>, not entering or exiting.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="ped"/> is inside a <see cref="Vehicle"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsFullyInVehicle(this Ped ped)
        {
            return ped.IsSittingInVehicle() && !ped.IsTaskActive(TaskType.EnterVehicle) && !ped.IsTaskActive(TaskType.ExitVehicle);
        }

        /// <summary>
        /// Checks if <paramref name="ped"/> is outside a <see cref="Vehicle"/>, not entering or exiting.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="ped"/> is outside a <see cref="Vehicle"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsFullyOutVehicle(this Ped ped)
        {
            return ped.CurrentVehicle == null && !ped.IsTaskActive(TaskType.ExitVehicle) && !ped.IsTaskActive(TaskType.EnterVehicle);
        }

        /// <summary>
        /// Checks if <paramref name="ped"/> is entering a <see cref="Vehicle"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="ped"/> is entering a <see cref="Vehicle"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsEnteringVehicle(this Ped ped)
        {
            return !ped.IsSittingInVehicle() && ped.IsTaskActive(TaskType.EnterVehicle) && !ped.IsTaskActive(TaskType.ExitVehicle);
        }

        /// <summary>
        /// Checks if <paramref name="ped"/> is exiting a <see cref="Vehicle"/>.
        /// </summary>
        /// <param name="ped">Instance of a <see cref="Ped"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="ped"/> is exiting a <see cref="Vehicle"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsLeavingVehicle(this Ped ped)
        {
            return !IsFullyOutVehicle(ped) && ped.IsTaskActive(TaskType.ExitVehicle);
        }

        /// <summary>
        /// Attaches <paramref name="entity1"/> to <paramref name="boneName"/> of <paramref name="toEntity"/>.
        /// </summary>
        /// <param name="entity1">Instance of an <see cref="Entity"/>.</param>
        /// <param name="toEntity">Second instance of an <see cref="Entity"/>.</param>
        /// <param name="boneName">Bone's name.</param>
        /// <param name="offset">Offset of attach point.</param>
        /// <param name="rotation">Rotation for attach.</param>
        /// <param name="useFixedRot">If <see langword="false"/> it ignores <paramref name="toEntity"/> vector.</param>
        public static void AttachTo(this Entity entity1, Entity toEntity, string boneName, Vector3 offset, Vector3 rotation, bool useFixedRot = true)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, entity1, toEntity, toEntity.Bones[boneName].Index, offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z, false, false, true, false, 2, useFixedRot);
        }

        /// <summary>
        /// Attaches <paramref name="entity1"/> to <paramref name="toEntity"/>.
        /// </summary>
        /// <param name="entity1">Instance of an <see cref="Entity"/>.</param>
        /// <param name="toEntity">Second instance of an <see cref="Entity"/>.</param>
        /// <param name="offset">Offset of attach point.</param>
        /// <param name="rotation">Rotation for attach.</param>
        /// <param name="useFixedRot">If <see langword="false"/> it ignores <paramref name="toEntity"/> vector.</param>
        public static void AttachTo(this Entity entity1, Entity toEntity, Vector3 offset, Vector3 rotation, bool useFixedRot = true)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, entity1, toEntity, 0, offset.X, offset.Y, offset.Z, rotation.X, rotation.Y, rotation.Z, false, false, true, false, 2, useFixedRot);
        }

        /// <summary>
        /// Attaches physically <paramref name="entity1"/> to <paramref name="toEntity"/>.
        /// </summary>
        /// <param name="entity1">Instance of an <see cref="Entity"/>.</param>
        /// <param name="toEntity">Second instance of an <see cref="Entity"/>.</param>
        /// <param name="offset">Offset of attach point.</param>
        /// <param name="rotation">Rotation for attach.</param>
        public static void AttachToPhysically(this Entity entity1, Entity toEntity, Vector3 offset, Vector3 rotation)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, entity1, toEntity, 0, 0, offset.X, offset.Y, offset.Z, 0, 0, 0, rotation.X, rotation.Y, rotation.Z, 1000000.0f, true, true, false, false, 2);
        }

        /// <summary>
        /// Attaches physically <paramref name="entity1"/> to <paramref name="boneIndex"/> of <paramref name="toEntity"/>.
        /// </summary>
        /// <param name="entity1">Instance of an <see cref="Entity"/>.</param>
        /// <param name="toEntity">Second instance of an <see cref="Entity"/>.</param>
        /// <param name="boneIndex">Bone index of <paramref name="toEntity"/>.</param>
        /// <param name="offset">Offset of attach point.</param>
        /// <param name="rotation">Rotation for attach.</param>
        public static void AttachToPhysically(this Entity entity1, Entity toEntity, int boneIndex, Vector3 offset, Vector3 rotation)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, entity1, toEntity, boneIndex, 0, offset.X, offset.Y, offset.Z, 0, 0, 0, rotation.X, rotation.Y, rotation.Z, 1000000.0f, true, true, false, false, 2);
        }

        /// <summary>
        /// Attaches physically <paramref name="entity1"/> to <paramref name="boneName"/> of <paramref name="toEntity"/>.
        /// </summary>
        /// <param name="entity1">Instance of an <see cref="Entity"/>.</param>
        /// <param name="toEntity">Second instance of an <see cref="Entity"/>.</param>
        /// <param name="boneName">Bone name of <paramref name="toEntity"/>.</param>
        /// <param name="offset">Offset of attach point.</param>
        /// <param name="rotation">Rotation for attach.</param>
        public static void AttachToPhysically(this Entity entity1, Entity toEntity, string boneName, Vector3 offset, Vector3 rotation)
        {
            AttachToPhysically(entity1, toEntity, toEntity.Bones[boneName].Index, offset, rotation);
        }

        /// <summary>
        /// Checks if <paramref name="entity"/> is entirely inside the <paramref name="garage"/>.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <param name="garage"><see cref="GarageDoor"/> hash.</param>
        /// <returns><see langword="true"/> if <paramref name="entity"/> is entirely inside <paramref name="garage"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsEntirelyInGarage(this Entity entity, GarageDoor garage)
        {
            return Function.Call<bool>(Hash.IS_OBJECT_ENTIRELY_INSIDE_GARAGE, garage, entity, 0, 0);
        }

        /// <summary>
        /// Checks if <paramref name="entity"/> is partially inside the <paramref name="garage"/>.
        /// </summary>
        /// <param name="entity">Instance of an <see cref="Entity"/>.</param>
        /// <param name="garage"><see cref="GarageDoor"/> hash.</param>
        /// <returns><see langword="true"/> if <paramref name="entity"/> is partially inside <paramref name="garage"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsPartiallyInGarage(this Entity entity, GarageDoor garage)
        {
            return Function.Call<bool>(Hash.IS_OBJECT_PARTIALLY_INSIDE_GARAGE, garage, entity, 0);
        }

        /// <summary>
        /// Toggles reduced grip for <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="state">State of the toggle.</param>
        public static void SetReduceGrip(this Vehicle vehicle, bool state)
        {
            Function.Call(Hash.SET_VEHICLE_REDUCE_GRIP, vehicle, state);
        }

        /// <summary>
        /// Gets the kinetic enegery of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns>Kinetic enegery of <paramref name="vehicle"/>.</returns>
        public static float GetKineticEnergy(this Vehicle vehicle)
        {
            return 0.5f * HandlingData.GetByVehicleModel(vehicle.Model).Mass * (float)Math.Pow(vehicle.Speed, 2);
        }

        /// <summary>
        /// Clones the <paramref name="vehicle"/> using <see cref="VehicleReplica"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns>Instance of <see cref="VehicleReplica"/>.</returns>
        public static VehicleReplica Clone(this Vehicle vehicle)
        {
            return new VehicleReplica(vehicle);
        }

        /// <summary>
        /// Teleports <paramref name="vehicle"/> to the <paramref name="position"/>, maintaining the height from ground.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="position">Destination position.</param>
        public static void TeleportTo(this Vehicle vehicle, Vector3 position)
        {
            position = vehicle.Position.TransferHeight(position);

            position.RequestCollision();
            vehicle.Position = position;
        }

        /// <summary>
        /// Toggles visible, collisions and engine states of <paramref name="vehicle"/>. Hides\unhides also passengers.
        /// </summary>
        /// <param name="vehicle">Instance of a <paramref name="vehicle"/>.</param>
        /// <param name="isVisible">State.</param>
        public static void SetVisible(this Vehicle vehicle, bool isVisible)
        {
            vehicle.IsVisible = isVisible;
            vehicle.IsCollisionEnabled = isVisible;
            vehicle.IsPositionFrozen = !isVisible;
            vehicle.IsEngineRunning = isVisible;

            foreach (Ped ped in vehicle.Occupants)
            {
                ped.IsVisible = isVisible;
                ped.CanBeDraggedOutOfVehicle = isVisible;
            }
        }

        /// <summary>
        /// Checks if <paramref name="vehicle"/> is being driven by <see cref="Player"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="vehicle"/> is being driven by <see cref="Player"/>; otherwise <see langword="false"/>.</returns>
        public static bool IsPlayerDriving(this Vehicle vehicle)
        {
            return vehicle.IsFunctioning() && FusionUtils.PlayerVehicle == vehicle && FusionUtils.PlayerPed.SeatIndex == VehicleSeat.Driver;
        }

        //public static bool IsDMC12TimeMachine(this Vehicle vehicle)
        //{
        //    return vehicle.Model == FusionUtils.DMC12 && vehicle.Mods[VehicleModType.TrimDesign].Index != 0;
        //}

        /// <summary>
        /// Gets the speed in MPH of the <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns>Speed in MPH.</returns>
        public static float GetMPHSpeed(this Vehicle vehicle)
        {
            return vehicle.Speed.ToMPH();
        }

        /// <summary>
        /// Sets speed in MPH of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="value">Speed in MPH.</param>
        public static void SetMPHSpeed(this Vehicle vehicle, float value)
        {
            vehicle.ForwardSpeed = value.ToMS();
        }

        /// <summary>
        /// Sets lights brightness of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="brightness">Value of brightness.</param>
        public static void SetLightsBrightness(this Vehicle vehicle, float brightness)
        {
            Function.Call(Hash.SET_VEHICLE_LIGHT_MULTIPLIER, vehicle, brightness);
        }

        /// <summary>
        /// Checks if two vehicles are pointed in the same direction.
        /// </summary>
        /// <param name="veh">First instance of a <see cref="Vehicle"/>.</param>
        /// <param name="vehicle">Second instance of a <see cref="Vehicle"/>.</param>
        /// <param name="by"></param>
        /// <returns><see langword="true"/> the direction is the same; otherwise <see langword="false"/>.</returns>
        public static bool SameDirection(this Vehicle veh, Vehicle vehicle, float by = 5)
        {
            return veh.Rotation.Z.Near(vehicle.Rotation.Z, by);
        }

        /// <summary>
        /// Deletes <paramref name="vehicle"/> and every <see cref="Ped"/> passenger.
        /// </summary>
        /// <param name="vehicle"><see cref="Vehicle"/> to be deleted.</param>
        public static void DeleteCompletely(this Vehicle vehicle)
        {
            if (vehicle.NotNullAndExists())
            {
                foreach (Ped x in vehicle.Occupants)
                {
                    if (x != FusionUtils.PlayerPed)
                    {
                        x?.Delete();
                    }
                }
            }

            vehicle?.Delete();
        }

        /// <summary>
        /// Checks if <paramref name="vehicle"/> is not null, exists and is alive.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns></returns>
        public static bool IsFunctioning(this Vehicle vehicle)
        {
            return vehicle.NotNullAndExists() && vehicle.IsAlive && !vehicle.IsDead;
        }

        /// <summary>
        /// Sets <see cref="LightsMode"/> of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="lightsMode"><see cref="LightsMode"/> to be applied.</param>
        public static void SetLightsMode(this Vehicle vehicle, LightsMode lightsMode)
        {
            Function.Call(Hash.SET_VEHICLE_HEADLIGHT_SHADOWS, vehicle, lightsMode);
            Function.Call(Hash.SET_VEHICLE_LIGHTS, vehicle, lightsMode);
        }

        /// <summary>
        /// Gets if headlights and high beams <paramref name="vehicle"/> are on or off.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="lightsOn"></param>
        /// <param name="highbeamsOn"></param>
        public static void GetLightsState(this Vehicle vehicle, out bool lightsOn, out bool highbeamsOn)
        {
            bool _lightsOn;
            bool _highbeamsOn;

            unsafe
            {
                Function.Call(Hash.GET_VEHICLE_LIGHTS_STATE, vehicle, &_lightsOn, &_highbeamsOn);
            }

            lightsOn = _lightsOn;
            highbeamsOn = _highbeamsOn;
        }


        /// <summary>
        /// Sets if <paramref name="vehicle"/> lights should appear as if player is inside.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="state">State of the lights.</param>
        public static void SetPlayerLights(this Vehicle vehicle, bool state)
        {
            Function.Call((Hash)0xC45C27EF50F36ADC, vehicle, state);
        }

        /// <summary>
        /// Decreases speed of <paramref name="vehicle"/> of <paramref name="by"/> value.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="by">Value substracted to speed.</param>
        /// <returns><see langword="true"/> if speed is almost zero; otherwise <see langword="false"/>.</returns>
        public static bool DecreaseSpeedAndWait(this Vehicle vehicle, float by = 20)
        {
            Vector3 vel = vehicle.RelativeVelocity();

            if (vel.Y >= -2 && vel.Y <= 2)
            {
                return true;
            }

            vehicle.Speed -= by * Game.LastFrameTime;

            return false;
        }

        /// <summary>
        /// Sets <paramref name="train"/>'s cruise <paramref name="speed"/> value (m/s).
        /// </summary>
        /// <param name="train">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="speed">Cruise speed (m/s).</param>
        public static void SetTrainCruiseSpeed(this Vehicle train, float speed)
        {
            if (!train.IsTrain)
            {
                return;
            }

            Function.Call(Hash.SET_TRAIN_CRUISE_SPEED, train, speed);
        }

        /// <summary>
        /// Sets <paramref name="train"/>'s cruise <paramref name="speed"/> value (MPH).
        /// </summary>
        /// <param name="train">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="speed">Cruise speed (MPH).</param>
        public static void SetTrainCruiseMPHSpeed(this Vehicle train, float speed)
        {
            if (!train.IsTrain)
            {
                return;
            }

            train.SetTrainCruiseSpeed(speed.ToMS());
        }

        /// <summary>
        /// Sets <paramref name="train"/>'s <paramref name="speed"/> value (m/s).
        /// </summary>
        /// <param name="train">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="speed">Speed (m/s).</param>
        public static void SetTrainSpeed(this Vehicle train, float speed)
        {
            if (!train.IsTrain)
            {
                return;
            }

            Function.Call(Hash.SET_TRAIN_SPEED, train, speed);
        }

        /// <summary>
        /// Sets <paramref name="train"/>'s <paramref name="speed"/> value (MPH).
        /// </summary>
        /// <param name="train">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="speed">Speed (MPH).</param>
        public static void SetTrainMPHSpeed(this Vehicle train, float speed)
        {
            if (!train.IsTrain)
            {
                return;
            }

            train.SetTrainSpeed(speed.ToMS());
        }

        /// <summary>
        /// Makes <paramref name="train"/> derail.
        /// </summary>
        /// <param name="train">Instance of a <see cref="Vehicle"/>.</param>
        public static void Derail(this Vehicle train)
        {
            if (!train.IsTrain)
            {
                return;
            }

            Function.Call(Hash.SET_RENDER_TRAIN_AS_DERAILED, train, true);
        }

        /// <summary>
        /// Returns the carriage at <paramref name="index"/>'s position of the <paramref name="train"/>. 0 returns the first carriage behind the <paramref name="train"/>.
        /// </summary>
        /// <param name="train">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="index">Position of the carriage.</param>
        /// <returns><see cref="Vehicle"/> instance of the carriage</returns>
        public static Vehicle GetTrainCarriage(this Vehicle train, int index)
        {
            if (!train.IsTrain)
            {
                return null;
            }

            return Function.Call<Vehicle>(Hash.GET_TRAIN_CARRIAGE, train, index);
        }

        /// <summary>
        /// Sets the <paramref name="train"/>'s <paramref name="position"/>.
        /// </summary>
        /// <param name="train">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="position">Instance of a <see cref="Vector3"/>.</param>
        public static void SetTrainPosition(this Vehicle train, Vector3 position)
        {
            if (!train.IsTrain)
            {
                return;
            }

            Function.Call(Hash.SET_MISSION_TRAIN_COORDS, train, position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Gets the original translation of the bone with <paramref name="index"/> in <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="index">Bone's index.</param>
        /// <returns><see cref="Vector3"/> translation of the bone.</returns>
        public static Vector3 GetBoneOriginalTranslation(this Vehicle vehicle, int index)
        {
            if (!vehicle.NotNullAndExists())
            {
                return Vector3.Zero;
            }

            unsafe
            {
                CVehicle* veh = (CVehicle*)vehicle.MemoryAddress;
                NativeVector3 v = veh->inst->archetype->skeleton->skeletonData->bones[index].translation;
                return v;
            }
        }

        /// <summary>
        /// Gets the original rotation of the bone with <paramref name="index"/> in <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="index">Bone's index.</param>
        /// <returns><see cref="Vector3"/> rotation of the bone.</returns>
        public static Quaternion GetBoneOriginalRotation(this Vehicle vehicle, int index)
        {
            if (!vehicle.NotNullAndExists())
            {
                return Quaternion.Zero;
            }

            unsafe
            {
                CVehicle* veh = (CVehicle*)vehicle.MemoryAddress;
                NativeVector4 v = veh->inst->archetype->skeleton->skeletonData->bones[index].rotation;
                return v;
            }

        }

        /// <summary>
        /// Returns index of <paramref name="bone"/> in <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <param name="bone">Name of the bone.</param>
        /// <returns>Bone's index.</returns>
        public static int GetBoneIndex(this Vehicle vehicle, string bone)
        {
            if (!vehicle.NotNullAndExists())
            {
                return -1;
            }

            unsafe
            {
                CVehicle* veh = (CVehicle*)vehicle.MemoryAddress;
                CrSkeletonData* skelData = veh->inst->archetype->skeleton->skeletonData;
                uint boneCount = skelData->bonesCount;

                for (uint i = 0; i < boneCount; i++)
                {
                    if (skelData->GetBoneNameForIndex(i) == bone)
                    {
                        return unchecked((int)i);
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets wheels position of <paramref name="vehicle"/>.
        /// </summary>
        /// <param name="vehicle">Instance of a <see cref="Vehicle"/>.</param>
        /// <returns>List of wheels position</returns>
        public static Dictionary<string, Vector3> GetWheelsPosition(this Vehicle vehicle)
        {
            Dictionary<string, Vector3> ret = new Dictionary<string, Vector3>();

            foreach (string wheel in FusionUtils.WheelsBonesNames)
            {
                if (vehicle.Bones[wheel].Index > 0)
                {
                    ret.Add(wheel, vehicle.Bones[wheel].RelativePosition.GetSingleOffset(Coordinate.Z, -0.05f));
                }
            }

            return ret;
        }
    }
}
