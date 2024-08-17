using UnityEngine;

public class WindyPlace : MonoBehaviour
{
    [SerializeField, Min(0)] private float windForce;
    [SerializeField, Range(0, 359)] private float windAngle;

    private Vector3 windDirection;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rigidbody))
        {
            Debug.Log(Time.time);

            rigidbody.AddForce(windDirection * windForce);
        }
    }

    private void OnValidate()
    {
        windDirection = Quaternion.Euler(0, 0, windAngle) * Vector2.up;
    }

    private void OnDrawGizmosSelected()
    {
        var pos = transform.position + windDirection;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, pos);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pos, 0.02f);
    }
}
