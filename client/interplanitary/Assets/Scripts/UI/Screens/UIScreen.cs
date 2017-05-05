using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    public abstract ScreenName screenName
    {
        get;
    }

    public bool IsLoadingScreen
    {
        get
        {
            return screenName == ScreenName.Loading;
        }
    }

    [Header("UI Screen Configuration")]
    [SerializeField]
    bool _isPrimaryScreen;
    public bool IsPrimaryScreen
    {
        get
        {
            return _isPrimaryScreen;
        }
    }

    [SerializeField]
    bool _skipBackNav;
    public bool SkipBackNav
    {
        get
        {
            return _skipBackNav;
        }
    }

    [SerializeField]
    bool _isPersistentScreen;
    public bool IsPersistentScreen
    {
        get
        {
            return _isPersistentScreen;
        }
    }

    [HideInInspector]
    public CanvasGroup MainCanvasGroup;

    /// <summary>
    /// Exposes the MainCanvasGroup's interactability state 
    /// </summary>
    public bool IsInteractable
    {
        get
        {
            return MainCanvasGroup != null && MainCanvasGroup.interactable;
        }
    }

    enum ScreenState
    {
        Start,
        Update,
        Exit
    }
    ScreenState currentScreenState;

    void Start()
    {
        MainCanvasGroup = GetComponent<CanvasGroup>();

        currentScreenState = ScreenState.Start;
    }

    void Update()
    {
        if (currentScreenState == ScreenState.Start)
        {
            OnStart();
            currentScreenState = ScreenState.Update;
        }
        else if (currentScreenState == ScreenState.Update)
        {
            OnUpdate();
        }
    }

    public abstract IEnumerator Init();
    protected abstract void OnStart();
    protected abstract void OnUpdate();
    protected abstract void OnExit();

    public void Exit()
    {
        Debug.Log("Exiting " + screenName);
        currentScreenState = ScreenState.Exit;
        OnExit();
        
        // if not a loading screen, go through UIManager to process screen closing
        if(IsLoadingScreen)
        {
            Destroy();
        }
        else
        {
            UIManager.instance.CloseScreen();
        }
    }

    /// <summary>
    /// Only to be called from UIManager
    /// </summary>
    public void Destroy()
    {
        if(IsPersistentScreen)
        {
            // hide screen and make it non-interactable
            MainCanvasGroup.interactable = false;
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject); // if non persistent, destroy screen gameobject
        }

        Canvas screenCanvas = GetComponent<Canvas>();
        Camera cam = null;
        if (screenCanvas != null)
        {
            cam = screenCanvas.worldCamera;
        }

        DisposeOfCamera(!IsPersistentScreen, cam);
    }

    void DisposeOfCamera (bool destroyNotHide, Camera cam)
    {
        if(cam != null && cam != Camera.main && cam.gameObject != null)
        {
            if(destroyNotHide)
            {
                Destroy(cam.gameObject);
            }
            else
            {
                cam.gameObject.SetActive(false);
            }
        }
    }
}
