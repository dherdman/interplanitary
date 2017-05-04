using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    public abstract ScreenName screenName
    {
        get;
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
    public UIScreen PrevScreen;


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
        currentScreenState = ScreenState.Exit;
        OnExit();

        Destroy();
    }

    /// <summary>
    /// Only to be called from UIManager
    /// </summary>
    void Destroy()
    {
        Canvas screenCanvas = GetComponent<Canvas>();

        Camera cam = null;
        if(screenCanvas != null)
        {
            cam = screenCanvas.worldCamera;
        }

        if (screenCanvas != null && screenCanvas.worldCamera != null)
        {
            Destroy(screenCanvas.worldCamera.gameObject);
        }

        if (IsPersistentScreen)
        {
            gameObject.SetActive(false); // keep screen around, just hide it
            if(cam != null)
            {
                cam.gameObject.SetActive(false);
            }
        }
        else
        {
            if(cam != null)
            {
                Destroy(cam.gameObject);
            }
            Destroy(gameObject); // destroy self if non-persistent
        }
    }
}
