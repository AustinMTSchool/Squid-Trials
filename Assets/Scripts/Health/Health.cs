
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
    // percentages
    private int _damageReduction = 0;
    private int _defense = 0;
    
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
        var defensePercent = 100 - _defense;
        var subDamage = (damage * defensePercent) / 100;
        
        var damagePercent = 100 - _damageReduction;
        var totalDamage = (subDamage * damagePercent) / 100;
        
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

    public void _AddDefense(int amount)
    {
        _defense += amount;
    }

    public void _RemoveDefense(int amount)
    {
        _defense -= amount;
    }
    
    public void AddMaxHealth(int amount)
    {
        maxHealthPoints += amount;
        _health = maxHealthPoints;
        RequestSerialization();
    }

    public void RemoveMaxHealth(int amount)
    {
        maxHealthPoints -= amount;
        _health = maxHealthPoints;
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        
    }
}
