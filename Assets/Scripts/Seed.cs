using System;
using System.Collections;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] private float minLifeTime;
    [SerializeField] private float maxLifeTime;

    [SerializeField] private AudioClip[] collectSounds;

    private Rigidbody2D rb;

    public event Action<bool> Eaten;

    private bool isActive = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(LifeTimeRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerSize playerSize) && isActive)
        {
            playerSize.IncreaseSize();
            RemoveSelf(true);
        }
    }

    private void RemoveSelf(bool isEaten = false)
    {
        Eaten?.Invoke(isEaten);
        isActive = false;
        AudioSource.PlayClipAtPoint(collectSounds[UnityEngine.Random.Range(0, collectSounds.Length)], transform.position);
        Destroy(gameObject);
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
