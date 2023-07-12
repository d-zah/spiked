using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public float speed = 12f;
    public float walkingSpeed = 12f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public float groundDistance = 0.4f;
    public float jumpSpeed = 20f;
    // public float jumpSpeedDecay = .1f;

    public Transform groundCheck;
    public Transform playerTransform;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;
    private int consecutiveJumps = 0;

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0){
            velocity.y = 0f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 move;

        if(isGrounded){
            move = transform.right * x + transform.forward * z;
        } else if (z > 0f) {
            move = transform.right * x/2 + transform.forward * z;
        } else {
            move = transform.right * x/2 + transform.forward * z/2;
        }

        controller.Move(move * speed * Time.deltaTime);
        
        

        if(Input.GetButton("Jump") && isGrounded) { //used GetButton so that you can hold down space
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            consecutiveJumps++;
            if (z != 0) { //only increase speed if player is actually moving
                if (speed < jumpSpeed) { //gradually increase speed while chaining jumps
                    speed = walkingSpeed + 2 + Mathf.Sqrt(consecutiveJumps);
                } else { //clamp jump speed to 20
                    speed = jumpSpeed;
                }
            } else {
                speed = walkingSpeed;
                consecutiveJumps = 0;
            }
            
            
        } else if (isGrounded) {
            consecutiveJumps = 0;
            speed = walkingSpeed;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    void resetPosition() {
        speed = walkingSpeed;
        consecutiveJumps = 0;
        controller.enabled = false; //have to disable controller in order to adjust position
        playerTransform.position = new Vector3(0f, 10f, 0f);
        controller.enabled = true;  
    }
}
