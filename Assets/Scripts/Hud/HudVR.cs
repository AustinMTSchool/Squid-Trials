
using System;
using UdonSharp;
using UnityEngine;
using VRC;
using VRC.SDKBase;
using VRC.Udon;

public class HudVR : Hud
{
    public float smoothSpeed = 25f;
    
    [SerializeField] private Vector3 handOffset = new Vector3(0, 0.1f, 0);
    [SerializeField] private RectTransform _transform;
    private VRCPlayerApi _player;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    
    private void Start()
    {
        _player = Networking.LocalPlayer;
    }
    
    private void LateUpdate()
    {
        if (_player == null || !_player.IsValid())
        {
            return;
        }
    
        var handData = _player.GetTrackingData(VRCPlayerApi.TrackingDataType.LeftHand);
        var headData = _player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
    
        _targetPosition = handData.position + handOffset;
        _targetRotation = Quaternion.LookRotation(_transform.position - headData.position);
        
        _transform.position = Vector3.Lerp(_transform.position, _targetPosition, Time.deltaTime * smoothSpeed);
        _transform.rotation = Quaternion.Slerp(_transform.rotation, _targetRotation, Time.deltaTime * smoothSpeed);
    }
}
