using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public Transform playerTransform;
    private float playerSpeed;
    private float normalHeight;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = 0.05f;
        normalHeight = 0.5f;
        playerTransform.position = new Vector3 (0f, normalHeight, -10f);
    }

    // Update is called once per frame
    void Update()
    {
        float currentX = playerTransform.position.x;
        float currentZ = playerTransform.position.z;

        if (Input.GetKey (KeyCode.UpArrow)) {
            playerTransform.position = new Vector3 (currentX, normalHeight, currentZ + playerSpeed);
        }

        if (Input.GetKey (KeyCode.LeftArrow)) {
            playerTransform.position = new Vector3 (currentX - playerSpeed, normalHeight, currentZ);
        }
    }
}
