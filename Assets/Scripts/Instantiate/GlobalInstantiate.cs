
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class GlobalInstantiate : UdonSharpBehaviour
{

    [SerializeField] private GameObject[] instantiateList;
    
    private DataDictionary _instantiateList = new DataDictionary();
    
    void Start()
    {
        foreach (var obj in instantiateList)
        {
            _instantiateList.Add(obj.name, obj);
        }
    }
}
