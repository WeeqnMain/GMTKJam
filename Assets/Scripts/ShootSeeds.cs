using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShootSeeds : MonoBehaviour
{
    [SerializeField] GameObject seedPrefab;

    [SerializeField] float frequency;
    [SerializeField] float minHorizontalVelocity;
    [SerializeField] float maxHorizontalVelocity;

    private void Update()
    {
        if(Random.Range(0f, 1f) < frequency * Time.deltaTime)
        {
            GameObject seed = Instantiate(seedPrefab, 
                gameObject.transform.position, 
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f - float.MinValue)));
            
            try
            {
                Rigidbody2D seedRigidbody = seed.GetComponent<Rigidbody2D>();
                seedRigidbody.velocity = new Vector2(Random.Range(minHorizontalVelocity, maxHorizontalVelocity), 0);
            }
            catch (MissingComponentException)
            {
                Debug.Log("The seed prefab does not have a Rigidbody2D component!");
            }
        }
    }
}
