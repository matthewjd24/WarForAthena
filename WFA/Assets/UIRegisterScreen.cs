using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegisterScreen : MonoBehaviour
{
    public static UIRegisterScreen inst;

    [SerializeField] InputField userinp;
    [SerializeField] InputField passinp;
    [SerializeField] InputField emailinp;

    [SerializeField] Text status;

    string response = null;

    private void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        status.text = "";
        response = null;
        EventManager.MsgReceived += EventManager_MsgReceived;
    }

    private void OnDisable()
    {
        EventManager.MsgReceived -= EventManager_MsgReceived;
    }

    private void EventManager_MsgReceived(string[] msg)
    {
        if (msg[0] != "register") return;
        response = msg[1];
    }

    public void StartRegister()
    {
        StartCoroutine(TryRegister());
    }

    IEnumerator TryRegister()
    {
        response = null;
        float secs = 0;
        status.text = "Registering...";

        string name = userinp.text;
        string pass = passinp.text;
        string email = emailinp.text;

        _ = NetMsg.SendMsg($"register;{name};{pass};{email}");

        while (response == null) {
            yield return new WaitForEndOfFrame();
            secs += Time.deltaTime;

            if (secs > 6) {
                Debug.Log("Register timeout");
                status.text = "Timed out";
                yield break;
            }
        }

        Debug.Log("Register response: " + response);

        status.text = response;
        if (response.Contains("Success")) {
            PlayerID.isLoggedIn = true;
            StartingScreenManager.inst.GoToGame();
        }

        response = null;
    }
}
