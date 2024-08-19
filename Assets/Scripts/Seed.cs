using System;
using System.Collections;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] private bool unlimitedLifeTime;
    [SerializeField] private float minLifeTime;
    [SerializeField] private float maxLifeTime;

    private Rigidbody2D rb;
    private SeedsCounter seedsCounter;
    private bool eaten; // A added it so that the seed can only be eaten once, otherwise the collider could be triggered multiple times before being destroyed

    public event Action<bool> Eaten;

    private void Awake()
    {
        eaten = false;
        rb = GetComponent<Rigidbody2D>();
        if(!unlimitedLifeTime) StartCoroutine(LifeTimeRoutine());

        seedsCounter = FindObjectOfType<SeedsCounter>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSize playerSize) && !eaten)
        {
            eaten = true;
            playerSize.IncreaseSize();
            RemoveSelf(true);

            ++seedsCounter.counter;
        }
    }

    private void RemoveSelf(bool isEaten = false)
    {
        Destroy(gameObject);
        Eaten?.Invoke(isEaten);
    }

    private IEnumerator LifeTimeRoutine()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(minLifeTime, maxLifeTime));
        RemoveSelf();
    }

    public void ApplyForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
