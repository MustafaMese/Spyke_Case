using System.Collections.Generic;
using SlotMachineCase.Command;
using SlotMachineCase.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SlotMachineCase.View
{
    
    public class StartSpinningUnityEvent : UnityEvent {}
    
    public class SlotMachineView : MonoBehaviour, IView
    {
        [HideInInspector] public StartSpinningUnityEvent OnSpinningStart = new StartSpinningUnityEvent();
        
        public void Initialize()
        {
            GameManager.Instance.CommandManager.AddCommandListener<StartSpinningCommand>(OnStartSpinningCommand);
        }

        private void OnStartSpinningCommand(StartSpinningCommand e)
        {
            OnSpinningStart.Invoke();
        }
    }
}
