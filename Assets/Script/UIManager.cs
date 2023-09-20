using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image[] bar;
    public Color[] stageColors;
    public TMP_Text floodText;
    public Transform player, flood;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Clear();
    }

    private void Update()
    {
        var dist = player.position.y - flood.position.y - 26;
        floodText.text = $"Flood ↓ {dist:F2}m";
        floodText.rectTransform.localScale = Mathf.Clamp(2.5f - dist / 8f, 1, 4) * Vector3.one;
    }

    public void Fill(float amount)
    {
        for (int i = 0; i < 3; i++)
        {
            bar[i].fillAmount = amount - i;
            bar[i].color = stageColors[(int)amount];
        }
    }

    public void Clear()
    {
        foreach (Image b in bar)
        {
            b.fillAmount = 0;
        }
    }
}
