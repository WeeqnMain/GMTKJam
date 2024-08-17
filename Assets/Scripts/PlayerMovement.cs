using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerVisuals playerVisuals;

    // Movement on ground
    [SerializeField] float movementSpeedOnGround;     // How fast you can move on ground
    [SerializeField] float jumpStrength;     // How hight you can jump

    // Movement in air
    [SerializeField] float movementSpeedInAir;     // How fast you move in air while not flying

    // Flight
    [SerializeField] float maxFlyingSpeed;     // The max speed at which you can fly (in any direction, at any moment)
    [SerializeField] float thrustLift;     // How much lift thrusting gives you
    [SerializeField] float passiveAcceleration;     // How fast the player accelerates passively
    [SerializeField] float gravityScaleWhileFlying;    // The gravity scale while the player is flying


    Rigidbody2D rigidbody;

    bool isInAir = false;
    bool isFlying = false;

    float flyingDir;     // 1 if the player is flying right and -1 if the player is flying left

    // ------------------------------------------------------------------------
    // Unity functions
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        // Disable player rotation
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // Checking, whether the player is on the ground
        isInAir = rigidbody.velocity.y != 0;

        // Processing movement
        if(isFlying) fly();
        else if(isInAir) moveInAir();
        else moveOnGround();
    }

    // ------------------------------------------------------------------------
    // This function is called every frame the player is on the ground
    void moveOnGround()
    {
        // Movement on ground
        if(Input.GetKey(KeyCode.D))
        {
            playerVisuals.SetDirectionRight();
            rigidbody.velocity = Vector2.right * movementSpeedOnGround;
        }
        else if(Input.GetKey(KeyCode.A))
        {
            playerVisuals.SetDirectionLeft();
            rigidbody.velocity = Vector2.left * movementSpeedOnGround;
        }
        else
        {
            rigidbody.velocity = Vector2.zero;
        }

        // Jumping
        if(Input.GetKeyDown(KeyCode.Space) && !isInAir)
        {
            rigidbody.AddForce(Vector2.up * rigidbody.mass * jumpStrength);
        }
    }

    // ------------------------------------------------------------------------
    // This function is called every frame the player is in the air, but not flying
    void moveInAir()
    {
        // Movement in air
        if(Input.GetKey(KeyCode.D))
        {
            playerVisuals.SetDirectionRight();
            rigidbody.AddForce(Vector2.right * rigidbody.mass * movementSpeedInAir);
        }
        else if(Input.GetKey(KeyCode.A))
        {
            playerVisuals.SetDirectionLeft();
            rigidbody.AddForce(Vector2.left * rigidbody.mass * movementSpeedInAir);
        }

        // Start flying if the player pressed space in air
        if(Input.GetKeyDown(KeyCode.Space)) startFlying();
    }

    // ------------------------------------------------------------------------
    // This function is called every frame the player is flying
    void fly()
    {
        // Stop flying if the player is on the ground or presed shift
        if(!isInAir || Input.GetKeyDown(KeyCode.LeftShift))
        {
            stopFlying(); 
            return;
        }

        // Getting the input
        if(Input.GetKey(KeyCode.D)) flyingDir = 1f;
        else if(Input.GetKey(KeyCode.A)) flyingDir = -1f;

        // Passive acceleration
        rigidbody.AddForce(Vector2.right * passiveAcceleration * flyingDir);

        // Thrusting on space
        if(Input.GetKey(KeyCode.Space)) rigidbody.AddForce(Vector2.up * thrustLift); 

        // Limiting the max flying speed
        if(rigidbody.velocity.magnitude > maxFlyingSpeed) rigidbody.velocity = rigidbody.velocity.normalized * maxFlyingSpeed;


        // Visuals
        if(flyingDir == 1f) playerVisuals.SetDirectionRight();
        if(flyingDir == -1f) playerVisuals.SetDirectionLeft();
    }

    // ------------------------------------------------------------------------
    // These functions begin and end flight 
    // WARNING: these aren't called automatically
    void startFlying()
    {
        flyingDir = rigidbody.velocity.normalized.x;
        isFlying = true;

        rigidbody.gravityScale = gravityScaleWhileFlying;
    }
    void stopFlying()
    {
        isFlying = false;

        rigidbody.gravityScale = 1f;
    }
}
