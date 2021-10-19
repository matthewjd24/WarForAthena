using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingScreenManager : MonoBehaviour
{
    [SerializeField] List<GameObject> children = new List<GameObject>();

    void Start()
    {
        EventManager.RegisterResponse += GoToGame;
        EventManager.LoginResponse += GoToGame;

        foreach(Transform child in transform) {
            children.Add(child.gameObject);
        }

        StartCoroutine(WaitUntilConnected());
    }

    IEnumerator WaitUntilConnected()
    {
        yield return new WaitUntil(() => SslClient.sslStream != null);
        children[0].SetActive(false);
        children[1].SetActive(true);
    }

    private void GoToGame(string response)
    {
        if (response.Contains("Success")) {
            foreach (var e in children) {
                e.SetActive(false);
            }

            Debug.Log("Going to game");
        }
    }
}
