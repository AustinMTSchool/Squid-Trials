using System;
using UdonSharp;
using Unity.Mathematics;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class SoundFXManager : UdonSharpBehaviour
{
    [SerializeField] private AudioSource soundFX;
    [SerializeField] private Transform _placeHolderOrigin;
    
    private System.Random _random;

    public Transform placeHolderOrigin => _placeHolderOrigin;

    public void PlayRandomSoundFX(AudioClip[] clips, Transform origin, float volume, float pitch)
    {
        _random = new System.Random();
        var audioGameObject = Instantiate(soundFX.gameObject, origin);
        var audioSource = audioGameObject.GetComponent<AudioSource>();
        audioSource.clip = clips[_random.Next(clips.Length)];
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioSource.clip, audioSource.volume);
        var time = audioSource.clip.length;
        Destroy(audioGameObject, time);
    }
    
    public void PlayRandomSoundFX(AudioClip[] clips, Vector3 origin, float volume, float pitch)
    {
        _random = new System.Random();
        var audioGameObject = Instantiate(soundFX.gameObject, origin, Quaternion.identity);
        var audioSource = audioGameObject.GetComponent<AudioSource>();
        audioSource.clip = clips[_random.Next(clips.Length)];
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioSource.clip, audioSource.volume);
        var time = audioSource.clip.length;
        Destroy(audioGameObject, time);
    }
    
    public void PlayRandomSoundFX(AudioClip[] clips, float volume, float pitch)
    {
        _random = new System.Random();
        var audioGameObject = Instantiate(soundFX.gameObject, _placeHolderOrigin);
        var audioSource = audioGameObject.GetComponent<AudioSource>();
        audioSource.clip = clips[_random.Next(clips.Length)];
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(audioSource.clip, audioSource.volume);
        var time = audioSource.clip.length;
        Destroy(audioGameObject, time);
    }
}