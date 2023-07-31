using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SpikesPlacementManager : NetworkBehaviour
{
    //spike vars
    public int state = 0;

    [Header("Spikes")]
    public List<GameObject> spikePrefabs = new List<GameObject>(); 
    public const float SPIKEX = 2.757577f;
    public const float SPIKEZ = 2.982507f;
    public SpikesManager foundName;
    private int lastPlacedSpike;
    private bool readyForPlace = true;
    
    

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

    [Header("UI Elements")]
    public Sprite spikesIcon;
    public Sprite ballIcon;

    void Start(){
        placeDebugSpike();
    }

    public void placeDebugSpike(){
        if(!IsOwner) return;

        Vector3 initialSpike = new Vector3(100f, 0f, 0f);
        placeSpikeServerRpc(initialSpike);
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner || !Application.isFocused) return;

        if(!transform.parent.gameObject.GetComponent<PlayerMovement>().isInGame) return;

        if(Input.GetButton("Fire1")) {
            breakSpike();      
        } else if(Input.GetButton("Fire2")) {
            if(state == 1) {
                placeSpike();
            } else if (state == 2 && readyToThrow) {
                Throw();
            }
        }
        // if(Input.GetButton("Slot1")) { //select spikes
        //     state = 1;
        //     GameObject.Find("CurrentIcon").GetComponent<Image>().sprite = spikesIcon;
        // } else if (Input.GetButton("Slot2")) { //select ball
        //     state = 2;
        //     GameObject.Find("CurrentIcon").GetComponent<Image>().sprite = ballIcon;
        // }
    }

    private void placeSpike() {

        if(!IsOwner) return;
        RaycastHit hit;
        
        if(!Physics.Raycast(transform.position, transform.forward, out hit, 15f, 1 << LayerMask.NameToLayer("Ground"))/* && GameObject.Find(hit.collider.gameObject.name).tag != "SpawnPlatform"*/) return;
                
        if(!(hit.normal == Vector3.up)) return;
        bool check = true;
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.layer == 8){
            
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
                if(lastPlacedSpike == GameObject.Find("GameManager").GetComponent<GameManager>().highestSpikeID && !readyForPlace){
                    check = false;
                    break;
                }
            }              
        }
        if(check) {
            lastPlacedSpike = GameObject.Find("GameManager").GetComponent<GameManager>().highestSpikeID;
            placeSpikeServerRpc(hit.point);
            readyForPlace = false;
        }
        
    }


    [ServerRpc(RequireOwnership = false)]
    public void placeSpikeServerRpc(Vector3 hitPoint) {
        
        //create spike client side
        GameObject newSpike = Instantiate(spikePrefabs[0], hitPoint, Quaternion.identity);
        newSpike.GetComponent<NetworkObject>().Spawn();
        
        int name = newSpike.GetComponent<SpikesManager>().spikeID;

        resetReadinessClientRpc();

    }

    [ClientRpc]
    public void resetReadinessClientRpc() {
        readyForPlace = true;
    }


    private void breakSpike() {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 15f, 1 << LayerMask.NameToLayer("Spikes"))) {
                
                foundName = hit.collider.gameObject.GetComponent<SpikesManager>();
                breakSpikeServerRpc(foundName.spikeID);
     
            }
    }

    [ServerRpc(RequireOwnership = false)]
    public void breakSpikeServerRpc(int foundID) {
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.layer == 8) {
                SpikesManager currentManager = gameObj.GetComponent<SpikesManager>();
                if (currentManager.spikeID == foundID) {
                    gameObj.GetComponent<NetworkObject>().Despawn();
                    Destroy(gameObj);
                    return;
                } 
            }
        }
    }

    private void Throw(){
        readyToThrow = false;

        ThrowBallServerRpc();

        Invoke(nameof(ResetThrow), throwCooldown);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ThrowBallServerRpc(){

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, cam.rotation);
        projectile.GetComponent<NetworkObject>().Spawn();
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        Vector3 forceToAdd = cam.transform.forward * throwForce + transform.up * throwUpwardForce;

        projectileRb.AddForce(forceToAdd, ForceMode.Impulse);
               
        
    }

    private void ResetThrow() {
        readyToThrow = true;
    }


}

