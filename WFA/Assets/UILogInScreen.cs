using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogInScreen : MonoBehaviour
{
    public static UILogInScreen inst;

    [SerializeField] InputField userinp;
    [SerializeField] InputField passinp;
    public Text status;

    [SerializeField] string response = null;


    private void Awake()
    {
        inst = this;
        response = null;
        EventManager.MsgReceived += EventManager_MsgReceived;
    }

    private void OnDisable()
    {
        EventManager.MsgReceived -= EventManager_MsgReceived;
    }

    private void EventManager_MsgReceived(string[] msg)
    {
        if (msg[0] != "login") return;

        response = msg[1];
    }

    public void LogIn()
    {
        StartCoroutine(TryLogIn());
    }

    IEnumerator TryLogIn()
    {

        response = null;
        float secs = 0;
        status.text = "Logging in...";

        string name = userinp.text;
        string pass = passinp.text;

        _ = NetMsg.SendMsg($"login;{name};{pass};");

        while(response == null) {
            yield return new WaitForEndOfFrame();
            secs += Time.deltaTime;

            if (secs > 6) {
                Debug.Log("Login timeout");
                status.text = "Timed out";
                yield break;
            }
        }

        Debug.Log("Login response: " + response);

        status.text = response;
        if (response.Contains("Success")) {
            PlayerID.isLoggedIn = true;
            StartingScreenManager.inst.GoToGame();
        }

        response = null;
    }
}
