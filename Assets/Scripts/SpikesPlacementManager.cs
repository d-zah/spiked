using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesPlacementManager : MonoBehaviour
{

    public List<GameObject> spikePrefabs = new List<GameObject>(); 
    public List<GameObject> spikes = new List<GameObject>(); 
    private const float SPIKEX = 2.757577f;
    private const float SPIKEZ = 2.982507f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire2")) {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 10f, 1 << LayerMask.NameToLayer("Ground"))) {
                
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
                        spikes.Add(Instantiate(spikePrefabs[0], hit.point, Quaternion.identity));
                        // Debug.Log(hit.point);
                    }
                        
                }
                
            }
        }
    }
}
