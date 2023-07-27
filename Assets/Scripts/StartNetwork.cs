using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class StartNetwork : MonoBehaviour
{
    public void StartServer() {
        GameObject.Find("TempCamera").GetComponent<Camera>().enabled = false;
        GameObject.Find("TempCamera").GetComponent<AudioListener>().enabled = false;
        NetworkManager.Singleton.StartServer();
    }

    public void StartClient() {
        GameObject.Find("TempCamera").GetComponent<Camera>().enabled = false;
        GameObject.Find("TempCamera").GetComponent<AudioListener>().enabled = false;
        NetworkManager.Singleton.StartClient();
    }

    public void StartHost() {
        GameObject.Find("TempCamera").GetComponent<Camera>().enabled = false;
        GameObject.Find("TempCamera").GetComponent<AudioListener>().enabled = false;
        NetworkManager.Singleton.StartHost();
    }
}
