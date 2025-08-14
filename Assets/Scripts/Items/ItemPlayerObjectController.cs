
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ItemPlayerObjectController : UdonSharpBehaviour
{
    [SerializeField] private Holdable holdable;
    [UdonSynced] private bool _isItemActive = false;
    public Holdable Holdable => holdable;

    public void SetItemActive(bool value)
    {
        _isItemActive = value;
        holdable.gameObject.SetActive(_isItemActive);
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        Debug.Log("OnDeserialization on item controller");
        holdable.gameObject.SetActive(_isItemActive);
    }
}