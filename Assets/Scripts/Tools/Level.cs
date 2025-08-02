
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;

public class Level : UdonSharpBehaviour
{
    public int add;
    private int level;
    public override void Interact()
    {
        level += add;
        PlayerData.SetInt("level", level);
    }
}
