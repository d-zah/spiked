using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
                        gameObj.GetComponent<PlayerMovement>().team = PlayerTeam.YELLOW;
                    } else {
                        gameObj.GetComponent<PlayerMovement>().team = PlayerTeam.PURPLE;
                        alreadyAssignedPurple = true;
                    }
                    gameObj.GetComponent<PlayerMovement>().isInGame = true; //in game
                    gameObj.GetComponent<PlayerMovement>().isInPreGame = true;
                    gameObj.transform.GetChild(3).GetComponent<SpikesPlacementManager>().state = 1; //set to spike state
                    gameObj.SendMessage("resetPosition");
                    gameObj.SendMessage("invokePreGame");
                }
                
            }
            
            GameObject go = GameObject.Find("WaitingText");
            if(go) {
                go.SetActive(false);
            }
            
        }
        
    }

    public void resetRound(PlayerTeam winner){
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.layer == 8) {//spike layer
                if(IsServer){
                    gameObj.GetComponent<NetworkObject>().Despawn();
                }
                Destroy(gameObj);
            }     
        }

        if(winner == PlayerTeam.YELLOW){
            TMP_Text textElement = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
            textElement.color = Color.yellow;
            yellowScore++;
            if(yellowScore == 3){
                textElement.text = "Yellow Wins!";
            } else {
                textElement.text = "Yellow Scored!";
            }
            GameObject.Find("YellowScoreboard").GetComponent<Image>().sprite = (Sprite)this.GetType().GetField("yellow" + yellowScore).GetValue(this);
            Invoke(nameof(resetWinnerText), 3f);
        } else if(winner == PlayerTeam.PURPLE) {
            TMP_Text textElement = GameObject.Find("WinnerText").GetComponent<TMP_Text>();
            textElement.color = Color.magenta;
            purpleScore++;
            if(purpleScore == 3){
                textElement.text = "Purple Wins!";
            } else {
                textElement.text = "Purple Scored!";
            }
            GameObject.Find("PurpleScoreboard").GetComponent<Image>().sprite = (Sprite)this.GetType().GetField("purple" + purpleScore).GetValue(this);
            Invoke(nameof(resetWinnerText), 3f);
        }

        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.tag == "Player"){ //assign to both players
                gameObj.GetComponent<PlayerMovement>().isInPreGame = true;
                gameObj.SendMessage("resetPosition");
                if(purpleScore == 3 || yellowScore == 3){
                    Invoke(nameof(sendMenuServerRpc), 3.5f);
                } else {
                    gameObj.SendMessage("invokePreGame");
                    gameObj.transform.GetChild(3).GetComponent<SpikesPlacementManager>().placeDebugSpike();
                }
                
                
            }     
        }
        
    }
    

    public void resetWinnerText(){
        GameObject.Find("WinnerText").GetComponent<TMP_Text>().text = "";
    }

    [ServerRpc(RequireOwnership = false)]
    public void sendMenuServerRpc(){
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>()){
            if(gameObj.tag == "Player"){ //assign to both players
                
                Destroy(NetworkManager.Singleton.gameObject);
            }
        }
        sendMenuClientRpc();
    }

    [ClientRpc]
    public void sendMenuClientRpc(){
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        Cursor.lockState = CursorLockMode.None;
    }


}
