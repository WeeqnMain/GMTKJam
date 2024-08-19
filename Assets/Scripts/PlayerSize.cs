using TMPro;
using UnityEngine;

public class PlayerSize : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private float initialSpeed;
    [SerializeField] private float scaleIncrease;
    [SerializeField] private float massIncrease;
    [SerializeField] private float speedDecrease;
  
    private float cooldown;
    private Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        stats.BaseSpeed = initialSpeed;
    }
  
    private void Update()
    {
        if(cooldown > 0) cooldown -= Time.deltaTime;
    }

    public void IncreaseSize()
    {
        cooldown = 0.5f;
        gameObject.transform.localScale += Vector3.one * scaleIncrease;
        rigidbody.mass += massIncrease;
        stats.BaseSpeed -= speedDecrease;
    }
}
