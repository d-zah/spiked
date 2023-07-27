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
            foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
                if(gameObj.tag == "Player"){
                    //Game Start
                    gameObj.GetComponent<PlayerMovement>().isInGame = true;
                    GameObject go = GameObject.Find("WaitingText");
                    if (go) {
                        Debug.Log(go.name);
                    } else {
                        Debug.Log("there is no go");
                    }
                }
            }
        }
        
    }
}
