
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class TrafficLightManager : UdonSharpBehaviour
{
    [SerializeField] private TrafficLight[] trafficLights;

    public void SetRed()
    {
        foreach (var trafficLight in trafficLights)
        {
            trafficLight.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(trafficLight.SetRed));
        }
    }
    
    public void SetGreen()
    {
        foreach (var trafficLight in trafficLights)
        {
            trafficLight.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(trafficLight.SetGreen));
        }
    }
    
    public void SetYellow()
    {
        foreach (var trafficLight in trafficLights)
        {
            trafficLight.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(trafficLight.SetYellow));
        }
    }

    public void Reset()
    {
        foreach (var trafficLight in trafficLights)
        {
            trafficLight.SendCustomNetworkEvent(NetworkEventTarget.All, nameof(trafficLight.Reset));
        }
    }
}
