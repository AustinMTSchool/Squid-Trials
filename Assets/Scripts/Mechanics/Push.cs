
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Enums;

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
    public void PushPlayer(int playerID)
    {
        Debug.Log("Event received for push from " + playerID);
        _sender = VRCPlayerApi.GetPlayerById(playerID);
    
        if (_sender != null && _sender.IsValid())
        {
            // Get the sender's head rotation
            Quaternion senderRotation = _sender.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        
            // Calculate push direction from sender's forward direction
            Vector3 forward = senderRotation * Vector3.forward;
        
            // Flatten the forward direction for horizontal push
            Vector3 horizontalDirection = new Vector3(forward.x, 0, forward.z).normalized;
        
            // NEGATED: Push in opposite direction if it was pulling
            // Remove the negative sign if the direction is already correct
            _pushDirection = (-horizontalDirection * pushForceHorizontal) + (Vector3.up * pushForceVertical);
        
            SendCustomEventDelayedFrames(nameof(ApplyVelocity), 1, EventTiming.LateUpdate);
        }
    }

    public void ApplyVelocity()
    {
        if (++_currentFrame > _frameCapture)
        {
            _currentFrame = 0;
            return;
        }
        
        // Apply the calculated push direction
        Networking.LocalPlayer.SetVelocity(_pushDirection);
        SendCustomEventDelayedFrames(nameof(ApplyVelocity), 1, EventTiming.LateUpdate);
    }
}