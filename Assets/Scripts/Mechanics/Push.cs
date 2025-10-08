
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Enums;
using VRC.Udon.Common.Interfaces;

public class Push : UdonSharpBehaviour
{
    [SerializeField] private Player _player;
    
    private VRCPlayerApi _sender;
    private Vector3 _pushDirection;
    private int _frameCapture = 5;
    private int _currentFrame = 0;
    
    [Header("Push Settings")]
    [SerializeField] private float pushForceHorizontal = 5f;
    [SerializeField] private float pushForceVertical = 2f;

    [NetworkCallable]
    public void PushPlayerNetwork(int senderID, int targetID)
    {
        Debug.Log("Owner: " + Networking.GetOwner(gameObject).displayName + " of " + gameObject.name);
        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(PushPlayer), senderID, targetID);
    }
    
    [NetworkCallable]
    public void PushPlayer(int senderID, int targetID)
    {
        Debug.Log("Applied damage reduction");
        _player.Health._SetDamageReduction(90);
        _player._SetIsPushed(true);
        _sender = VRCPlayerApi.GetPlayerById(senderID);
        
        if (_sender != null && _sender.IsValid())
        {
            // Get the sender's head rotation (more accurate than body for VR)
            Quaternion senderRotation = _sender.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            
            // Calculate push direction from sender's forward direction
            Vector3 forward = senderRotation * Vector3.forward;
            
            // Flatten the forward direction for horizontal push (keep Y at 0)
            Vector3 horizontalDirection = new Vector3(forward.x, 0, forward.z).normalized;
            
            // Combine horizontal push with upward force
            _pushDirection = (horizontalDirection * pushForceHorizontal) + (Vector3.up * pushForceVertical);
            
            SendCustomEventDelayedFrames(nameof(ApplyVelocity), 1, EventTiming.LateUpdate);
        }
    }

    public void ApplyVelocity()
    {
        if (++_currentFrame > _frameCapture)
        {
            _currentFrame = 0;
            _IsPlayerRegrounded();
            return;
        }
        
        // Apply the calculated push direction
        Networking.LocalPlayer.SetVelocity(_pushDirection);
        SendCustomEventDelayedFrames(nameof(ApplyVelocity), 1, EventTiming.LateUpdate);
    }

    public void _IsPlayerRegrounded()
    {
        if (Networking.LocalPlayer.IsPlayerGrounded())
        {
            Debug.Log("Player regrounded");
            _player.Health._SetDamageReduction(0);
            _player._SetIsPushed(false);
            return;
        }
        else
        {
            SendCustomEventDelayedFrames("_IsPlayerRegrounded", 10, EventTiming.LateUpdate);
        }
    }
}