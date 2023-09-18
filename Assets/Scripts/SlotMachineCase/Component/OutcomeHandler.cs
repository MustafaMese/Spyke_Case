using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlotMachineCase.Component
{
    public class OutcomeHandler
    {
        private int[] _outcomeIntArray;

        private int _index;

        /*
         * Jackpot 0
         * Wild 1
         * Seven 2
         * Bonus 3
         * A 4
         */

        private readonly List<Data> _highPriority = new List<Data>()
        {
            { new(000, 5, 0) },
            { new(111, 6, 0) },
            { new(222, 7, 0) },
            { new(333, 8, 0) },
            { new(444, 9, 0) }
        };

        private readonly List<Data> _lowPriority = new List<Data>()
        {
            { new(314, 13, 0) },
            { new(112, 13, 0) },
            { new(004, 13, 0) },
            { new(134, 13, 0) },
            { new(340, 13, 0) }
        };

        public OutcomeHandler(bool setOutcomeArray)
        {
            // if (setOutcomeArray)
            // {
            //     SetOutcomeArray();
            //     _index = 0;
            // }
            // else
            // {
            //     LoadOutcome();
            // }
            
            SetOutcomeArray();
            _index = 0;
        }

        private void InitializeOutcomeArray()
        {
            _outcomeIntArray = new int[100];
            for (int i = 0; i < _outcomeIntArray.Length; i++)
                _outcomeIntArray[i] = -1;
        }

        public (int, int, int)[] GetOutcome()
        {
            (int, int, int)[] outcome = new (int, int, int)[_outcomeIntArray.Length];

            int[] digitArr = new int[3];
            
            for (int i = 0; i < _outcomeIntArray.Length; i++)
            {
                var value = _outcomeIntArray[i];

                for (int j = 0; j < 3; j++)
                {
                    var digit = value % 10;
                    digitArr[j] = digit;
                    value /= 10;
                }
                
                outcome[i] = (digitArr[0], digitArr[1], digitArr[2]);
            }

            return outcome;
        }


        public void SaveOutcome()
        {
            for (int i = 0; i < _outcomeIntArray.Length; i++)
            {
                PlayerPrefs.SetInt($"outcome{i}", _outcomeIntArray[i]);
            }
        }

        public void LogProbabilities()
        {
            foreach (var data in _highPriority)
            {
                TestProbability(data);
            }

            foreach (var data in _lowPriority)
            {
                TestProbability(data);
            }
        }


        private void LoadOutcome()
        {
            // Load
            for (int i = 0; i < _outcomeIntArray.Length; i++)
            {
                _outcomeIntArray[i] = PlayerPrefs.GetInt($"outcome{i}", -1);
            }

            // Check if it is true
            for (int i = 0; i < _outcomeIntArray.Length; i++)
            {
                if (_outcomeIntArray[i] == -1)
                {
                    SetOutcomeArray();
                    break;
                }
            }

            _index = 0;
        }

        private void SetOutcomeArray()
        {
            InitializeOutcomeArray();

            _index = 0;

            PlaceList(_highPriority, ref _index);

            PlaceList(_lowPriority, ref _index);
        }

        private void TestProbability(Data data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            List<(int min, int max)> periodList = GetPeriodList(100, data);

            var indexList = GetPlacedIndexList(_outcomeIntArray, data.ProbabilityName);

            stringBuilder.AppendLine("------------");
            stringBuilder.AppendLine($"{data.ProbabilityName} {data.ProbabilityValue}%");

            bool isTestPassed = true;

            for (int i = 0; i < periodList.Count; i++)
            {
                var minValue = periodList[i].min;
                var maxValue = periodList[i].max;

                bool found = false;

                string str = "";

                for (int j = 0; j < indexList.Count; j++)
                {
                    var index = indexList[j];

                    if (minValue <= index && index < maxValue)
                    {
                        found = true;

                        str += $"{index} ";
                    }
                }

                isTestPassed = found && isTestPassed;

                string foundString = found ? "Y" : "N";

                stringBuilder.AppendLine($"| {minValue} - {maxValue} | {str} | {foundString} |");
            }

            stringBuilder.AppendLine("-----------");

            string testOutcome = isTestPassed ? "PASSED" : "FAILED";

            stringBuilder.AppendLine($"{testOutcome}");

            Debug.Log(stringBuilder);
        }

        private List<int> GetPlacedIndexList(int[] arr, int name)
        {
            List<int> indexList = new();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Equals(name))
                    indexList.Add(i);
            }

            return indexList;
        }

        private void PlaceList(List<Data> source, ref int mainIndex)
        {
            List<Data> cantPlaced = new List<Data>();

            var list = new List<Data>(source);
            while (list.Count > 0)
            {
                var index = mainIndex % list.Count;

                var indexList = GetPeriod(_outcomeIntArray, 100, list[index]);
                
                if (indexList.Count == 0)
                {
                    cantPlaced.Add(new Data(
                        list[index].ProbabilityName,
                        list[index].ProbabilityValue,
                        list[index].count)
                    );

                    var data = list[index];
                    data.count++;
                    list[index] = data;
                    
                    if (list[index].ProbabilityValue <= list[index].count)
                        list.Remove(list[index]);
                }
                else
                {
                    var rndIndex = indexList[Random.Range(0, indexList.Count)];
                    _outcomeIntArray[rndIndex] = list[index].ProbabilityName;

                    var data = list[index];
                    data.count++;
                    list[index] = data;
                    
                    if (list[index].ProbabilityValue <= list[index].count)
                        list.Remove(list[index]);
                }

                mainIndex++;
            }

            if (cantPlaced.Count != 0)
            {
                var emptyIndexList = GetEmptyIndexes(_outcomeIntArray);

                for (int i = 0; i < emptyIndexList.Count; i++)
                {
                    var data = cantPlaced[i];
                    _outcomeIntArray[emptyIndexList[i]] = data.ProbabilityName;
                }
            }
        }

        private List<(int, int)> GetPeriodList(int maxLevel, Data data)
        {
            List<(int, int)> periodList = new();

            var period = maxLevel / data.ProbabilityValue;

            for (int i = 0; i < data.ProbabilityValue; i++)
            {
                var periodStart = (period + (maxLevel % data.ProbabilityValue == 0 ? 0 : 1)) * i;
                var periodEnd = Mathf.Clamp(periodStart + period, 0, maxLevel);

                periodList.Add((periodStart, periodEnd));
            }

            return periodList;
        }

        private List<int> GetPeriod(int[] arr, int maxLevel, Data data)
        {
            List<int> periodList = new();

            var period = maxLevel / data.ProbabilityValue;
            var periodStart = (period + (maxLevel % data.ProbabilityValue == 0 ? 0 : 1)) * data.count;
            var periodEnd = Mathf.Clamp(periodStart + period, 0, maxLevel);

            for (int i = periodStart; i < periodEnd; i++)
            {
                if (arr[i] == -1)
                    periodList.Add(i);
            }

            return periodList;
        }

        private List<int> GetEmptyIndexes(int[] arr)
        {
            var list = new List<int>();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == -1)
                    list.Add(i);
            }

            return list;
        }

        private struct Data
        {
            public readonly int ProbabilityName;
            public readonly int ProbabilityValue;
            public int count;

            public Data(int probabilityName, int probabilityValue, int count)
            {
                this.ProbabilityName = probabilityName;
                this.ProbabilityValue = probabilityValue;
                this.count = count;
            }
        }
    }
}