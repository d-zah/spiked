using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : NetworkBehaviour
{

    public CharacterController controller;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float walkingSpeed = 12f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float jumpSpeed = 20f;
    // public float jumpSpeedDecay = .1f;

    public Transform groundCheck;
    public Transform playerTransform;
    public LayerMask groundMask;
    public TMP_Text textElement;

    Vector3 velocity;
    public bool isInGame;
    public bool isInPreGame = false;
    public int team = 0; //0 = spec, 1 = Purple, 2 = Yellow
    
    bool isGrounded;
    bool isSprinting;
    [SerializeField] private int consecutiveJumps = 0;

    void Start(){
        
        isInGame = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().checkForGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner || !Application.isFocused || isInPreGame) return;

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0){
            velocity.y = 0f;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if(z > 0f) {
            isSprinting = true;
        } else {
            isSprinting = false;
        }

        Vector3 move;

        if(isGrounded){
            move = transform.right * x + transform.forward * z;
        } else if (isSprinting) {
            move = transform.right * x/2 + transform.forward * z;
        } else {
            move = transform.right * x/2 + transform.forward * z/2;
            consecutiveJumps = 0;
        }

        controller.Move(move * speed * Time.deltaTime);
        
        

        if(Input.GetButton("Jump") && isGrounded) { //used GetButton so that you can hold down space
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            consecutiveJumps++;
            if (isSprinting) { //only increase speed if player is actually moving
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
        if(team == 1) {
            playerTransform.position = new Vector3(74f, 3f, 0f);
            transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
        } else if(team == 2) {
            playerTransform.position = new Vector3(-74f, 3f, 0f);
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        } else {
            playerTransform.position = new Vector3(-5f, 3f, 0f);
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }
        
        controller.enabled = true;  
    }

    

    void invokePreGame(){
        if(!textElement){
            textElement = GameObject.Find("Countdown").GetComponent<TMP_Text>();
        }
        textElement.text = "3";
        Invoke(nameof(countdown2), 1.0f);
        Invoke(nameof(countdown1), 2.0f);
        Invoke(nameof(disablePreGame), 3f);
        Invoke(nameof(resetCountdown), 4.0f);
    }

    void countdown2(){
        textElement.text = "2";
    }

    void countdown1(){
        textElement.text = "1";
    }

    void disablePreGame(){
        isInPreGame = false;
        textElement.text = "Go!";
    }

    void resetCountdown(){
        textElement.text = "";
    }
}
