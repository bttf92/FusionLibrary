﻿using FusionLibrary.Extensions;
using GTA;
using GTA.Math;
using System;

namespace FusionLibrary
{
    public class WaypointScript : Script
    {
        private static bool _loadScene;
        private static Vector3 _position;

        private static bool _loadWaypoint;
        private static bool _loadSceneAfter;
        private static Vector3 _waypointPos = Vector3.Zero;

        public static Vector3 WaypointPosition
        {
            get
            {
                Vector3 tmp = _waypointPos;
                _waypointPos = Vector3.Zero;

                return tmp;
            }
            private set => _waypointPos = value;
        }

        public WaypointScript()
        {
            Tick += WaypointScript_Tick;
        }

        public static void LoadScene(Vector3 position)
        {
            _position = position;
            _loadScene = true;
        }

        public static void LoadWaypointPosition(bool loadSceneAfter = false)
        {
            _waypointPos = Vector3.Zero;
            _loadWaypoint = true;
            _loadSceneAfter = loadSceneAfter;
        }

        private void WaypointScript_Tick(object sender, EventArgs e)
        {
            if (_loadScene)
            {
                _position.LoadScene();

                _loadScene = false;
            }

            if (_loadWaypoint)
            {
                _waypointPos = FusionUtils.GetWaypointPosition();

                _loadWaypoint = false;

                if (_loadSceneAfter)
                {
                    _waypointPos.LoadScene();

                    _loadSceneAfter = false;
                }
            }
        }
    }
}
