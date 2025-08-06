
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PlayerWalk : UdonSharpBehaviour
{
    [SerializeField] private Audio playerWalk;
    [SerializeField] private float walkIntervalPlay = 0.7F;
    [SerializeField] private float runIntervalPlay = 0.5F;
    
    private VRCPlayerApi _player;
    private float _horizontalMovement;
    private float _verticalMovement;
    private bool _inVR;
    private bool _isMoving;
    private bool _wasGrounded = true; // Track previous grounded state
    
    void Start()
    {
        _player = Networking.LocalPlayer;
        _inVR = _player.IsUserInVR();
        _wasGrounded = _player.IsPlayerGrounded();
        Movement();
        AdjustGrounded();
    }

    public void AdjustGrounded()
    {
        bool currentlyGrounded = _player.IsPlayerGrounded();
        
        if (!_wasGrounded && currentlyGrounded)
        {
            if (HasMovementInput())
            {
                if (!_isMoving) Movement();
            }
        }
        _wasGrounded = currentlyGrounded;
        SendCustomEventDelayedSeconds(nameof(AdjustGrounded), 0.05F);
    }

    public void Movement()
    {
        if (IsWalking())
        {
            Debug.Log("Playing sound");
            playerWalk.Play(_player.GetPosition());
            _isMoving = true;
            SendCustomEventDelayedSeconds(nameof(Movement), walkIntervalPlay);
            return;
        }

        if (IsRunning())
        {
            Debug.Log("Playing sound running");
            playerWalk.Play(_player.GetPosition());
            _isMoving = true;
            SendCustomEventDelayedSeconds(nameof(Movement), runIntervalPlay);
            return;
        }
        _isMoving = false;
    }
    
    public override void InputMoveVertical(float value, UdonInputEventArgs args)
    {
        Debug.Log("Vert: " + value);
        if (!_inVR)
        {
            if (Mathf.Approximately(value, 0.0F)) return;
            if (!_isMoving) Movement();
        }
        else
        {
            _verticalMovement = value;
            if (Mathf.Approximately(value, 0.0F)) return;
            if (!_isMoving) Movement();
        }
    }

    public override void InputMoveHorizontal(float value, UdonInputEventArgs args)
    {
        Debug.Log("Horz: " + value);
        if (!_inVR)
        {
            if (Mathf.Approximately(value, 0.0F)) return;
            if (!_isMoving) Movement();
        }
        else
        {
            _horizontalMovement = value;
            if (Mathf.Approximately(value, 0.0F)) return;
            if (!_isMoving) Movement();
        }
    }

    private bool HasMovementInput()
    {
        return !Mathf.Approximately(Input.GetAxis("Horizontal"), 0.0f) || 
               !Mathf.Approximately(Input.GetAxis("Vertical"), 0.0f);
    }

    private bool IsWalking()
    {
        if (!_inVR)
        {
            if (_player.IsPlayerGrounded() && !Input.GetKey(KeyCode.LeftShift) && HasMovementInput())
            {
                return true;
            }
        }
        else
        {
            float movement = Mathf.Max(Mathf.Abs(_verticalMovement), Mathf.Abs(_horizontalMovement));
            if (_player.IsPlayerGrounded() && movement > 0.0F && movement < 0.5F && HasMovementInput())
            {
                return true;
            }
        }
        return false;
    }

    private bool IsRunning()
    {
        if (!_inVR)
        {
            if (_player.IsPlayerGrounded() && Input.GetKey(KeyCode.LeftShift) && HasMovementInput())
            {
                return true;
            }
        }
        else
        {
            float movement = Mathf.Max(Mathf.Abs(_verticalMovement), Mathf.Abs(_horizontalMovement));
            if (_player.IsPlayerGrounded() && movement >= 0.5F && movement <= 1.1F && HasMovementInput())
            {
                return true;
            }
        }
        return false;
    }
}