
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class ItemManager : UdonSharpBehaviour
{
    [SerializeField] private Item[] allItems;
    
    private DataDictionary _allItemsByID = new DataDictionary();
    
    public DataDictionary AllItemsByID => _allItemsByID;
    public Item[] AllItems => allItems;

    private void Start()
    {
        foreach (Item item in allItems)
        {
            _allItemsByID.Add(item.ID, new DataToken(item));
        }
    }

    public Item GetItemByID(string id)
    {
        if (_allItemsByID.TryGetValue(id, TokenType.Reference, out DataToken value))
        {
            return (Item)value.Reference;
        }
        Debug.LogWarningFormat($"[ItemManager] No registered item ID name is {name}\nThe item may not have been added into ItemManager");
        return null;
    }
}
