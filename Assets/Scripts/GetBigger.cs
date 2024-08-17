using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetBigger : MonoBehaviour
{

    public float scaleFactor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
      if (other.gameObject.CompareTag("Food")) 
      {
        this.gameObject.transform. localScale *= scaleFactor;
        other.gameObject.SetActive(false);
      }
    }
}
