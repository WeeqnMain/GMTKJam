using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShootSeeds : MonoBehaviour
{
    [SerializeField] GameObject seedPrefab;

    [SerializeField] float frequency;
    [SerializeField] private float velocity;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    private void Update()
    {
        if(Random.Range(0f, 1f) < frequency * Time.deltaTime)
        {
            GameObject seed = Instantiate(seedPrefab, 
                gameObject.transform.position + Vector3.back,
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f - float.MinValue)));

            Rigidbody2D seedRigidbody = seed.GetComponent<Rigidbody2D>();
            float angle = Random.Range(minAngle, maxAngle);
            seedRigidbody.velocity = new Vector2(
                -Mathf.Sin((angle - 90) % 360),
                Mathf.Cos((angle - 90) % 360));
            seedRigidbody.velocity = seedRigidbody.velocity.normalized * velocity;
        }
    }
}
