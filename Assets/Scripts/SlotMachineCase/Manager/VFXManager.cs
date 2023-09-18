using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfx;

    private void Start()
    {
        Stop();
    }

    public void PlayVFX(int key, Action OnEnd)
    {
        StartCoroutine(Play(key, OnEnd));
    }
    
    public IEnumerator Play(int key, Action OnEnd)
    {
        switch (key)
        {
            case 0:
                Play(25);
                break;
            case 1:
                Play(20);
                break;
            case 2:
                Play(15);
                break;
            case 3:
                Play(10);
                break;
            case 4:
                Play(5);
                break;
        }

        yield return new WaitForSeconds(2f);
        Stop();
        
        OnEnd?.Invoke();
    }

    private void Stop()
    {
        vfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    
    private void Play(int count)
    {
        var emission = vfx.emission;
        emission.rateOverTime = count;
        
        vfx.Play();
    }
}
