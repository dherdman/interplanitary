
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneLoader
{
    const string MENU_ROOT = "Scenes/";
    const string PATH_ROOT = "Assets/Scenes/";

    [MenuItem(MENU_ROOT + "GameStart")]
    static void LoadGameStart()
    {
        EditorSceneManager.OpenScene(PATH_ROOT + "GameStart.unity");
    }

    [MenuItem(MENU_ROOT + "Menu")]
    static void LoadMenu()
    {
        EditorSceneManager.OpenScene(PATH_ROOT + "MenuScene.unity");
    }

    [MenuItem(MENU_ROOT + "Game")]
    static void LoadGame()
    {
        EditorSceneManager.OpenScene(PATH_ROOT + "Gameplay.unity");
    }
}
