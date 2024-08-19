using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BerriesCounter : MonoBehaviour
{   
    private TextMeshProUGUI TMP;
    public int seedsCounter;

    private void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
        seedsCounter = 0;
    }

    private void Update()
    {
        TMP.text = seedsCounter.ToString();
    }
}
