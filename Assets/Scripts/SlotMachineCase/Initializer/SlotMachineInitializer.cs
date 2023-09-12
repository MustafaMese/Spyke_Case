using System;
using SlotMachineCase.Component;
using SlotMachineCase.Controller;
using SlotMachineCase.Model;
using SlotMachineCase.View;
using UnityEngine;

namespace SlotMachineCase.Initializer
{
    public class SlotMachineInitializer : MonoBehaviour
    {
        [SerializeField] private SlotMachineView slotMachineViewPrefab;
        
        
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            var model = new SlotMachineModel();

            var view = Instantiate(slotMachineViewPrefab);

            var slotMachineRoomInitializer = FindObjectOfType<SlotMachineRoomInitializer>();

            var controller = new SlotMachineController(model, view, slotMachineRoomInitializer);
        }
    }
}
