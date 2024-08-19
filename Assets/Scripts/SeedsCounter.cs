using TMPro;
using UnityEngine;

public class SeedsCounter : MonoBehaviour
{   
    [SerializeField] private TextMeshProUGUI TMP;

    [SerializeField] private SeedCreator seedCreator;

    private void Awake()
    {
        TMP = GetComponent<TextMeshProUGUI>();
        seedCreator.SeedsEatenAmountChanged += UpdateValue;
    }

    private void UpdateValue(int value)
    {
        TMP.text = value.ToString();
    }
}
