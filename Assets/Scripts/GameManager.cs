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

            GameObject.FindObjectOfType<Canvas>(true).gameObject.SetActive(true);
            
            foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
                if(gameObj.tag == "Player"){ //assign to both players
                    if(alreadyAssignedPurple){
                        gameObj.GetComponent<PlayerMovement>().team = 2;
                    } else {
                        gameObj.GetComponent<PlayerMovement>().team = 1;
                        alreadyAssignedPurple = true;
                    }
                    gameObj.GetComponent<PlayerMovement>().isInGame = true; //in game
                    gameObj.transform.GetChild(3).GetComponent<SpikesPlacementManager>().state = 1; //set to spike state
                    gameObj.SendMessage("resetPosition");
                    gameObj.GetComponent<PlayerMovement>().isInPreGame = true;
                    gameObj.SendMessage("invokePreGame");
                }
                
            }
            
            GameObject go = GameObject.Find("WaitingText");
            if(go) {
                go.SetActive(false);
            }
            
        }
        
    }

    public void resetRound(){
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.tag == "Player"){ //assign to both players
                gameObj.SendMessage("resetPosition");
                gameObj.GetComponent<PlayerMovement>().isInPreGame = true;
                gameObj.SendMessage("invokePreGame");
            }     
        }
    }
}
