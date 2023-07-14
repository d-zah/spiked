using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesPlacementManager : MonoBehaviour
{

    public List<GameObject> spikePrefabs = new List<GameObject>(); 
    public List<GameObject> spikes = new List<GameObject>(); 
    public const float SPIKEX = 2.757577f;
    public const float SPIKEZ = 2.982507f;
    public GameObject foundObject;
    public GameObject foundPlatform;
    public int spikeID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire2")) {
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
                        Vector3 fixedPos = new Vector3(hit.point);
                        foundPlatform = GameObject.Find(hit.collider.gameObject.name + "SpikeBounds");
                        fixedPos = foundPlatform.GetComponent<Collider>().ClosestPoint(hit.point);

                        GameObject newSpike = Instantiate(spikePrefabs[0], fixedPos, Quaternion.identity);
                        newSpike.name = "spike" + spikeID;
                        spikeID++;
                        spikes.Add(newSpike);

                        // Debug.Log(hit.point);
                    }
                        
                }
                
            }
        }
        if(Input.GetButton("Fire1")) {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 15f, 1 << LayerMask.NameToLayer("Spikes"))) {
                
                if(spikes.Count != 0){
                    foundObject = GameObject.Find(hit.collider.gameObject.name);
                    Destroy(foundObject);
                    spikes.Remove(foundObject);

                }       
            }
                // if(check){
                //     spikes.Add(Instantiate(spikePrefabs[0], hit.point, Quaternion.identity));
                //     // Debug.Log(hit.point);
                // }
                        
                
                
        }
    }
}

