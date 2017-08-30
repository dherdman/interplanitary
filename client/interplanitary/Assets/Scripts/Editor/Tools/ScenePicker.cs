
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneLoader
{
    const string MENU_ROOT = "Scenes/";
    const string PATH_ROOT = "Assets/Scenes/";

    [MenuItem(MENU_ROOT + "GameStart")]
    static void LoadGameStart()
    {
        OpenScene("GameStart.unity");
    }

    [MenuItem(MENU_ROOT + "Menu")]
    static void LoadMenu()
    {
        OpenScene("MenuScene.unity");
    }

    [MenuItem(MENU_ROOT + "Game")]
    static void LoadGame()
    {
        OpenScene("Gameplay.unity");
    }

    static void OpenScene (string fileName)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(PATH_ROOT + fileName);
    }
}
