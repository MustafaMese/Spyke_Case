using SlotMachineCase.Component;
using SlotMachineCase.Controller;
using SlotMachineCase.Model;
using SlotMachineCase.View;
using UnityEngine;

namespace SlotMachineCase.Initializer
{
    public class SlotMachineRoomInitializer : MonoBehaviour, IInitializer
    {
        [SerializeField] private SlotMachineRoomView view;
        [SerializeField] private ObjectLerper objectLerper;

        private SlotMachineRoomController _controller;
        
        public void Initialize()
        {
            var model = new SlotMachineRoomModel();

            _controller = new SlotMachineRoomController(model, view, objectLerper);
        }

        public SlotMachineRoomController GetController() => _controller;

    }
}