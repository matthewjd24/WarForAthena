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

    private void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        status.text = "";
    }

    public void SetStatusText(string response)
    {
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
