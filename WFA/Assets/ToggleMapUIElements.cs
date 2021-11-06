using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMapUIElements : MonoBehaviour
{
    [SerializeField] List<GameObject> items;

    private void OnEnable()
    {
        foreach(var e in items) {
            e.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach(var e in items) {
            e.SetActive(false);
        }
    }
}
