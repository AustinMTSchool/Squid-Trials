
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Health : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private int maxHealthPoints;
    [SerializeField] private float speedDebuff = 0.1F;
    
    private int debuffIntervals = 20;
    // percentage
    private int _damageReduction = 0;
    
    [UdonSynced] private int _health = 100;
    
    public int GetHealth => _health;
    public int MaxHealthPoints => maxHealthPoints;

    public void _SetHealth(int health)
    {
        _health = health;
        
        if (_health <= 0)
        {
            _health = maxHealthPoints;
            player.Death();
            return;
        }
        
        RequestSerialization();
    }

    public void _ResetHealth()
    {
        _health = maxHealthPoints;
        RequestSerialization();
    }

    public void _AddHealth(int health)
    {
        _health += health;
        
        if (_health > maxHealthPoints)
            _health = maxHealthPoints;
        
        RequestSerialization();
    }

    // True if dies otherwise a live
    public bool _RemoveHealth(int damage)
    {
        var damageMultiplier = 100 - _damageReduction;
        var totalDamage = (damage * damageMultiplier) / 100;
        
        _health -= (int) totalDamage;
        Debug.Log("Player health: " + _health);
        
        player.PlayerOverlayEffects._DamageOverlay(_health);
        
        if (_health <= 0)
        {
            _health = maxHealthPoints;
            player.Death();
            return true;
        }
        
        RequestSerialization();
        return false;
    }

    public void _SetDamageReduction(int reduction)
    {
        _damageReduction = reduction;
    }

    public override void OnDeserialization()
    {
        
    }
}
