using System;
using System.Collections;
using System.Collections.Generic;
using SlotMachineCase.Component;
using UnityEngine;
using UnityEngine.UI;

public class OutcomeTest : MonoBehaviour
{
    [SerializeField] private Button testButton;
    
    private OutcomeHandler _outcomeHandler;
    
    private void Awake()
    {
        _outcomeHandler = new OutcomeHandler(true);
        
        testButton.onClick.AddListener(() =>
        {
            Test();
        });
    }

    private void Test()
    {
        _outcomeHandler.LogProbabilities();
    }
}
