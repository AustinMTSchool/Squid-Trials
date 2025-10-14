
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class ClassesManager : UdonSharpBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Classes[] allClasses;
    [SerializeField] private Transform classesComponent;
    
    private ClassesNetwork _classesNetwork;
    private Classes _currentSelectedClass;
    private DataDictionary _classesByNames = new DataDictionary();
    private bool _initialized = false;
    
    public Classes CurrentClass => _currentSelectedClass;

    private void Start()
    {
        foreach (Classes classes in allClasses)
        {
            _classesByNames.Add($"{classes.ClassName}", classes);
        }
    }

    public override void OnPlayerRestored(VRCPlayerApi player)
    {
        if (!player.isLocal) return;

        var comp = Networking.FindComponentInPlayerObjects(player, classesComponent);
        _classesNetwork = comp.GetComponent<ClassesNetwork>();
        
        if (_classesNetwork == null) return;

        _currentSelectedClass = GetClassByName(_classesNetwork.ClassName);

        if (_currentSelectedClass == null)
        {
            _initialized = true;
            return;
        }
        
        _currentSelectedClass.ApplyEffects(this.player);
        _initialized = true;
    }

    public void OnClassSelection(Classes classes)
    {
        if (!_initialized) return;
        if (classes == _currentSelectedClass) return;
    
        if (_currentSelectedClass != null)
        {
            _currentSelectedClass.RemoveEffects(player);
        }
        
        _currentSelectedClass = classes;
        _currentSelectedClass.ApplyEffects(player);
        _classesNetwork.SetClass(classes);
    }

    public Classes GetClassByName(string className)
    {
        if (_classesByNames.TryGetValue(className, TokenType.Reference, out DataToken value))
        {
            return (Classes) value.Reference;
        }
        return null;
    }
}
