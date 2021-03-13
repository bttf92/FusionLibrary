using System;

namespace FusionLibrary
{
    public class Enums
    {
        public enum Coordinate
        {
            X,
            Y,
            Z
        }

        [Flags]
        public enum SpawnFlags
        {
            Default = 0,
            WarpPlayer = 1,
            ForcePosition = 2,
            ResetValues = 4,
            Broken = 8,
            ForceReentry = 16,
            CheckExists = 32,
            NoOccupants = 64,
            NoVelocity = 128,
            New = 256
        }

        public enum CameraSwitchType
        {
            Instant,
            Animated
        }

        public enum LightsMode
        {
            Default,
            Disabled,
            AlwaysOn
        }

        public enum MapArea
        {
            County = 2072609373,
            City = -289320599
        }

        public enum WheelId
        {
            FrontLeft = 0,
            FrontRight = 1,
            Middle1Left = 2,
            Middle1Right = 3,
            RearLeft = 4,
            RearRight = 5,
            Middle2Left = 45,
            Middle2Right = 47
        }

        public enum AnimationType
        {
            Offset,
            Rotation
        }

        public enum AnimationStep
        {
            Off,
            First,
            Second,
            Third,
            Fourth,
            Fifth,
            Sixth,
            Seventh
        }
    }
}
