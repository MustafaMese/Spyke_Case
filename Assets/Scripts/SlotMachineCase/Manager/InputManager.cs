using System;
using SlotMachineCase.Command;
using UnityEngine;

namespace SlotMachineCase.Manager
{
    public class InputManager : MonoBehaviour
    {
        private void Update()
        {
            if(Input.anyKey)
                GameManager.Instance.CommandManager.InvokeCommand(new StartInputCommand());
        }
    }
}