using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Audio : UdonSharpBehaviour
{
    [SerializeField] private SoundFXManager _soundFXManager;
    [SerializeField] private AudioClip[] _audioClip = null;
    [SerializeField] private int minVolume = 90;
    [SerializeField] private int maxVolume = 100;
    [SerializeField] private int minPitch = 90;
    [SerializeField] private int maxPitch = 110;

    public SoundFXManager SoundFXManager => _soundFXManager;
    
    private System.Random _random;

    private void Start()
    {
        _random = new System.Random();
    }

    public void Play()
    {
        _soundFXManager.placeHolderOrigin.position = transform.position;
        _soundFXManager.PlayRandomSoundFX(_audioClip, GetVolume(), GetPitch());
    }

    public void Play(Transform origin)
    {
        _soundFXManager.placeHolderOrigin.position = origin.position;
        _soundFXManager.PlayRandomSoundFX(_audioClip, _soundFXManager.placeHolderOrigin.transform, GetVolume(), GetPitch());
    }
    
    public void Play(Vector3 origin)
    {
        _soundFXManager.placeHolderOrigin.position = origin;
        _soundFXManager.PlayRandomSoundFX(_audioClip, _soundFXManager.placeHolderOrigin.position, GetVolume(), GetPitch());
    }

    public float GetVolume()
    {
        return (float) _random.Next(minVolume, maxVolume) / 100;
    }

    public float GetPitch()
    {
        return (float) _random.Next(minPitch, maxPitch) / 100;
    }
}