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
        floodText.text = $"Flood ↓ {player.position.y - flood.position.y - 22:F2}m";
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
