
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using Random = System.Random;

public class GlassBreakerGroup : UdonSharpBehaviour
{
    [SerializeField] private GlassBreaker[] glassBreakers;

    [SerializeField] private bool _containsBreakable = false;


    public void _Initialize(bool containsBreakable = false)
    {
        _containsBreakable = containsBreakable;

        if (_containsBreakable)
        {
            var index = UnityEngine.Random.Range(0, 2);
            glassBreakers[index]._SetBreakState(true);
        }
    }

    public void _Reset()
    {
        foreach (var glassBreaker in glassBreakers)
        {
            glassBreaker.SendCustomNetworkEvent(NetworkEventTarget.All, "Reset");
        }
        _containsBreakable = false;
        RequestSerialization();
    }
}
