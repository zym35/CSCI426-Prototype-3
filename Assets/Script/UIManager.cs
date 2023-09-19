using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public Image[] bar;
    public Color[] stageColors;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Clear();
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
