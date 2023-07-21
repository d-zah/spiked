using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpikesPlacementManager : NetworkBehaviour
{
    //spike vars
    public int state = 0;

    [Header("Spikes")]
    public List<GameObject> spikePrefabs = new List<GameObject>(); 
    public List<SpikesManager> spikeNames = new List<SpikesManager>(); 
    public const float SPIKEX = 2.757577f;
    public const float SPIKEZ = 2.982507f;
    public SpikesManager foundName;
    
    

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

    bool readyToThrow = true;



    // Update is called once per frame
    void Update()
    {
        if(!IsOwner || !Application.isFocused) return;

        

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

        if(!IsOwner) return;
        RaycastHit hit;
        
        if(!Physics.Raycast(transform.position, transform.forward, out hit, 15f, 1 << LayerMask.NameToLayer("Ground"))/* && GameObject.Find(hit.collider.gameObject.name).tag != "SpawnPlatform"*/) return;
                
        if(!(hit.normal == Vector3.up)) return;
        bool check = true;
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.layer == 8){
            
                // Debug.Log("Checking " + gameObj.GetComponent<SpikesManager>().spikeID);
                float xDiff = Mathf.Abs(gameObj.transform.position.x - hit.point.x);
                float zDiff = Mathf.Abs(gameObj.transform.position.z - hit.point.z);
                if(xDiff < 2 * SPIKEX && zDiff < SPIKEZ){
                    check = false;
                    break;
                }
                if(xDiff < SPIKEX && zDiff < 2 * SPIKEZ){
                    check = false;
                    break;
                }
            }              
        }
        if(check) placeSpikeServerRpc(hit.point);
            
        
            
    }


    [ServerRpc(RequireOwnership = false)]
    public void placeSpikeServerRpc(Vector3 hitPoint) {
        // RaycastHit spikeHit = new RaycastHit();
        // spikeHit.point = new Vector3(hitX, hitY, hitZ);

        
        //create spike client side
        // Debug.Log(hitPoint);
        GameObject newSpike = Instantiate(spikePrefabs[0], hitPoint, Quaternion.identity);
        newSpike.GetComponent<NetworkObject>().Spawn();
        
        
        int name = newSpike.GetComponent<SpikesManager>().spikeID;
        addSpikeClientRpc(name);
                        
        
    }

    [ClientRpc]
    public void addSpikeClientRpc(int name) {
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.layer == 8) {
                SpikesManager currGameObj = gameObj.GetComponent<SpikesManager>();
                if(currGameObj.spikeID == name) {
                    addSpikeToList(currGameObj);
                }      
                return;  
            }
                  
        }
    }

    private void addSpikeToList(SpikesManager spikeToAdd) {
        Debug.Log(spikeToAdd.spikeID + " Adding to List");
        spikeNames.Add(spikeToAdd);
    }


    private void breakSpike() {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 15f, 1 << LayerMask.NameToLayer("Spikes"))) {
                
                if(spikeNames.Count != 0){
                    foundName = hit.collider.gameObject.GetComponent<SpikesManager>();
                    Debug.Log(foundName.spikeID + " Found");
                    breakSpikeServerRpc(foundName.spikeID);
                    spikeNames.Remove(foundName);

                }       
            }
    }

    [ServerRpc(RequireOwnership = false)]
    public void breakSpikeServerRpc(int foundID) {
        foreach(SpikesManager currentSpike in spikeNames) {
            if (currentSpike.spikeID == foundID) {
                Debug.Log(currentSpike.spikeID + " Has been Broken");
                currentSpike.GetComponent<NetworkObject>().Despawn();
                Destroy(currentSpike);
                return;
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

