using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSystem;

    private void Start()
    {
        Stop();
    }

    public void PlayVFX(string key, Action OnEnd)
    {
        StartCoroutine(Play(key, OnEnd));
    }
    
    public IEnumerator Play(string key, Action OnEnd)
    {
        switch (key)
        {
            case "Jackpot":
                Play(25);
                break;
            case "Wild":
                Play(20);
                break;
            case "Seven":
                Play(15);
                break;
            case "Bonus":
                Play(10);
                break;
            case "A":
                Play(5);
                break;
        }

        yield return new WaitForSeconds(2f);
        Stop();
        
        OnEnd?.Invoke();
    }

    private void Stop()
    {
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    
    private void Play(int count)
    {
        var emission = particleSystem.emission;
        emission.rateOverTime = count;
        
        particleSystem.Play();
    }
}
