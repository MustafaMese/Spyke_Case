using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotMachineCase.Component
{
    public class UIPathHandler
    {
        private const int MIDDLE_INDEX = 0;
        private const int TOTAL_OBJECT_COUNT = 5;
        private const int TRANSITION_INDEX_COUNT = 3;

        public List<LerpData> GetLerpDataList(List<RoomUI> list,
            List<Transform> transformPath,
            int targetIndex,
            int tourCount,
            float pathDuration,
            bool isLastList,
            bool calculateExtraTime,
            float extraTime,
            Action<Transform, int, int> OnWaypointChanged)
        {
            List<LerpData> lerpDataList = new();
            
            var pathLength = TOTAL_OBJECT_COUNT * tourCount + (MIDDLE_INDEX - list[targetIndex].PathIndex) % TOTAL_OBJECT_COUNT;

            for (int i = 0; i < list.Count; i++)
            {
                var roomUI = list[i];

                roomUI.PathLength = pathLength;
                
                Vector3[] path = CreatePath(roomUI, transformPath, pathLength);

                LerpData lerpData;
                
                Dictionary<int, float> indexDurations = new();
                if (isLastList)
                {
                    indexDurations = calculateExtraTime
                        ? CalculateDurations(tourCount, path, pathDuration, extraTime)
                        : null;

                    lerpData = new LerpData(roomUI.transform, path, pathDuration, i == list.Count - 1, OnWaypointChanged, indexDurations);
                }
                else
                {
                    lerpData = new LerpData(roomUI.transform, path, pathDuration, false, OnWaypointChanged);
                }

                lerpDataList.Add(lerpData);
            }

            return lerpDataList;
        }
        
        private Dictionary<int, float> CalculateDurations(int tourCount, Vector3[] path, float pathDuration, float extraTime)
        {
            Dictionary<int, float> indexDurations = new();

            var firstPart = TOTAL_OBJECT_COUNT * (tourCount / 2);

            var defaultDuration = pathDuration / path.Length;
            
            // First three default speed
            for (int i = 0; i < firstPart; i++)
            {
                indexDurations.Add(i, defaultDuration);
            }

            // Transition steps.
            var targetDuration = extraTime / (path.Length - firstPart);
            for (int i = firstPart, y = 1; i < firstPart + TRANSITION_INDEX_COUNT; i++, y++)
            {
                var time = Mathf.Lerp(defaultDuration, targetDuration, (1 / TRANSITION_INDEX_COUNT) * y);
                indexDurations.Add(i, time);
            }

            // Last part
            for (int i = firstPart + 3; i < path.Length; i++)
            {
                indexDurations.Add(i, targetDuration);
            }
            
            return indexDurations;
        }

        private Vector3[] CreatePath(RoomUI roomUI, List<Transform> transformPath, int pathLength)
        {
            var path = new Vector3[pathLength];
            
            for (int i = 0; i < pathLength; i++)
            {
                path[i] = transformPath[(roomUI.PathIndex + i) % TOTAL_OBJECT_COUNT].position;
            }

            return path;
        }
    }
}
