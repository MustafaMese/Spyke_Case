using System.Collections.Generic;
using SlotMachineCase.Command;
using SlotMachineCase.Component;
using SlotMachineCase.Manager;
using UnityEngine;
using UnityEngine.Events;

namespace SlotMachineCase.View
{
    public class OnRoomBlurryUnityEvent : UnityEvent {}
    
    public class SlotMachineRoomView : MonoBehaviour, IView
    {
        [HideInInspector] public OnRoomBlurryUnityEvent OnRoomsMoving = new OnRoomBlurryUnityEvent();

        [SerializeField] private int tourCount;
        [SerializeField] private float duration;
        
        [SerializeField] private List<RoomUI> leftPanelRoomList = new();
        [SerializeField] private List<RoomUI> middlePanelRoomList = new();
        [SerializeField] private List<RoomUI> rightPanelRoomList = new();

        [SerializeField] private List<Transform> leftPanelPath = new();
        [SerializeField] private List<Transform> middlePanelPath = new();
        [SerializeField] private List<Transform> rightPanelPath = new();

        public List<RoomUI> LeftPanelRoomList => leftPanelRoomList;
        public List<RoomUI> MiddlePanelRoomList => middlePanelRoomList;
        public List<RoomUI> RightPanelRoomList => rightPanelRoomList;
        
        public List<Transform> LeftPanelPath => leftPanelPath;
        public List<Transform> MiddlePanelPath => middlePanelPath;
        public List<Transform> RightPanelPath => rightPanelPath;

        public float Duration
        {
            get => duration;
        }

        public int TourCount
        {
            get => tourCount;
        }
        
        public void Initialize()
        {
            InitializeRoomUIs();
            
            GameManager.Instance.CommandManager.AddCommandListener<MoveRoomsCommand>(OnMakeRoomBlurryCommand);
        }

        private void InitializeRoomUIs()
        {
            InitializeRoomUIList(leftPanelRoomList);
            InitializeRoomUIList(middlePanelRoomList);
            InitializeRoomUIList(rightPanelRoomList);
        }

        private void InitializeRoomUIList(List<RoomUI> list)
        {
            var totalRoomCount = 5;
            for (int i = 0; i < totalRoomCount; i++)
            {
                var index = i % totalRoomCount;
                list[index].PathIndex = index;
            }
        }

        private void OnMakeRoomBlurryCommand(MoveRoomsCommand e)
        {
            OnRoomsMoving.Invoke();
        }
    }
}
