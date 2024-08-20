using UnityEngine;

public class BirdNPC : MonoBehaviour
{
    public enum BirdColor
    {
        Red,
        Cyan,
    }

    public enum AnimationType
    {
        Idle,
        Flying,
    }

    [SerializeField] private BirdColor birdColor;

    [SerializeField] private AnimationType animationType;

    [Header("Flying Parameters")]
    [SerializeField] private float flyingSpeed;
    [SerializeField] private Transform pathEnd;
    private Vector3 pathStart;
    private Vector3 direction;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pathStart = transform.position;
        direction = pathEnd.position - pathStart;
    }

    private void Start()
    {
        animator.SetTrigger(animationType.ToString());
        animator.SetTrigger(birdColor.ToString());
    }

    private void Update()
    {
        if (animationType == AnimationType.Flying)
        {
            transform.Translate(flyingSpeed * Time.deltaTime * direction.normalized);
            if (Vector2.Distance(transform.position, pathEnd.position) < 0.1f)
            {
                transform.position = pathStart;
            }
        }
    }
}


