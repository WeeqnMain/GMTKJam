using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Win : MonoBehaviour
{
    [SerializeField] private int requiredSeeds;

    private SeedCreator seedCreator;
    private Image image;

    private void Awake()
    {
        seedCreator = FindObjectOfType<SeedCreator>();
        image = GetComponent<Image>();
        image.enabled = false;
    }

    private void Update()
    {
        if(seedCreator.SeedsEaten >= requiredSeeds)
        {
            image.enabled = true;
        }
    }
}
