using GTA;
using GTA.Native;
using System.Collections.Generic;
using static FusionLibrary.FusionEnums;

namespace FusionLibrary
{
    public class TaskDrive
    {
        public Ped Ped { get; }

        public Vehicle Vehicle { get; }

        public TaskSequence TaskSequence { get; private set; }

        public int Count
        {
            get
            {
                return DriveActions.Count;
            }
        }

        private readonly List<DriveAction> DriveActions = new List<DriveAction>();

        public bool IsPlaying
        {
            get
            {
                return Ped.TaskSequenceProgress > -1;
            }
        }

        public TaskDrive(Ped ped, Vehicle vehicle)
        {
            Ped = ped;
            Vehicle = vehicle;
        }

        public TaskDrive(Vehicle vehicle)
        {
            Ped = vehicle.Driver;
            Vehicle = vehicle;
        }

        public TaskDrive(Ped ped)
        {
            Ped = ped;
            Vehicle = ped.CurrentVehicle;
        }

        public TaskDrive Create()
        {
            TaskSequence = new TaskSequence();

            return this;
        }

        public TaskDrive Add(DriveAction driveAction, int time)
        {
            if (TaskSequence == null || TaskSequence.IsClosed)
            {
                Create();
            }

            Function.Call(Hash.TASK_VEHICLE_TEMP_ACTION, Ped, Vehicle, (int)driveAction, time);

            DriveActions.Add(driveAction);

            return this;
        }

        public DriveAction Current()
        {
            if (!IsPlaying || Ped.TaskSequenceProgress >= Count)
            {
                return DriveAction.None;
            }

            return DriveActions[Ped.TaskSequenceProgress];
        }

        public void Start()
        {
            Ped.Task.PerformSequence(TaskSequence);
        }

        public void Close()
        {
            TaskSequence.Close();
        }
    }
}
