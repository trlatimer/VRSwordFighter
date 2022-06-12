using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NetworkManager))]
public class NetworkManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.HelpBox("This creates and starts a new room.", MessageType.Info);

        NetworkManager networkManager = (NetworkManager)target;
        if (GUILayout.Button("Create Room"))
        {
            NetworkManager.instance.CreateRoom("");
            
        }

        if (GUILayout.Button("Start Game"))
        {
            MainMenu menu = GameObject.FindObjectOfType<MainMenu>();
            menu.StartButton_Click();
        }
    }
}
