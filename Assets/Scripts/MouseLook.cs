using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MouseLook : NetworkBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Camera mainCamera;
    float xRotation = 0f;

    private void Initialize() {
        mainCamera = Camera.main;
    }

    public override void OnNetworkSpawn() {
        if(!IsOwner){
            
            //Destroy(this.gameObject);
            this.GetComponent<Camera>().enabled = false;
            this.GetComponent<AudioListener>().enabled = false;
            this.enabled = false;
        } else {
            base.OnNetworkSpawn();
            Initialize();
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner || !Application.isFocused) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 0.001f;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 0.001f;

        // * Time.deltaTime
    
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

}
