
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

public class TrafficLight : UdonSharpBehaviour
{
    [SerializeField] private TrafficSpotLight red;
    [SerializeField] private TrafficSpotLight green;
    [SerializeField] private TrafficSpotLight yellow;

    private TrafficSpotLight _lastLight;

    private void Start()
    {
        _lastLight = yellow;
        yellow._SwapLight();
    }

    [NetworkCallable]
    public void SetRed()
    {
        _lastLight._SwapLight();
        red._SwapLight();
        _lastLight = red;
    }
    
    [NetworkCallable]
    public void SetYellow()
    {
        _lastLight._SwapLight();
        yellow._SwapLight();
        _lastLight = yellow;
    }
    
    [NetworkCallable]
    public void SetGreen()
    {
        _lastLight._SwapLight();
        green._SwapLight();
        _lastLight = green;
    }

    [NetworkCallable]
    public void Reset()
    {
        red._SetLight(false);
        green._SetLight(false);
        yellow._SetLight(true);
        _lastLight = yellow;
    }
}
