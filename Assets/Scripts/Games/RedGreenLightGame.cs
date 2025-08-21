
using UdonSharp;
using UnityEngine;
using VRC.SDK3.UdonNetworkCalling;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class RedGreenLightGame : Game
{
    public override void Initialize()
    {
        Debug.Log("Initializing RedGreenLightGame");
        base.Initialize();
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(Worked));
    }

    [NetworkCallable]
    public void Worked()
    {
        if (application.Player.IsInGames)
        {
            Debug.Log("Invoking actions for red green light game");
        }
    }
}
