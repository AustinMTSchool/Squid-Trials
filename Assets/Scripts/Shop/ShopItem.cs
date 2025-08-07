
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ShopItem : UdonSharpBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private TextMeshProUGUI _NameDisplay;
    [SerializeField] private TextMeshProUGUI _costDisplay;
    [SerializeField] private Image _iconDisplay;
    
    public Item Item => item;
}
