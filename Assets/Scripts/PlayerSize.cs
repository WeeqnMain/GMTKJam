using TMPro;
using UnityEngine;

public class PlayerSize : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] public Transform spriteTransform;
    [SerializeField] private float initialSpeed;
    [SerializeField] private float scaleIncrease;
    [SerializeField] private float speedDecrease;
  
    private float cooldown;

    private void Awake()
    {
        stats.BaseSpeed = initialSpeed;
    }
  
    private void Update()
    {
        if(cooldown > 0) cooldown -= Time.deltaTime;
    }

    public void IncreaseSize()
    {
        cooldown = 0.5f;
        stats.BaseSpeed -= speedDecrease;

        spriteTransform.localScale += Vector3.one * scaleIncrease;
    }
}
