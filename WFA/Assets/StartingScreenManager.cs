using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingScreenManager : MonoBehaviour
{
    public static StartingScreenManager inst;

    [SerializeField] GameObject connect;
    [SerializeField] GameObject login;
    [SerializeField] GameObject register;
    [SerializeField] GameObject dced;

    List<GameObject> screens;

    [SerializeField] GameObject Game;

    private void Awake()
    {
        inst = this;
        screens = new List<GameObject>() {
            connect,
            login,
            register,
            dced
        };
    }

    void Start()
    {
        foreach(var e in screens) {
            e.SetActive(false);
        }
        connect.SetActive(true);

        StartCoroutine(WaitUntilConnected());
    }

    IEnumerator WaitUntilConnected()
    {
        yield return new WaitUntil(() => SslClient.sslStream != null);
        connect.SetActive(false);
        login.SetActive(true);
    }

    public void GoToGame()
    {
        PlayerID.currentWorld = 1;

        transform.parent.gameObject.SetActive(false);
        Game.SetActive(true);

        Debug.Log("Going to game");
    }
}
