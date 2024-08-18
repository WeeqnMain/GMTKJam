using System.Collections;
using System.Collections.Generic;
using TarodevController;
using TMPro;
using UnityEngine;

public class PlayerSize : MonoBehaviour
{
  [SerializeField] private TarodevController.PlayerStats stats;
  [SerializeField] private TextMeshProUGUI outputText;
  [SerializeField] private float initialSpeed;
  [SerializeField] private float scaleIncrease;
  [SerializeField] private float massIncrease;
  [SerializeField] private float speedDecrease;
  
  private Vector3 baseScale;
  private int seedsEaten;

  private Rigidbody2D rigidbody;
  private void Awake()
  {
      rigidbody = GetComponent<Rigidbody2D>();
      baseScale = gameObject.transform.localScale;
      seedsEaten = 0;
      stats.BaseSpeed = initialSpeed;
  }
  
  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.gameObject.CompareTag("Food")) 
    {
      ++seedsEaten;
      gameObject.transform.localScale += Vector3.one * scaleIncrease;
      rigidbody.mass += massIncrease;
      stats.BaseSpeed -= speedDecrease;
      outputText.text = seedsEaten.ToString();

      Destroy(other.gameObject);
    }
  }
}
