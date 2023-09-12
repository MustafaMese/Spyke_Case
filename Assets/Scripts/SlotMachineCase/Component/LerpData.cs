using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMachineCase.Component
{
    public struct LerpData
    {
        public Transform obj;
        public Vector3[] path;
        public bool isLast;
        public Dictionary<int, float> indexDurations;

        private readonly float _totalDuration;

        public Action<Transform, int, int> OnWaypointChanged;

        public LerpData(
            Transform obj, 
            Vector3[] path, 
            float totalDuration, 
            bool isLast, 
            Action<Transform, int, int> onWaypointChanged,
            Dictionary<int, float> indexDurations = null) 
        {
            this.obj = obj;
            this.path = path;
            this.isLast = isLast;
            _totalDuration = totalDuration;
            OnWaypointChanged = onWaypointChanged;

            this.indexDurations = indexDurations;
        }

        public float GetIndexDuration(int index)
        {
            if (indexDurations != null && indexDurations.Count > 0)
            {
                return indexDurations[index];
            }
            else
            {
                return _totalDuration / path.Length;
            }
        }
        
        public float GetDurationBetweenTwoPoints(Vector3 point1, Vector3 point2)
        {
            return _totalDuration / path.Length;
        }
    }
}