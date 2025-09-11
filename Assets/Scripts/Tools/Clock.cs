
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Clock : UdonSharpBehaviour
{
    public static string ConvertInteger(int time)
    {
        string minutes = "" + (int) time / 60;
        string seconds = "" + (int) time % 60;
        
        if (seconds.Length == 1)
            seconds = "0" + seconds;
        
        return minutes + ":" + seconds;
    }
}
