using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class PlayerSize : MonoBehaviour
{
    [SerializeField] private TarodevController.PlayerStats stats;

    [SerializeField] private float scaleIncrease;
    [SerializeField] private float massIncrease;
    [SerializeField] private float speedDecrease;
    
    private float seedsEaten;
    private Vector3 baseScale;

    private Rigidbody2D rigidbody;

    private void Awake()
    {
        baseScale = gameObject.transform.localScale;

        rigidbody = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.gameObject.CompareTag("Food")) 
      {
        gameObject.transform.localScale += Vector3.one * scaleIncrease;
        rigidbody.mass += massIncrease;
        stats.BaseSpeed -= speedDecrease;

        Destroy(other.gameObject);
      }
    }
}
