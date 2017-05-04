using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScreenName
{
    Splash,
    Loading,
    MainMenu
}

public class UIManager : Singleton<UIManager>
{
    List<UIScreen> ActiveScreens;
    UIScreen CurrentActiveScreen;

    void Start()
    {
        ActiveScreens = new List<UIScreen>();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");

        ShowScreen(ScreenName.MainMenu);
    }

    public void ShowScreen(ScreenName name, System.Action onScreenReady = null)
    {
        if(CurrentActiveScreen == null || CurrentActiveScreen.screenName != name)
        {
            DisplayScreen(ScreenName.Loading, () =>
            {
                if (name != ScreenName.Loading) // if final goal is to show a screen other than loading, load that screen
                {
                    DisplayScreen(name, () =>
                    {
                        ExitScreen(ScreenName.Loading);
                        if(onScreenReady != null)
                        {
                            onScreenReady();
                        }
                    });
                }
            });
        }
    }

    void DisplayScreen(ScreenName name, System.Action onScreenReady = null)
    {
        UIScreen screen = GetActiveScreen(name);

        if (screen == null)
        {
            screen = Instantiate(GetCanvasResource(name), transform);
        }

        StartCoroutine(DisplayScreen(screen, onScreenReady));
    }

    IEnumerator DisplayScreen(UIScreen screenInstance, System.Action onScreenReady = null)
    {
        if (screenInstance.IsPrimaryScreen)
        {
            CloseAllScreens();
            screenInstance.GetComponent<Canvas>().worldCamera = CameraManager.instance.MainCamera;
        }
        else 
        {
            screenInstance.GetComponent<Canvas>().worldCamera = CameraManager.instance.GetNewNamedOverlayCamera(screenInstance.screenName.ToString()); //string.Format("[Overlay] {0}", screenInstance.name);
        }

        yield return StartCoroutine(screenInstance.Init());

        SetActiveScreen(screenInstance);

        if (onScreenReady != null)
        {
            onScreenReady();
        }
    }

    void SetActiveScreen(UIScreen screen)
    {
        screen.PrevScreen = CurrentActiveScreen;
        CurrentActiveScreen = screen;
        ActiveScreens.Add(screen);

        screen.MainCanvasGroup.interactable = true;
    }

    /// <summary>
    /// Returns the first instantiated screen with the given name, regardless of if it is active
    /// </summary>
    UIScreen GetActiveScreen(ScreenName name)
    {
        for (int i = 0; i < ActiveScreens.Count; i++)
        {
            if (ActiveScreens[i].screenName == name)
            {
                return ActiveScreens[i];
            }
        }
        return null;
    }

    void CloseAllScreens()
    {
        for (int i = ActiveScreens.Count - 1; i >= 0; i--)
        {
            if (ActiveScreens[i].isActiveAndEnabled && ActiveScreens[i].screenName != ScreenName.Loading) // close all screens except loading
            {
                ActiveScreens[i].Exit();
            }
            if(!ActiveScreens[i].IsPersistentScreen) // purge non persistent screens from the list
            {
                ActiveScreens.RemoveAt(i);
            }
        }
    }

    UIScreen GetCanvasResource(ScreenName name)
    {
        return Resources.Load<UIScreen>(GetCanvasResourcePath(name));
    }

    string GetCanvasResourcePath(ScreenName name)
    {
        return string.Format("{0}{1}{2}", ClientConstants.BASE_CANVAS_RESOURCE_PATH, name.ToString(), ClientConstants.CANVAS_RESOURCE_SUFFIX);
    }

    void ExitScreen (ScreenName name)
    {
        UIScreen screen = GetActiveScreen(name);

        if(screen != null && screen.isActiveAndEnabled)
        {
            screen.Exit();
        }
    }
}
