
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

[Serializable]
public class Dialogue : UdonSharpBehaviour
{
    [SerializeField] private DialogueInput[] inputs;
    
    private DataDictionary _dialogueList = new DataDictionary();

    public DataDictionary DialogueList => _dialogueList;

    private void Start()
    {
        foreach (var dialogueInput in inputs)
        {
            _dialogueList.Add(dialogueInput.Time, dialogueInput.Message);
        }
    }
}
