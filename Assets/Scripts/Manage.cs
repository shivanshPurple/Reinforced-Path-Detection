using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage : MonoBehaviour
{
    public GameObject[] UI;

    private bool showStats = true;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (showStats == true)
                showStats = false;
            else
                showStats = true;
        }

        foreach (GameObject ui in UI)
        {
            ui.SetActive(showStats);
        }
    }
}
