
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;

[RequireComponent(typeof(Collider))]
public class Hand : UdonSharpBehaviour
{
    [SerializeField] protected Player player;
    [SerializeField] protected Transform pushComponent;
    [SerializeField] protected PushManager pushManager;
    [SerializeField] protected int maxPlayerPush = 2;
    
    protected DataList _playersContactList = new DataList();
    protected VRCPlayerApi _playerApi;
    
    public DataList PlayersContactList => _playersContactList;

    protected virtual void Start()
    {
        _playerApi = Networking.LocalPlayer;
    }

    public virtual void _Push()
    {
        for (int i = 0; i < _playersContactList.Count; i++)
        {
            if (i == 2) break;
            Debug.Log("Pushing: " + _playersContactList[i].Int);
            var targetPlayer = VRCPlayerApi.GetPlayerById(_playersContactList[i].Int);
            var component = Networking.FindComponentInPlayerObjects(targetPlayer, pushComponent);
            var push = component.GetComponent<Push>();
            Debug.Log("Sending on: " + component.gameObject.name);
            if (player.ClassesManager.CurrentClass != null)
            {
                push.PushPlayerNetwork(_playerApi.playerId, player.ClassesManager.CurrentClass.ForceKnockbackPercentage);
            }
            else
            {
                push.PushPlayerNetwork(_playerApi.playerId, 0);
            }
        }
        _playersContactList.Clear();
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        Debug.Log("Entering: " + player.playerId);
        if (player.isLocal) return;

        if (_playersContactList.Contains(player.playerId)) return;
        
        Debug.Log("Added a player to push list");
        _playersContactList.Add(new DataToken(player.playerId));
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        Debug.Log("Exiting: " + player.playerId);
        if (player.isLocal) return;
        
        _playersContactList.Remove(player.playerId);
    }
    
    
}
