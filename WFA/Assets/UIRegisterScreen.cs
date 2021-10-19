using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegisterScreen : MonoBehaviour
{
    [SerializeField] InputField userinp;
    [SerializeField] InputField passinp;
    [SerializeField] InputField emailinp;

    [SerializeField] Text status;

    private void OnEnable()
    {
        status.text = "";
        EventManager.RegisterResponse += RegisterResponse;
    }

    private void RegisterResponse(string response)
    {
        Debug.Log(response);
        status.text = response;
    }

    public void Register()
    {
        string username = userinp.text;
        string pass = passinp.text;
        string email = emailinp.text;

        new NetMsg.Register() {
            username = username,
            password = pass,
            email = email,
        }.Send();
        status.text = "Registering...";
    }
}
