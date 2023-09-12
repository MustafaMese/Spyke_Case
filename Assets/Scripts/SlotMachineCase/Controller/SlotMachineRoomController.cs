using System;
using System.Collections.Generic;
using SlotMachineCase.Command;
using SlotMachineCase.Component;
using SlotMachineCase.Manager;
using SlotMachineCase.Model;
using SlotMachineCase.View;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SlotMachineCase.Controller
{
    public class SlotMachineRoomController : IController
    {
        private SlotMachineRoomView _slotMachineView;
        private SlotMachineRoomModel _slotMachineModel;

        private ObjectLerper _objectLerper;
        private UIPathHandler _uiPathHandler;
        private OutcomeHandler _outcomeHandler;

        public Action OnStartMoving;

        private List<Vector3> _lastPointsAtPaths = new();
        private (string left, string middle, string right)[] _outcomeArray;
        private int _slotIndex;

        private (bool particulePlay, string key) outcomeValueTuple;
        
        public SlotMachineRoomController(
            IModel model, 
            IView view,
            ObjectLerper objectLerper)
        {
            _objectLerper = objectLerper;
            _uiPathHandler = new UIPathHandler();

            _slotIndex = PlayerPrefs.GetInt("slotIndex", 0);
            _outcomeHandler = new OutcomeHandler(_slotIndex >= 100);
            
            _slotMachineModel = (SlotMachineRoomModel) model;
            _slotMachineModel.Initialize();
            
            _slotMachineView = (SlotMachineRoomView)view;
            _slotMachineView.Initialize();
            
            Initialize();
        }
        
        public void Initialize()
        {
            _lastPointsAtPaths.Add(_slotMachineView.LeftPanelPath[^1].position);
            _lastPointsAtPaths.Add(_slotMachineView.MiddlePanelPath[^1].position);
            _lastPointsAtPaths.Add(_slotMachineView.RightPanelPath[^1].position);
            
            _outcomeArray = _outcomeHandler.GetOutcome();
            
            GameManager.Instance.CommandManager.AddCommandListener<SlotMachineStoppedCommand>(Controller_SlotMachineStopped);
            
            OnStartMoving = Controller_OnStartMoving;

            _slotMachineModel.IsMoving.OnValueChanged.AddListener(Model_OnRoomMove);
            
            _slotMachineView.OnRoomsMoving.AddListener(View_MoveRooms);
        }

        private void Controller_SlotMachineStopped(SlotMachineStoppedCommand e)
        {
            _slotMachineModel.IsMoving.Value = false;
            
            _slotIndex++;
            _outcomeHandler.SaveOutcome(_slotIndex);
            ConfigureRoomUILists();

            if (outcomeValueTuple.particulePlay)
            {
                GameManager.Instance.VfxManager.PlayVFX(
                    outcomeValueTuple.key,
                    () =>
                    {
                        GameManager.Instance.CommandManager.InvokeCommand(new SlotMachineCanStartAgainCommand());
                    });
            }
            else
            {
                GameManager.Instance.CommandManager.InvokeCommand(new SlotMachineCanStartAgainCommand());
            }
        }

        private void ConfigureRoomUILists()
        {
            ConfigureRoomUIList(_slotMachineView.LeftPanelRoomList);
            ConfigureRoomUIList(_slotMachineView.MiddlePanelRoomList);
            ConfigureRoomUIList(_slotMachineView.RightPanelRoomList);
        }

        private void ConfigureRoomUIList(List<RoomUI> list)
        {
            for (int i = 0; i < list.Count; i++)
                list[i].ConfigurePathIndex(5);
        }

        private void Controller_OnStartMoving()
        {
            _slotMachineModel.IsMoving.Value = true;
        }

        private void View_MoveRooms()
        {
            var outcome = _outcomeArray[_slotIndex];

            var leftIndex = _slotMachineModel.GetIndex(outcome.left);
            var middleIndex = _slotMachineModel.GetIndex(outcome.middle);
            var rightIndex = _slotMachineModel.GetIndex(outcome.right);

            var extraShortDuration = Random.Range(0.05f, 0.1f);
            var extraLongDuration = 0f;
            bool calculateExtraTime = false;

            if (leftIndex == rightIndex && rightIndex == middleIndex)
            {
                if (_slotMachineModel.GetIndex("Jackpot") == middleIndex)
                {
                    extraLongDuration = 2.25f;
                }
                else
                {
                    extraLongDuration = 1f;
                }
                
                calculateExtraTime = true;
            }

            outcomeValueTuple = (calculateExtraTime, outcome.middle);
            
            MoveLeftRooms(
                leftIndex, 
                _slotMachineView.TourCount, 
                _slotMachineView.Duration);
            
            MoveMiddleRooms(
                middleIndex, 
                _slotMachineView.TourCount,
                _slotMachineView.Duration + extraShortDuration);
            
            MoveRightRooms(
                rightIndex, 
                _slotMachineView.TourCount, 
                _slotMachineView.Duration + extraShortDuration, 
                calculateExtraTime, 
                extraLongDuration);
        }
        
        private void Model_OnRoomMove(bool previousValue, bool currentValue)
        {
            if(!_slotMachineModel.IsMoving.Value)
                return;
                
            GameManager.Instance.CommandManager.InvokeCommand(new MoveRoomsCommand());
        }

        private void MoveLeftRooms(int targetIndex, int tourCount, float pathDuration)
        {
            var lerpList = _uiPathHandler.GetLerpDataList(
                _slotMachineView.LeftPanelRoomList, 
                _slotMachineView.LeftPanelPath, 
                targetIndex, 
                tourCount, 
                pathDuration,
                false,
                false,
                0f,
                OnRoomUIWaypointChanged);

            for (int i = 0; i < lerpList.Count; i++)
                _objectLerper.Lerp(lerpList[i]);
        }

        private void MoveMiddleRooms(int targetIndex, int tourCount, float pathDuration)
        {
           var lerpList = _uiPathHandler.GetLerpDataList(
               _slotMachineView.MiddlePanelRoomList, 
               _slotMachineView.MiddlePanelPath, 
               targetIndex, 
               tourCount, 
               pathDuration,
               false,
               false,
               0f,
               OnRoomUIWaypointChanged);
           
           for (int i = 0; i < lerpList.Count; i++)
               _objectLerper.Lerp(lerpList[i]);
        }

        private void MoveRightRooms(int targetIndex, int tourCount, float pathDuration, bool calculateExtraTime, float extraDuration)
        {
            var lerpList = _uiPathHandler.GetLerpDataList(
                _slotMachineView.RightPanelRoomList, 
                _slotMachineView.RightPanelPath, 
                targetIndex, 
                tourCount, 
                pathDuration,
                true,
                calculateExtraTime,
                extraDuration,
                OnRoomUIWaypointChanged);
            
            for (int i = 0; i < lerpList.Count; i++)
                _objectLerper.Lerp(lerpList[i]);
        }

        private void OnRoomUIWaypointChanged(Transform obj, int index, int pathLength)
        {
            var roomUI = obj.GetComponent<RoomUI>();

            roomUI.CheckPositionForOpacity(_lastPointsAtPaths);

            if (index == 3)
            {
                roomUI.TurnBlur();
            }
            
            if(index == pathLength - 3)
            {
                roomUI.TurnNormal();
            }
        }
    }
}
