using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform checkpointPosition;

    public event Action<Transform> PassedByPlayer;

    public Transform GetCheckpointPosition() => checkpointPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController player))
        {
            PassedByPlayer?.Invoke(checkpointPosition);
            gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        if (checkpointPosition == null)
            Debug.LogWarning($"Checkpoint {name} doesn't have CheckpointPosition field set");
    }
}