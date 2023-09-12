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
        private string[] _outcomeStringArray;

        private int _index;
        
        private readonly List<Data> _highPriority = new List<Data>()
        {
            { new("Jackpot,Jackpot,Jackpot", 5, 0) },
            { new("Wild,Wild,Wild", 6, 0) },
            { new("Seven,Seven,Seven", 7, 0) },
            { new("Bonus,Bonus,Bonus", 8, 0) },
            { new("A,A,A", 9, 0) }
        };

        private readonly List<Data> _lowPriority = new List<Data>()
        {
            { new("A,Wild,Bonus", 13, 0) },
            { new("Wild,Wild,Seven", 13, 0) },
            { new("Jackpot,Jackpot,A", 13, 0) },
            { new("Wild,Bonus,A", 13, 0) },
            { new("Bonus,A,Jackpot", 13, 0) }
        };

        public OutcomeHandler(bool setOutcomeArray)
        {
            _outcomeStringArray = new string[100];

            if (setOutcomeArray)
            {
                SetOutcomeArray();
                _index = 0;
            }
            else
            {
                LoadOutcome();
            }
        }

        public (string, string, string)[] GetOutcome()
        {
            (string, string, string)[] outcome = new (string, string, string)[_outcomeStringArray.Length];

            for (int i = 0; i < _outcomeStringArray.Length; i++)
            {
                var str = _outcomeStringArray[i];
                string[] strArr = str.Split(',', 3);
                
                outcome[i] = (strArr[0], strArr[1], strArr[2]);
            }

            return outcome;
        }


        public void SaveOutcome(int index)
        {
            for (int i = 0; i < _outcomeStringArray.Length; i++)
            {
                PlayerPrefs.SetString($"outcome{i}", _outcomeStringArray[i]);
            }
        }
        
        public void LogProbabilities()
        {
            foreach (var item in _outcomeStringArray)
            {
                if(item == "Bonus,A,Jackpot")
                    Debug.Log("be");
            }
            
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
            for (int i = 0; i < _outcomeStringArray.Length; i++)
            {
                _outcomeStringArray[i] = PlayerPrefs.GetString($"outcome{i}", "");
            }
            
            // Check if it is true
            for (int i = 0; i < _outcomeStringArray.Length; i++)
            {
                if (_outcomeStringArray[i] == "")
                {
                    SetOutcomeArray();
                    break;
                }
            }

            _index = 0;
        }
        
        private void SetOutcomeArray()
        {
            _outcomeStringArray = new string[100];

            _index = 0;

            PlaceList(_highPriority, ref _index);

            PlaceList(_lowPriority, ref _index);
        }
        
        private void TestProbability(Data data)
        {
            StringBuilder stringBuilder = new StringBuilder();

            List<(int min, int max)> periodList = GetPeriodList(100, data);

            var indexList = GetPlacedIndexList(_outcomeStringArray, data.probabilityName);
            
            stringBuilder.AppendLine("------------");
            stringBuilder.AppendLine($"{data.probabilityName} {data.probabilityValue}%");

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

                    if (minValue <= index && index <= maxValue)
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

        private List<int> GetPlacedIndexList(string[] arr, string name)
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

                List<int> indexList = GetPeriod(_outcomeStringArray, 100, list[index]);

                if (indexList.Count == 0)
                {
                    cantPlaced.Add(new Data(
                        list[index].probabilityName,
                        list[index].probabilityValue,
                        list[index].count)
                    );

                    list[index].count++;

                    if (list[index].probabilityValue <= list[index].count)
                        list.Remove(list[index]);
                }
                else
                {
                    var rndIndex = indexList[Random.Range(0, indexList.Count)];
                    _outcomeStringArray[rndIndex] = list[index].probabilityName;

                    list[index].count++;

                    if (list[index].probabilityValue <= list[index].count)
                        list.Remove(list[index]);
                }

                mainIndex++;
            }

            if (cantPlaced.Count != 0)
            {
                var emptyIndexList = GetEmptyIndexes(_outcomeStringArray);

                for (int i = 0; i < emptyIndexList.Count; i++)
                {
                    var data = cantPlaced[i];
                    _outcomeStringArray[emptyIndexList[i]] = data.probabilityName;
                }
            }
            
            
        }

        private List<(int, int)> GetPeriodList(int maxLevel, Data data)
        {
            List<(int, int)> periodList = new();

            var period = maxLevel / data.probabilityValue;

            for (int i = 0; i < data.probabilityValue; i++)
            {
                var periodStart = (period + 1) * i;
                var periodEnd = Mathf.Clamp(periodStart + period, 0, 99);

                periodList.Add((periodStart, periodEnd));
            }

            return periodList;
        }

        private List<int> GetPeriod(string[] arr, int maxLevel, Data data)
        {
            List<int> periodList = new();

            var period = maxLevel / data.probabilityValue;
            var periodStart = (period + 1) * data.count;
            var periodEnd = Mathf.Clamp(periodStart + period, 0, 99);

            for (int i = periodStart; i < periodEnd; i++)
            {
                if (arr[i] == null)
                    periodList.Add(i);
            }

            return periodList;
        }

        private List<int> GetEmptyIndexes(string[] arr)
        {
            var list = new List<int>();

            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == null)
                    list.Add(i);
            }

            return list;
        }

        private class Data
        {
            public string probabilityName;
            public int probabilityValue;
            public int count;

            public Data(string probabilityName, int probabilityValue, int count)
            {
                this.probabilityName = probabilityName;
                this.probabilityValue = probabilityValue;
                this.count = count;
            }
        }
    }
}