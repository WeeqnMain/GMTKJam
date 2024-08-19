using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SeedsCounter : MonoBehaviour
{   
    private TextMeshProUGUI TMP;
    [DoNotSerialize]public int counter;

    private void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
        counter = 0;
    }

    private void Update()
    {
        TMP.text = counter.ToString();
    }
}
