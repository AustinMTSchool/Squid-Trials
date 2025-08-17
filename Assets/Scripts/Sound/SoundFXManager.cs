using System;
using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SoundFXManager : UdonSharpBehaviour
{
    [SerializeField] private AudioSource soundFX;
    
    private System.Random _random;

    public void PlayRandomSoundFX(AudioClip[] clips, Transform origin, float volume, float pitch)
    {
        _random = new System.Random();
        soundFX.clip = clips[_random.Next(clips.Length)];
        soundFX.volume = volume;
        soundFX.pitch = pitch;
        soundFX.PlayOneShot(soundFX.clip, soundFX.volume);
        var time = soundFX.clip.length;
    }
    
    public void PlayRandomSoundFX(AudioClip[] clips, Vector3 origin, float volume, float pitch)
    {
        _random = new System.Random();
        soundFX.clip = clips[_random.Next(clips.Length)];
        soundFX.volume = volume;
        soundFX.pitch = pitch;
        soundFX.PlayOneShot(soundFX.clip, soundFX.volume);
    }
    
    public void PlayRandomSoundFXByDefaultPosition(AudioClip[] clips, float volume, float pitch)
    {
        _random = new System.Random();
        soundFX.clip = clips[_random.Next(clips.Length)];
        soundFX.volume = volume;
        soundFX.pitch = pitch;
        soundFX.PlayOneShot(soundFX.clip, soundFX.volume);
    }
}