using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public void SetDirectionLeft()
    {
        transform.localScale = new Vector3(-1, 1, 1);
    }

    public void SetDirectionRight()
    {
        transform.localScale = Vector3.one;
    }
}
