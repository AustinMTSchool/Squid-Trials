
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;

public class ShootOut : UdonSharpBehaviour
{
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private Audio shoot;

    [NetworkCallable]
    public void Fire()
    {
        Debug.Log("Fired from " + gameObject.name);
        shoot.Play();
    }
}
