using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    // Use this for initialization
    void Start()
    {
        //UIManager.instance.GoToMainMenu();
        UIManager.instance.StartGame();
    }

    // Update is called once per frame
    void Update()
    {

    }

}
