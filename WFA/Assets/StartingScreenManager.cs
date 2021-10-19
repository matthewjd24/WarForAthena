using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingScreenManager : MonoBehaviour
{
    public static StartingScreenManager inst;

    [SerializeField] List<GameObject> children = new List<GameObject>();

    [SerializeField] GameObject inGameUI;

    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
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

    public void GoToGame()
    {
        foreach (var e in children) {
            e.SetActive(false);
        }

        inGameUI.SetActive(true);

        Debug.Log("Going to game");
    }
}
