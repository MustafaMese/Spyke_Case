using System;
using System.Collections.Generic;
using SlotMachineCase.Component;
using UnityEngine;

namespace SlotMachineCase.Manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        private CommandManager _commandManager;

        public static GameManager Instance => _instance;
        public CommandManager CommandManager => _commandManager;

        public VFXManager VfxManager;
        
        private void Awake()
        {
            _instance = this;

            _commandManager = new CommandManager();
        }
    }
}
