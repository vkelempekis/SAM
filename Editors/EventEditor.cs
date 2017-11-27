using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EventEditor : EditorWindow {
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(EventEditor));
    }

    public Vector2 scrollPosition;

    public void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
        if(Event.current.Equals(Event.KeyboardEvent("return"))){
            GUI.FocusControl(null);
            Repaint();
        }

        //Field to edit the name of the event
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        EventManager.CurrentEvent.name = GUILayout.TextField(EventManager.CurrentEvent.name);
        GUILayout.EndHorizontal();

        //Horizontal line
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Parameters for each target group
        for (int i = 0; i < EventManager.CurrentEvent.targets.Count; i++){
            // Button to remove the target group from the target group list
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                EventManager.CurrentEvent.targets.Remove(EventManager.CurrentEvent.targets[i]);
            }

            //Field to edit the type of the event for this target group
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type");
            EventManager.CurrentEvent.targets[i].type = (EventManager.Target.EventType)EditorGUILayout.EnumPopup(EventManager.CurrentEvent.targets[i].type);
            GUILayout.EndHorizontal();

            //Edit the target groups name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target");
            EventManager.CurrentEvent.targets[i].targetName = GUILayout.TextField(EventManager.CurrentEvent.targets[i].targetName, GUILayout.Width(200), GUILayout.MaxHeight(20));
            GUILayout.EndHorizontal();


            //Field to edit the delay time before the event gets posted for this group
            GUILayout.BeginHorizontal();
            GUILayout.Label("Delay");
            EventManager.CurrentEvent.targets[i].delayTime = EditorGUILayout.Slider(EventManager.CurrentEvent.targets[i].delayTime, 0, 10);
            GUILayout.EndHorizontal();

            if (EventManager.CurrentEvent.targets[i].type != EventManager.Target.EventType.PostEvent){
                //Field to edit the probability of the event getting posted for this group
                GUILayout.BeginHorizontal();
                GUILayout.Label("Probability");
                EventManager.CurrentEvent.targets[i].probability = EditorGUILayout.Slider(EventManager.CurrentEvent.targets[i].probability, 0, 1);
                GUILayout.EndHorizontal();


                //Field to edit the fade time of the event getting posted
                GUILayout.BeginHorizontal();
                GUILayout.Label("Fade Time");
                EventManager.CurrentEvent.targets[i].fadeTime = EditorGUILayout.Slider(EventManager.CurrentEvent.targets[i].fadeTime, 0, 10);
                GUILayout.EndHorizontal();
            }



            //Horizontal Line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        }

        if (GUILayout.Button("+ Add", GUILayout.Width(50), GUILayout.Height(20)))
        {
            EventManager.Target newGroup = new EventManager.Target();
            EventManager.CurrentEvent.targets.Add(newGroup);
        }



        GUILayout.EndScrollView();
    }
}
