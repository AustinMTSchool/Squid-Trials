
using System;
using System.Text;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ColorPattern : UdonSharpBehaviour
{
    [SerializeField] private string[] colors;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        var coloredText = new StringBuilder();
        for (var i = 0; i < text.text.Length; i++)
        {
            var colorIndex = i % colors.Length;
            coloredText.Append($"<color={colors[colorIndex]}>{text.text[i]}");
        }
        text.text = coloredText.ToString();
    }
}
