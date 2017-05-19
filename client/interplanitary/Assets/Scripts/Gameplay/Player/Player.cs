using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    void Update()
    {

    }

    /// <summary>
    /// Toggles this player's controller enabled/disabled
    /// </summary>
    /// <returns>New enabled state of controller</returns>
    public void ToggleControl ()
    {
        if(playerController != null)
        {
            if(playerController.enabled)
            {
                playerController.DisableSelf();
            } 
            else
            {
                playerController.enabled = true;
            }
        }
    }
}
