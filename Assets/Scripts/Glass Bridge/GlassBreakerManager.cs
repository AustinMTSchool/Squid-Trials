
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;
using Random = System.Random;

public class GlassBreakerManager : UdonSharpBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GlassBreakerGroup[] glassBreakerGroups;
    [SerializeField] private int breakablesPerPlayerCount = 2;

    private DataList _availableBreakables = new DataList();

    private void Start()
    {
        for (int i = 0; i < glassBreakerGroups.Length; i++)
        {
            _availableBreakables.Add(i);
        }
    }

    public void _Initialize()
    {
        int totalbreakables = (int) Math.Ceiling((double) gameManager.AllPlayersInGame.Count / breakablesPerPlayerCount);
        int breakables = Math.Min(totalbreakables, glassBreakerGroups.Length);
        Debug.Log("Amount of breakables: " + breakables);
        
        for (int i = 0; i < breakables; i++)
        {
            var index = UnityEngine.Random.Range(0, _availableBreakables.Count);
            Debug.Log("_availableBreakables length: " + _availableBreakables.Count + " accessing index: " + index + " of " + _availableBreakables[index].TokenType);
            int breakableIndex = _availableBreakables[index].Int;
            glassBreakerGroups[breakableIndex]._Initialize(true);
            _availableBreakables.RemoveAt(index);
        }
    }

    public void _Reset()
    {
        Debug.Log("Bridge reset");
        _availableBreakables.Clear();
        
        for (int i = 0; i < glassBreakerGroups.Length; i++)
        {
            glassBreakerGroups[i]._Reset();
            _availableBreakables.Add(i);
        }
    }
}
