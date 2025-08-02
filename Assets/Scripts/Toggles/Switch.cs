
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Switch : UdonSharpBehaviour
{
    [SerializeField] private GameObject[] disables;
    [SerializeField] private GameObject[] enables;

    public void OnClick()
    {
        foreach (var disable in disables) disable.SetActive(false);
        foreach (var enable in enables) enable.SetActive(true);
    }
}
