using UnityEngine;

public class BirdNPC : MonoBehaviour
{
    public enum BirdColor
    {
        Blue,
        Cyan,
        Green, 
        Purple,
        Red,
    }

    public enum AnimationType
    {
        Idle,
        Flying,
    }

    [SerializeField] private BirdColor birdColor;

    [SerializeField] private AnimationType animationType;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
    }

    private void Start()
    {
        animator.SetTrigger(animationType.ToString());
        animator.SetTrigger(birdColor.ToString());
    }
}


