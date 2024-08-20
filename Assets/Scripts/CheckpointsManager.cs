using System.Collections;
using UnityEngine;

public class CheckpointsManager : MonoBehaviour
{
    [SerializeField] private Checkpoint startCheckpoint;
    [SerializeField] private Checkpoint[] checkpoints;

    [SerializeField] private DeathZone[] deathZones;

    private Transform resetPosition;

    private bool isResetingPosition;

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
        if (isResetingPosition == false)
            StartCoroutine(ResetPlayerPositionRoutine(player));
    }

    public IEnumerator ResetPlayerPositionRoutine(PlayerController player)
    {
        isResetingPosition = true;

        player.AddFrameForce(40f * Vector2.up);

        PlayerVisuals visuals = player.GetComponentInChildren<PlayerVisuals>();

        bool isAnimationEnded = false;
        visuals.StartDeathAnimation(() => isAnimationEnded = true);
        while (isAnimationEnded == false)
        {
            yield return null;
        }
        player.transform.position = resetPosition.position;

        isResetingPosition = false;
    }
}
