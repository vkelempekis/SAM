using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (AudioWrapper))]
public class AudioWrapperEditor : Editor {

    //public AnimationCurve curve;
	
	public override void OnInspectorGUI () {
		serializedObject.Update ();

        //Serialized property setup
        SerializedProperty groupsProp = serializedObject.FindProperty("groups");
		SerializedProperty soundsProp = serializedObject.FindProperty ("sounds");
        SerializedProperty muteProp = serializedObject.FindProperty("mute");
        SerializedProperty bypassFXProp = serializedObject.FindProperty("bypassFX");
        SerializedProperty bypassListenerProp = serializedObject.FindProperty("bypassListenerFX");
        SerializedProperty bypassReverbProp = serializedObject.FindProperty("bypassReverb");
        SerializedProperty playOnAwakeProp = serializedObject.FindProperty("playOnAwake");
        SerializedProperty loopProp = serializedObject.FindProperty("loop");
        SerializedProperty priorityProp = serializedObject.FindProperty("priority");
        SerializedProperty volumeProp = serializedObject.FindProperty("volume");
		SerializedProperty randomVolProp = serializedObject.FindProperty ("randomVolume");
		SerializedProperty volMinProp = serializedObject.FindProperty ("volumeMin");
		SerializedProperty volMaxProp = serializedObject.FindProperty ("volumeMax");
        SerializedProperty pitchProp = serializedObject.FindProperty("pitch");
		SerializedProperty randomPitchProp = serializedObject.FindProperty ("randomPitch");
		SerializedProperty pitchMinProp = serializedObject.FindProperty ("pitchMin");
		SerializedProperty pitchMaxProp = serializedObject.FindProperty ("pitchMax");
        SerializedProperty panProp = serializedObject.FindProperty("pan");
        SerializedProperty blendProp = serializedObject.FindProperty("spatialBlend");
        SerializedProperty reverbMixProp = serializedObject.FindProperty("reverbMix");
        SerializedProperty dopplerProp = serializedObject.FindProperty("dopplerLevel");
        SerializedProperty spreadProp = serializedObject.FindProperty("spread");
        SerializedProperty minDistProp = serializedObject.FindProperty("minDistance");
        SerializedProperty maxDistProp = serializedObject.FindProperty("maxDistance");
		SerializedProperty containerProp = serializedObject.FindProperty ("containerType");



        // Custom GUI controls
        EditorGUILayout.PropertyField(groupsProp, new GUIContent("Groups"), true);
		EditorGUILayout.PropertyField (soundsProp, new GUIContent ("Audio Clips"), true);
		if (soundsProp.arraySize > 1) {                           											//Show the container field if there is more than one clip
			EditorGUILayout.PropertyField (containerProp, new GUIContent ("Container Type"), true);
		}
        EditorGUILayout.PropertyField(muteProp, new GUIContent("Mute"));
        EditorGUILayout.PropertyField(bypassFXProp, new GUIContent("Bypass Effects"));
        EditorGUILayout.PropertyField(bypassListenerProp, new GUIContent("Bypass Listener Effects"));
        EditorGUILayout.PropertyField(bypassReverbProp, new GUIContent("Bypass Reverb Zones"));
        EditorGUILayout.PropertyField(playOnAwakeProp, new GUIContent("Play On Awake"));
        EditorGUILayout.PropertyField(loopProp, new GUIContent("Loop"));
        EditorGUILayout.IntSlider(priorityProp,0, 256, new GUIContent("Priority"));

        EditorGUILayout.Slider(volumeProp, 0, 1, new GUIContent("Volume"));
		EditorGUILayout.PropertyField (randomVolProp, new GUIContent ("Randomize Volume"),true);
		if (randomVolProp.boolValue) {																		// Show min, max values for the randomization if it is selected
			EditorGUILayout.Slider( volMinProp, 0f, 1.0f, new GUIContent ("Min Volume"));
			EditorGUILayout.Slider( volMaxProp, 0f, 1.0f, new GUIContent ("Max Volume"));
		}

        EditorGUILayout.Slider(pitchProp,-3, 3, new GUIContent("Pitch"));
		EditorGUILayout.PropertyField (randomPitchProp, new GUIContent ("Randomize Pitch"),true);
		if (randomPitchProp.boolValue) {
			EditorGUILayout.Slider( pitchMinProp, -3.0f, 1.0f, new GUIContent ("Min Pitch"));
			EditorGUILayout.Slider( pitchMaxProp, 1f, 3.0f, new GUIContent ("Max Pitch"));
		}

        EditorGUILayout.Slider(panProp, -1f, 1f, new GUIContent("Stero Pan"));
        EditorGUILayout.Slider(blendProp, 0f, 1f, new GUIContent("Spatial Blend"));
        EditorGUILayout.Slider(reverbMixProp, 0f, 1.1f, new GUIContent("Reverb Zone Mix"));

        //3D sound settings
        EditorGUILayout.Slider(dopplerProp, 0f, 5f, new GUIContent("Doppler Level"));
        EditorGUILayout.IntSlider(spreadProp, 0, 360, new GUIContent("Spread"));
        EditorGUILayout.PropertyField(minDistProp, new GUIContent("Min Distance"));
        EditorGUILayout.PropertyField(maxDistProp, new GUIContent("Max Distance"));


        //Attenuation Curve
        //EditorGUILayout.CurveField(curve);
		serializedObject.ApplyModifiedProperties ();
	}
}
