using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerVisuals playerVisuals;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] jump;
    [SerializeField] private AudioClip[] shortWingFlap;
    [SerializeField] private AudioClip longWingFlap;

    private void Start()
    {
        playerController.Jumped += PlayJumpSound;
        playerVisuals.WingFlapped += PlayShortWingFlapSound;
    }

    private void PlayJumpSound(JumpType obj)
    {
        var rand = Random.Range(0, jump.Length);
        AudioSource.PlayClipAtPoint(jump[rand], transform.position);
    }

    private void PlayShortWingFlapSound()
    {
        var rand = Random.Range(0, shortWingFlap.Length);
        AudioSource.PlayClipAtPoint(shortWingFlap[rand], transform.position);
    }
}
