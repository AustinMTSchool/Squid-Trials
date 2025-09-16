
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using Random = System.Random;

public class ShootOutManager : UdonSharpBehaviour
{
    [SerializeField] private ShootOut[] shootOuts;
    
    private Random _random = new Random();
    private int min = 0;
    private int max = 0;

    private void Start()
    {
        _random = new Random();
        max = shootOuts.Length;
    }

    public void Fire()
    {
        var num = _random.Next(min, max);
        shootOuts[num].SendCustomNetworkEvent(NetworkEventTarget.All, "Fire");
    }
}
