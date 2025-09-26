
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
    [SerializeField] protected Transform pushComponent;
    [SerializeField] protected PushManager pushManager;
    
    protected DataList _playersContactList = new DataList();
    protected VRCPlayerApi _playerApi;
    
    public DataList PlayersContactList => _playersContactList;

    private void Start()
    {
        _playerApi = Networking.LocalPlayer;
        _OnUpdate();
        _Push();
    }

    protected virtual void _OnUpdate()
    {
        
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player.isLocal) return;
        
        _playersContactList.Add(new DataToken(player.playerId).Int);
    }

    public virtual void _Push()
    {
        // TODO put a cap on how many players could be pushed
        for (int i = 0; i < _playersContactList.Count; i++)
        {
            var player = VRCPlayerApi.GetPlayerById(_playersContactList[i].Int);
            var component = Networking.FindComponentInPlayerObjects(player, pushComponent);
            var push = component.GetComponent<Push>();
            push.PushPlayer(_playerApi.playerId);
        }
    }

    public override void OnPlayerCollisionEnter(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        if (_playersContactList.Contains(player.playerId)) return;
        
        _playersContactList.Add(new DataToken(player.playerId).Int);
    }

    public override void OnPlayerCollisionExit(VRCPlayerApi player)
    {
        if (!player.isLocal) return;
        
        _playersContactList.Remove(player.playerId);
    }
    
    
}
