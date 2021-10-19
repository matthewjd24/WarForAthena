using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    public static PlayerID inst;
    public static string accountName;
    public static bool isLoggedIn;

    private void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        EventManager.LoginResponse += LogIn;
        EventManager.DCed += Disconnected;
    }

    void LogIn(string response)
    {
        if (response.Contains("Success")) {
            isLoggedIn = true;
        }
        else {
            isLoggedIn = false;
        }
    }

    private void Disconnected()
    {
        accountName = "";
        isLoggedIn = false;
    }
}
