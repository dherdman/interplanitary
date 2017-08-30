
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

static class SYMBOLS
{
    public const string DEBUG_MODE = "DEBUG_MODE";
}

public class SymbolWindow : EditorWindow
{
    const string MENU_ROOT = "Tools/Symbols";

    static Dictionary<string, bool> symbols;

    [MenuItem(MENU_ROOT)]
    static void Init()
    {
        string[] symbolArray = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';');

        symbols = new Dictionary<string, bool>();
        for(int i = 0; i < symbolArray.Length; i++)
        {
            if(!string.IsNullOrEmpty(symbolArray[i]))
            {
                symbols.Add(symbolArray[i], true);
            }
        }
        
        // Get existing open window or if none, make a new one:
        SymbolWindow window = (SymbolWindow)GetWindow(typeof(SymbolWindow));
        window.Show();
    }

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        AddSymbolLine(SYMBOLS.DEBUG_MODE, "Debug Mode");

        GUILayout.Space(10);
        if(GUILayout.Button("Save"))
        {
            SaveSymbols();
        }

        EditorGUILayout.EndVertical();
    }

    void AddSymbolLine (string symbol, string label)
    {
        if(!symbols.ContainsKey(symbol))
        {
            symbols.Add(symbol, false);
        }

        EditorGUILayout.BeginHorizontal();
        symbols[symbol] = EditorGUILayout.Toggle(symbols[symbol]);
        EditorGUILayout.LabelField(label);
        EditorGUILayout.EndHorizontal();
    }

    void SaveSymbols ()
    {
        string defines = "";
        foreach(var kvp in symbols)
        {
            if(kvp.Value)
            {
                defines += kvp.Key;
            }
        }

        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, defines);
    }
}