
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
    [SerializeField] private float pushForceVertical = 3f;

    public VRCPlayerApi PushedBy => _sender;

    [NetworkCallable]
    public void PushPlayerNetwork(int senderID, float pushForce)
    {
        Debug.Log("Owner: " + Networking.GetOwner(gameObject).displayName + " of " + gameObject.name);
        SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(PushPlayer), senderID, pushForce);
    }

    [NetworkCallable]
    public void PushPlayer(int senderID, float pushForce)
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
            Debug.Log("Before Pushed: " + _pushDirection);
            
            var pushForceMath =  (pushForce + 100) * _pushDirection;
            _pushDirection = pushForceMath / 100;

            Debug.Log("Pushed No Anti: " + _pushDirection);
            
            if (_player.ClassesManager.CurrentClass != null)
            {
                // Convert percentage to decimal (20% -> 0.2) and reduce push force
                _pushDirection *= (1f - _player.ClassesManager.CurrentClass.AntiKnockbackPercentage / 100f);
            }

            Debug.Log("Pushed: " + _pushDirection);
            
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
        
        Networking.LocalPlayer.SetVelocity(_pushDirection);
        SendCustomEventDelayedFrames(nameof(ApplyVelocity), 1, EventTiming.LateUpdate);
    }

    public void _IsPlayerRegrounded()
    {
        if (Networking.LocalPlayer.IsPlayerGrounded())
        {
            _player.Health._SetDamageReduction(0);
            _player._SetIsPushed(false);
            _sender = null;
            return;
        }
        else
        {
            SendCustomEventDelayedFrames("_IsPlayerRegrounded", 10, EventTiming.LateUpdate);
        }
    }
}