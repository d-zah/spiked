using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesPlacementManager : MonoBehaviour
{
    //spike vars
    public int state = 0;

    [Header("Spikes")]
    public List<GameObject> spikePrefabs = new List<GameObject>(); 
    public List<GameObject> spikes = new List<GameObject>(); 
    public const float SPIKEX = 2.757577f;
    public const float SPIKEZ = 2.982507f;
    public GameObject foundObject;
    public int spikeID;

    //projectile vars
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start() {
        readyToThrow = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetButton("Fire1")) {
            breakSpike();      
        } else if(Input.GetButton("Fire2")) {
            if(state == 0) {
                placeSpike();
            } else if (state == 1 && readyToThrow) {
                Throw();
            }
        }
        if(Input.GetButton("Slot1")) {
            state = 0;
        } else if (Input.GetButton("Slot2")) {
            state = 1;
        }
    }
    private void placeSpike() {
        RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 15f, 1 << LayerMask.NameToLayer("Ground"))) {
                
                if(hit.normal == Vector3.up){
                    bool check = true;
                    foreach(GameObject spike in spikes){
                        float xDiff = Mathf.Abs(spike.transform.position.x - hit.point.x);
                        float zDiff = Mathf.Abs(spike.transform.position.z - hit.point.z);
                        if(xDiff < 2 * SPIKEX && zDiff < SPIKEZ){
                            check = false;
                            break;
                        }
                        if(xDiff < SPIKEX && zDiff < 2 * SPIKEZ){
                            check = false;
                            break;
                        }
                        
                    }
                    if(check){
                    
                        GameObject newSpike = Instantiate(spikePrefabs[0], hit.point, Quaternion.identity);
                        newSpike.name = "spike" + spikeID;
                        spikeID++;
                        spikes.Add(newSpike);

                        // Debug.Log(hit.point);
                    }
                        
                }
                
            }
    }

    private void breakSpike() {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 15f, 1 << LayerMask.NameToLayer("Spikes"))) {
                
                if(spikes.Count != 0){
                    foundObject = GameObject.Find(hit.collider.gameObject.name);
                    Destroy(foundObject);
                    spikes.Remove(foundObject);

                }       
            }
    }
    private void Throw(){
        readyToThrow = false;

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    private void ResetThrow() {
        readyToThrow = true;
    }


}

