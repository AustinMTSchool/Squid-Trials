
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ClassesNetwork : UdonSharpBehaviour
{
    [UdonSynced] private string _className;

    public string ClassName => _className;
    
    public void SetClass(Classes classes)
    {
        _className = classes.ClassName;
        RequestSerialization();
    }
}
