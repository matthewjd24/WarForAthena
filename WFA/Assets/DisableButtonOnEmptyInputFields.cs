using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableButtonOnEmptyInputFields : MonoBehaviour
{
    [SerializeField] List<InputField> inputFields;

    Button myB;

    private void OnEnable()
    {
        myB = GetComponent<Button>();
        Check();
    }

    void Update()
    {
        Check();
    }

    void Check()
    {
        bool anyEmpty = false;
        foreach (var e in inputFields) {
            if (e.text == "") anyEmpty = true;
        }


        bool shouldBeEnabled = true;
        if (anyEmpty || PlayerID.isLoggedIn) shouldBeEnabled = false;


        if (!shouldBeEnabled && myB.interactable) {
            myB.interactable = false;
            Canvas.ForceUpdateCanvases();
        }
        else if (shouldBeEnabled && !myB.interactable) {
            myB.interactable = true;
            Canvas.ForceUpdateCanvases();
        }

    }
}
