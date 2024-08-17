using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerGroundMovement : MonoBehaviour
{
    [SerializeField] PlayerVisuals playerVisuals;
    [SerializeField] float movementSpeedOnGround;     // The speed at which the player moves
    [SerializeField] float movementSpeedInAir;     // The speed at which player is able to move in air, while not flying
    [SerializeField] float jumpStrength;     // The strength of player's jump


    Rigidbody2D playerRigidbody;
    bool isInAir;

    void Start()
    {
        // Getting the rigidbody component
        playerRigidbody = GetComponent<Rigidbody2D>();

        // Setting a few things
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Checking, whether the player is not on the ground
        isInAir = playerRigidbody.velocity.y != 0;

        if(Input.GetKey(KeyCode.D))
        {
            playerVisuals.SetDirectionRight();
            if(!isInAir) playerRigidbody.velocity = Vector2.right * movementSpeedOnGround;
            else playerRigidbody.AddForce(Vector2.right * playerRigidbody.mass * movementSpeedInAir);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            playerVisuals.SetDirectionLeft();
            if(!isInAir) playerRigidbody.velocity = Vector2.left * movementSpeedOnGround;
            else playerRigidbody.AddForce(Vector2.left * playerRigidbody.mass * movementSpeedInAir);
        }
        else if(!isInAir)
        {
            playerRigidbody.velocity = Vector2.zero;
        }

        // Jumping
        if(Input.GetKey(KeyCode.Space) && !isInAir)
        {
            playerRigidbody.AddForce(Vector2.up * playerRigidbody.mass * jumpStrength);
        }
    }
}
