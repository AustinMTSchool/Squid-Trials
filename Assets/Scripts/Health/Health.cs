
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Health : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private uint maxHealthPoints;
    [SerializeField] private float speedDebuff = 0.1F;
    private int debuffIntervals = 20;
    
    [UdonSynced] private uint _health = 100;
    
    public uint GetHealth => _health;
    public uint MaxHealthPoints => maxHealthPoints;

    public void _SetHealth(uint health)
    {
        _health = health;
        RequestSerialization();
    }

    public void _ResetHealth()
    {
        _health = maxHealthPoints;
        RequestSerialization();
    }

    public void _AddHealth(uint health)
    {
        _health += health;
        
        if (_health > maxHealthPoints)
            _health = maxHealthPoints;
        
        RequestSerialization();
    }

    // True, if dies otherwise a live
    public bool _RemoveHealth(uint health)
    {
        _health -= health;
        player.PlayerOverlayEffects._DamageOverlay(_health);
        
        if (_health == 0)
        {
            _health = maxHealthPoints;
            player.Death();
            return true;
        }
        
        RequestSerialization();
        return false;
    }

    public override void OnDeserialization()
    {
        
    }
}
