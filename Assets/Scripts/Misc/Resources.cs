
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Resources : UdonSharpBehaviour
{
    [SerializeField] private Sprite blankSprite;
    public Sprite BlankSprite => blankSprite;
}
