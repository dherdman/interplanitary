using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    void Start()
    {
        UIManager.instance.ShowScreen(ScreenName.Loading, (UIScreen loading) =>
        {
            SceneManager.LoadScene(Scenes.Gameplay);
            UIManager.instance.HideLoadingScreen();
        });
    }

    public Player CurrentPlayer
    {
        get; private set;
    }

    public GameHudScreen GameHud
    {
        get; private set;
    }

    public void RegisterPlayer(Player p)
    {
        CurrentPlayer = p;

        UIManager.instance.ShowScreen(ScreenName.GameHud, (UIScreen gameHud) =>
        {
            GameHud = (GameHudScreen)gameHud;
            SetupGameHud();
        });
    }

    public void SetupGameHud()
    {
        for (int i = 0; i < CurrentPlayer.CharacterInventory.Equipped.Weapons.Count; i++)
        {
            UpdateHudSlot(i);
        }
    }

    public void UpdateHudSlot(int i)
    {
        if (GameHud != null)
        {
            GameHud.UpdateWeaponSlot(i, CurrentPlayer.CharacterInventory.Equipped.IsWeaponSlotSelected(i), CurrentPlayer.CharacterInventory.Equipped.Weapons[i]);
        }
    }
}
