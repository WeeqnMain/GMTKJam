using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // The speed at which the player moves
    public float movementSpeedOnGround;
    
    // The speed at which player is able to move in air, while not flying
    public float movementSpeedInAir;
    
    // The strength of player's jump
    public float jumpStrength;


    // The object's rigidbody
    Rigidbody2D playerRigidbody;

    void Start()
    {
        // Getting the rigidbody component
        playerRigidbody = GetComponent<Rigidbody2D>();

        // Setting a few things
        playerRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Checking, if the player is on the ground
        if(playerRigidbody.velocity.y == 0)
        {
            // Jumping
            if(Input.GetKey(KeyCode.Space))
            {
                playerRigidbody.AddForce(Vector2.up * playerRigidbody.mass * jumpStrength);
            }

            // Movement on the ground
            if(Input.GetKey(KeyCode.D))
            {
                playerRigidbody.velocity = Vector2.right * movementSpeedOnGround;
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                playerRigidbody.velocity = Vector2.left * movementSpeedOnGround;
            }
            else
            {
                playerRigidbody.velocity = Vector2.zero;
            }
        }
        else
        {
            // In-air movement
            if(Input.GetKey(KeyCode.D))
            {
                playerRigidbody.AddForce(Vector2.right * playerRigidbody.mass * movementSpeedInAir);
            }
            if(Input.GetKey(KeyCode.A))
            {
                playerRigidbody.AddForce(Vector2.left * playerRigidbody.mass * movementSpeedInAir);
            }
        }
    }
}
