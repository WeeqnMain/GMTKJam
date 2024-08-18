using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ShootSeeds : MonoBehaviour
{
    [SerializeField] GameObject seedPrefab;

    [SerializeField] float frequency;
    [SerializeField] private Vector2 minPos;
    [SerializeField] private Vector2 maxPos;

    private void Update()
    {
        if(Random.Range(0f, 1f) < frequency * Time.deltaTime)
        {
            Vector3 pos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), -1f);

            GameObject seed = Instantiate(seedPrefab, 
                gameObject.transform.position + pos, 
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f - float.MinValue)));
        }
    }
}
