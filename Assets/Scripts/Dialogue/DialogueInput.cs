
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[Serializable]
public class DialogueInput : UdonSharpBehaviour
{
    [SerializeField, Range(0, 600)] private int time;
    [SerializeField, TextArea(3,5)] private string message;
    
    public int Time => time;
    public string Message => message;
}
