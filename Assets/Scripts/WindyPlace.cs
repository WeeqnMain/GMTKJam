using UnityEngine;

public class WindyPlace : MonoBehaviour
{
    [SerializeField, Min(0)] private float windForce;
    [SerializeField, Range(0, 359)] private float windAngle;

    private float appliedForce;

    private void Awake()
    {
        //magically change the force to get more accuracy
        appliedForce = windForce / 100f; 
    }

    private void OnValidate()
    {
        appliedForce = windForce / 100f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController player))
        {
            if (player.IsGliding)
            {
                player.AddFrameForce(Quaternion.Euler(0, 0, windAngle) * Vector2.up * appliedForce);
            }
        }
        else if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            rb.AddForce(Quaternion.Euler(0, 0, windAngle) * Vector2.up * appliedForce, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        var pos = transform.position + Quaternion.Euler(0, 0, windAngle) * Vector2.up;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, pos);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pos, 0.02f);
    }
}
