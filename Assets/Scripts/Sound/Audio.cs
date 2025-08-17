using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Audio : UdonSharpBehaviour
{
    [SerializeField] private AudioSource soundFX;
    [SerializeField] private AudioClip[] _audioClip = null;
    [SerializeField] private int minVolume = 90;
    [SerializeField] private int maxVolume = 100;
    [SerializeField] private int minPitch = 90;
    [SerializeField] private int maxPitch = 110;
    
    private System.Random _random;

    private void Start()
    {
        _random = new System.Random();
    }

    public void Play()
    {
       PlayRandomSoundFXByDefaultPosition(_audioClip, GetVolume(), GetPitch());
    }

    public float GetVolume()
    {
        return (float) _random.Next(minVolume, maxVolume) / 100;
    }

    public float GetPitch()
    {
        return (float) _random.Next(minPitch, maxPitch) / 100;
    }

    
    private void PlayRandomSoundFXByDefaultPosition(AudioClip[] clips, float volume, float pitch)
    {
        _random = new System.Random();
        soundFX.clip = clips[_random.Next(clips.Length)];
        soundFX.volume = volume;
        soundFX.pitch = pitch;
        soundFX.PlayOneShot(soundFX.clip, soundFX.volume);
    }
}