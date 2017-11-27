using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AudioWrapper))]
public class AudioWrapperEditor : Editor {
	
	public override void OnInspectorGUI () {
		serializedObject.Update ();

        //Serialized property setup
        SerializedProperty groupsProp = serializedObject.FindProperty("groups");
		SerializedProperty soundsProp = serializedObject.FindProperty ("sounds");
		SerializedProperty randomVolProp = serializedObject.FindProperty ("randomVolume");
		SerializedProperty volMinProp = serializedObject.FindProperty ("volumeMin");
		SerializedProperty volMaxProp = serializedObject.FindProperty ("volumeMax");
		SerializedProperty randomPitchProp = serializedObject.FindProperty ("randomPitch");
		SerializedProperty pitchMinProp = serializedObject.FindProperty ("pitchMin");
		SerializedProperty pitchMaxProp = serializedObject.FindProperty ("pitchMax");
		SerializedProperty containerProp = serializedObject.FindProperty ("containerType");

        // Custom GUI controls
        EditorGUILayout.PropertyField(groupsProp, new GUIContent("Groups"), true);
		EditorGUILayout.PropertyField (soundsProp, new GUIContent ("Audio Clips"), true);
		if (soundsProp.arraySize > 1) {                           											//Show the container field if there is more than one clip
			EditorGUILayout.PropertyField (containerProp, new GUIContent ("Container Type"), true);
		}
		//EditorGUILayout.PropertyField (sourceProp, new GUIContent ("Audio Source"), true);
		EditorGUILayout.PropertyField (randomVolProp, new GUIContent ("Randomize Volume"),true);
		if (randomVolProp.boolValue) {																		// Show min, max values for the randomization if it is selected
			EditorGUILayout.Slider( volMinProp, 0f, 1.0f, new GUIContent ("Min Volume"));
			EditorGUILayout.Slider( volMaxProp, 0f, 1.0f, new GUIContent ("Max Volume"));
		}
		EditorGUILayout.PropertyField (randomPitchProp, new GUIContent ("Randomize Pitch"),true);
		if (randomPitchProp.boolValue) {
			EditorGUILayout.Slider( pitchMinProp, -3.0f, 1.0f, new GUIContent ("Min Pitch"));
			EditorGUILayout.Slider( pitchMaxProp, 1f, 3.0f, new GUIContent ("Max Pitch"));
		}

		serializedObject.ApplyModifiedProperties ();
	}
}
