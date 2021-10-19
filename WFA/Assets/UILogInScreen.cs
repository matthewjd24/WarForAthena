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


    private void Awake()
    {
        EventManager.LoginResponse += LoginResponse;
    }

    private void LoginResponse(string response)
    {
        Debug.Log(response);
        status.text = response;
    }

    public void TryLogIn()
    {
        string name = userinp.text;
        string pass = passinp.text;

        new NetMsg.Login() {
            username = name,
            password = pass,
        }.Send();
    }
}
