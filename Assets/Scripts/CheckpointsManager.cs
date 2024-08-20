using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    [SerializeField] private Checkpoint startCheckpoint;
    [SerializeField] private Checkpoint[] checkpoints;

    [SerializeField] private DeathZone[] deathZones;

    private Transform resetPosition;

    private void Awake()
    {
        resetPosition = startCheckpoint.GetCheckpointPosition();
        foreach (var checkpoint in checkpoints)
        {
            checkpoint.PassedByPlayer += CheckpointPassed;
        }
        foreach (var deathZone in deathZones)
        {
            deathZone.PlayerGotIntoZone += ResetPlayerPosition;
        }
    }

    private void CheckpointPassed(Transform newResetPosition)
    {
        resetPosition = newResetPosition;
    }

    private void ResetPlayerPosition(PlayerController player)
    {
        player.transform.position = resetPosition.position;
    }
}
