using System;
using UnityEngine;

public class SeedCreator : MonoBehaviour
{
    [SerializeField] private Seed _seedPrefab;

    [SerializeField, Range(0, 1)] private float _spawnProbability;
    [SerializeField] private float _spawnFrequency;
    private float _timeToSpawn;

    [SerializeField] private float _throwForce;
    [SerializeField] private float _minAngle;
    [SerializeField] private float _maxAngle;

    public int SeedsEaten { get; private set; }

    public event Action<int> SeedsEatenAmountChanged;

    private void Awake()
    {
        _timeToSpawn = _spawnFrequency;

        Seed[] seedsInScene = FindObjectsByType<Seed>(FindObjectsSortMode.None);
        foreach (var seed in seedsInScene)
        {
            seed.Eaten += OnSeedEaten;
        }
    }

    private void Update()
    {
        if (_timeToSpawn > 0)
            _timeToSpawn -= Time.deltaTime;
        else
        {
            _timeToSpawn = _spawnFrequency;
            if (_spawnProbability > UnityEngine.Random.Range(0f, 1f))
            {
                SpawnSeed();
            }
        }
    }

    private void SpawnSeed()
    {
        Seed seedInstance = Instantiate(_seedPrefab.gameObject, transform.position, Quaternion.identity).GetComponent<Seed>();

        seedInstance.Eaten += OnSeedEaten;

        float randAngle = UnityEngine.Random.Range(_minAngle, _maxAngle);
        var direction = Quaternion.Euler(0f, 0f, randAngle) * Vector2.down;
        seedInstance.ApplyForce(direction * _throwForce);
    }

    private void OnSeedEaten(bool isEaten)
    {
        if (isEaten)
            SeedsEaten++;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, 0f, _minAngle) * Vector2.down);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0f, 0f, _maxAngle) * Vector2.down);
    }
}
