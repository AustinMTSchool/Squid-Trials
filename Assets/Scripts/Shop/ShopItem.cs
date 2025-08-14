
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ShopItem : UdonSharpBehaviour
{
    [SerializeField] private Item item;
    
    [Space, Header("Shop Attributes")]
    [SerializeField] private int cost;
    
    [Space, Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI _NameDisplay;
    [SerializeField] private TextMeshProUGUI _costDisplay;
    [SerializeField] private Image _iconDisplay;
    [SerializeField] private TextMeshProUGUI _descriptionDisplay;
    
    public Item Item => item;

    private void Start()
    {
        _NameDisplay.text = item.name;
        _costDisplay.text = $"${cost}";
        _iconDisplay.sprite = item.Icon;
        _descriptionDisplay.text = item.Description;
    }
}
