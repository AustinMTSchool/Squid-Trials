
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TrafficSpotLight : UdonSharpBehaviour
{
    [SerializeField] private GameObject offLight;
    [SerializeField] private GameObject onLight;

    private bool _isOn = false;
    
    public void _SwapLight()
    {
        _isOn = !_isOn;
        
        if (_isOn)
        {
            onLight.SetActive(true);
            offLight.SetActive(false);
        }
        else
        {
            onLight.SetActive(false);
            offLight.SetActive(true);
        }
    }

    public void _SetLight(bool isOn)
    {
        _isOn = isOn;
        
        if (_isOn)
        {
            onLight.SetActive(true);
            offLight.SetActive(false);
        }
        else
        {
            onLight.SetActive(false);
            offLight.SetActive(true);
        }
    }
}
