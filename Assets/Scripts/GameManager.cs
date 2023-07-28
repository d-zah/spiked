using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public int highestSpikeID;

    void Start() {
        highestSpikeID = 0;
    }

    void increaseHighestSpikeID() {
        highestSpikeID++;
    }

    public void checkForGame(){
        int players = 0;
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.tag == "Player"){
                players++;
            }
        }
        if(players == 2) {
            //Game Start
            bool alreadyAssignedPurple = false;
            foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
                if(gameObj.tag == "Player"){
                    if(alreadyAssignedPurple){
                        gameObj.GetComponent<PlayerMovement>().team = 2;
                    } else {
                        gameObj.GetComponent<PlayerMovement>().team = 1;
                        alreadyAssignedPurple = true;
                    }
                    gameObj.GetComponent<PlayerMovement>().isInGame = true;
                    gameObj.transform.GetChild(3).GetComponent<SpikesPlacementManager>().state = 1;
                }
                
            }
            
            GameObject go = GameObject.Find("WaitingText");
            if(go) {
                go.SetActive(false);
            }
            GameObject.FindObjectOfType<Canvas>(true).gameObject.SetActive(true);
        }
        
    }
}
