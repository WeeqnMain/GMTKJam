using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedLifeTime : MonoBehaviour
{
    [SerializeField] float minLifeTime;
    [SerializeField] float maxLifeTime;

    float lifeTime;

    private void Awake()
    {
        lifeTime = Random.Range(minLifeTime, maxLifeTime);
    }

    private void Update()
    {
        lifeTime -= 1 * Time.deltaTime;

        if(lifeTime <= 0) Destroy(gameObject);
    }
}