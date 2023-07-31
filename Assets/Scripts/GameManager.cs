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
    public Sprite purple0, purple1, purple2, purple3, yellow0, yellow1, yellow2, yellow3;

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
        if(winner == 2){
            yellowScore++;
        } else {
            purpleScore++;
        }
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.tag == "Player"){ //assign to both players
                gameObj.SendMessage("resetPosition");
                gameObj.GetComponent<PlayerMovement>().isInPreGame = true;
                gameObj.SendMessage("invokePreGame");
                gameObj.transform.GetChild(3).GetComponent<SpikesPlacementManager>().placeDebugSpike();
                if(winner == 2){
                    TMP_Text textElement = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
                    textElement.color = Color.yellow;
                    textElement.text = "Yellow Scored!";
                    GameObject.Find("YellowScoreboard").GetComponent<Image>().sprite = (Sprite)this.GetType().GetField("yellow" + yellowScore).GetValue(this);
                    Invoke(nameof(resetWinnerText), 3f);
                } else {
                    TMP_Text textElement = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
                    textElement.color = Color.magenta;
                    textElement.text = "Purple Scored!";
                    GameObject.Find("PurpleScoreboard").GetComponent<Image>().sprite = (Sprite)this.GetType().GetField("purple" + purpleScore).GetValue(this);
                    Invoke(nameof(resetWinnerText), 3f);
                }
            }     
        }
        
    }
    

    public void resetWinnerText(){
        GameObject.Find("WinnerText").GetComponent<TMP_Text>().text = "";
    }
}
