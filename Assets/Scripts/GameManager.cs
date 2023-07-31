using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class GameManager : NetworkBehaviour
{
    public int highestSpikeID;
    public int purpleScore;
    public int yellowScore;

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

    public void resetRound(int winner){
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.layer == 8) {
                if(IsServer){
                    gameObj.GetComponent<NetworkObject>().Despawn();
                }
                Destroy(gameObj);
            }     
        }
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.tag == "Player"){ //assign to both players
                gameObj.SendMessage("resetPosition");
                gameObj.GetComponent<PlayerMovement>().isInPreGame = true;
                gameObj.SendMessage("invokePreGame");
                gameObj.transform.GetChild(3).GetComponent<SpikesPlacementManager>().placeDebugSpike();
                if(winner == 2){
                    TMP_Text textElement = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
                    yellowScore++;
                    textElement.color = new Color(255, 255, 0, 255);
                    textElement.text = "Yellow Scored!";
                    Invoke(nameof(resetWinnerText), 3f);
                } else {
                    TMP_Text textElement = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
                    purpleScore++;
                    textElement.color = new Color(130, 0, 190, 255);
                    textElement.text = "Purple Scored!";
                    Invoke(nameof(resetWinnerText), 3f);
                }
            }     
        }
        
    }
    

    public void resetWinnerText(){
        GameObject.Find("WinnerText").GetComponent<TMP_Text>().text = "";
    }
}
