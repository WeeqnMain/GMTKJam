using System;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public event Action<PlayerController> PlayerGotIntoZone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController player))
        {
            PlayerGotIntoZone?.Invoke(player);
        }
    }
}