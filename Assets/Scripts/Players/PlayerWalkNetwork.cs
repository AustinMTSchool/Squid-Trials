using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class PlayerWalkNetwork : UdonSharpBehaviour
{
    [SerializeField] private Audio walk;
    [UdonSynced] private Vector3 _vector;
    
    public void Play()
    {
        _vector = Networking.LocalPlayer.GetPosition();
        transform.position = _vector;
        walk.Play();
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        transform.position = _vector;
        walk.Play();
    }
}
