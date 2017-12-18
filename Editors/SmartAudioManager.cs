using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class SmartAudioManager : EditorWindow {


    public static void Init()
    {
        SmartAudioManager window = (SmartAudioManager)GetWindow(typeof(SmartAudioManager));
        window.Show();
    }

    EventManager eventManager = new EventManager();

    void OnInspectorUpdate()
    {
        
    }

    void OnGUI()
    {
        
        GUILayout.BeginHorizontal(GUILayout.Width(position.width/4));
        if(GUILayout.Button("Wrappers"))
        {
            
        }
        if (GUILayout.Button("Events"))
        {
            GUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();
        }
        GUILayout.EndHorizontal();
    }

    void Update()
    {
        Repaint();
    }
}
