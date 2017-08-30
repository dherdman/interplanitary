using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScreenName
{
    Splash,
    Loading,
    MainMenu,
    Settings,
    GameHud,
}

public class UIManager : Singleton<UIManager>
{
    List<UIScreen> InstantiatedScreens;
    Stack<UIScreen> OverlayStack;
    Stack<UIScreen> PrimaryScreenStack;

    UIScreen CurrentActiveScreen
    {
        get
        {
            return OverlayStack == null || OverlayStack.Count == 0 ? null : OverlayStack.Peek(); 
        }
    }

    void Start()
    {
        InstantiatedScreens = new List<UIScreen>();
        OverlayStack = new Stack<UIScreen>();
        PrimaryScreenStack = new Stack<UIScreen>();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(Scenes.Menu);

        ShowScreen(ScreenName.MainMenu);
    }

    public void StartGame ()
    {
        ShowScreen(ScreenName.Loading, () =>
        {
            SceneManager.LoadScene(Scenes.Gameplay);
            ShowScreen(ScreenName.GameHud);
        });
    }

    public void ShowScreen(ScreenName name, System.Action onScreenReady = null)
    {
        if (CurrentActiveScreen == null || CurrentActiveScreen.screenName != name)
        {
            DisplayScreen(ScreenName.Loading, () =>
            {
                if (name != ScreenName.Loading) // if final goal is to show a screen other than loading, load that screen
                {
                    DisplayScreen(name, () =>
                    {
                        ExitScreen(ScreenName.Loading);
                        if (onScreenReady != null)
                        {
                            onScreenReady();
                        }
                    });
                }
                else
                {
                    if (onScreenReady != null)
                    {
                        onScreenReady();
                    }
                }
            });
        }
    }

    void DisplayScreen(ScreenName name, System.Action onScreenReady = null)
    {
        UIScreen screen = GetScreenInstance(name);

        if (screen == null)
        {
            screen = Instantiate(GetCanvasResource(name), transform);
        }

        StartCoroutine(DisplayScreen(screen, onScreenReady));
    }

    IEnumerator DisplayScreen(UIScreen screenInstance, System.Action onScreenReady = null)
    {
        screenInstance.gameObject.SetActive(true); // ensure screen is active

        Canvas canvas = screenInstance.GetComponent<Canvas>();
        if (screenInstance.IsPrimaryScreen)
        {
            CloseAllScreens();
            canvas.worldCamera = CameraManager.instance.MainCamera;
        }
        else
        {
            canvas.worldCamera = CameraManager.instance.GetNewNamedOverlayCamera(screenInstance.screenName.ToString());
            canvas.worldCamera.depth = OverlayStack.Count;

            canvas.worldCamera.transform.position = new Vector3(0, (OverlayStack.Count + 1) * canvas.worldCamera.orthographicSize * 3);

            if (!screenInstance.IsLoadingScreen)
            {
                UIScreen loading = GetScreenInstance(ScreenName.Loading);
                if (loading != null && loading.isActiveAndEnabled)
                {
                    loading.GetComponent<Canvas>().worldCamera.depth = OverlayStack.Count + 1;
                }
            }
        }

        yield return StartCoroutine(screenInstance.Init());

        SetActiveScreen(screenInstance);

        if (onScreenReady != null)
        {
            onScreenReady();
        }
    }

    void SetActiveScreen(UIScreen screen, bool isNewScreen = true)
    {
        if (CurrentActiveScreen != null)
        {
            CurrentActiveScreen.MainCanvasGroup.interactable = false; // disable interaction on previous screen
        }

        if (isNewScreen)
        {
            if(!InstantiatedScreens.Contains(screen))
            {
                InstantiatedScreens.Add(screen);
            }

            if(screen.IsPrimaryScreen)
            {
                PrimaryScreenStack.Push(screen);
            }
            else if(!screen.IsLoadingScreen)
            {
                OverlayStack.Push(screen);
            }
        }

        screen.MainCanvasGroup.interactable = true;
    }

    /// <summary>
    /// Returns the first instantiated screen with the given name, regardless of if it is active
    /// </summary>
    UIScreen GetScreenInstance(ScreenName name)
    {
        for (int i = 0; i < InstantiatedScreens.Count; i++)
        {
            if (InstantiatedScreens[i].screenName == name)
            {
                return InstantiatedScreens[i];
            }
        }
        return null;
    }

    public void CloseScreen()
    {
        UIScreen curScreen = OverlayStack.Pop(); // remove current screen from stack and destroy it
        curScreen.Destroy();
        
        if (OverlayStack.Count > 0)
        {
            UIScreen nextScreen = OverlayStack.Peek();
            if (nextScreen.SkipBackNav)
            {
                CloseScreen();
            }
            else
            {
                SetActiveScreen(nextScreen, false);
            }
        }
    }

    void CloseAllScreens()
    {
        for (int i = InstantiatedScreens.Count - 1; i >= 0; i--)
        {
            if (InstantiatedScreens[i].isActiveAndEnabled && !InstantiatedScreens[i].IsLoadingScreen) // close all screens except loading
            {
                InstantiatedScreens[i].Exit();
            }

            if (!InstantiatedScreens[i].IsPersistentScreen) // purge non persistent screens from the list
            {
                InstantiatedScreens.RemoveAt(i);
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

    void ExitScreen(ScreenName name)
    {
        UIScreen screen = GetScreenInstance(name);

        if (screen != null && screen.isActiveAndEnabled)
        {
            screen.Exit();
        }
    }
}
