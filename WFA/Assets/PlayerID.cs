using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerID : MonoBehaviour
{
    public static PlayerID inst;
    public static string accountName;
    public static bool isLoggedIn;
    public static int currentWorld;

    private void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        EventManager.DCed += Disconnected;
    }

    private void Disconnected()
    {
        accountName = "";
        isLoggedIn = false;
    }
}
