using UnityEngine;

public class WindyPlace : MonoBehaviour
{
    [SerializeField, Min(0)] private float windForce;
    [SerializeField, Range(0, 359)] private float windAngle;

    [SerializeField, Range(0, 1)] private float HorizontalForceMultiplier = 1f;
    [SerializeField, Range(0, 1)] private float VerticalForceMultiplier = 1f;

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
                player.GetComponent<Rigidbody2D>().AddForce(CalculateForce(), ForceMode2D.Impulse);
            }
        }
        else if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            rb.AddForce(Quaternion.Euler(0, 0, windAngle) * Vector2.up * appliedForce, ForceMode2D.Impulse);
        }
    }

    private Vector2 CalculateForce()
    {
        Vector2 direction = Quaternion.Euler(0, 0, windAngle) * Vector2.up * appliedForce;
        Vector2 force = new(direction.x * HorizontalForceMultiplier, direction.y * VerticalForceMultiplier);
        return force;
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
