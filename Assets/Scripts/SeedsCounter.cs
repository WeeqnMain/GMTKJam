using TMPro;
using UnityEngine;

public class SeedsCounter : MonoBehaviour
{   
    [SerializeField] private TextMeshProUGUI TMP;

    [SerializeField] private SeedCreator seedCreator;

    public int counter; // This is required for the SeedCreator script to know, when to stop generating seeds

    private void Awake()
    {
        counter = 0;
        TMP = GetComponent<TextMeshProUGUI>();
        seedCreator.SeedsEatenAmountChanged += UpdateValue;
    }

    private void UpdateValue(int value)
    {
        TMP.text = value.ToString();
        ++counter;
    }
}
