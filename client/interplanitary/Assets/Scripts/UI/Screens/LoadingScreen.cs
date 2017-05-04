using System.Collections;
using UnityEngine;
using core;

public class LoadingScreen : UIScreen
{
    public override ScreenName screenName
    {
        get
        {
            return ScreenName.Loading;
        }
    }

    [Header("Loading Screen")]
    [SerializeField]
    Transform cube;
    [SerializeField]
    Vector3 frameRotation;

    Coroutine LoadingSpinnerCoroutine;

    public override IEnumerator Init()
    {
        LoadingSpinnerCoroutine = StartCoroutine(RunSpinner());

        yield return null;
    }

    IEnumerator RunSpinner()
    {
        while (true)
        {
            cube.Rotate(frameRotation);
            yield return new WaitForEndOfFrame();
        }
    }

    protected override void OnStart()
    {
        // TODO needed?
    }

    protected override void OnUpdate()
    {
        // TODO needed?
    }

    protected override void OnExit()
    {
        StopCoroutine(LoadingSpinnerCoroutine);
    }
}
