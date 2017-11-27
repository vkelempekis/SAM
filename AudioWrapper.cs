using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioWrapper : MonoBehaviour {

    public List<string> groups;                                       //List of groups the source belongs to. To receive broadcasts from the events
	public List<AudioClip> sounds;                                    //List of audio clips that can be played from the source
	AudioSource source;                     					      //The source that will play the sound(s)
	public bool randomVolume;
	public float volumeMin;
	public float volumeMax;
	public bool randomPitch;      // Must normalize random volume and pitch and have the initial value of the source unchanged
	public float pitchMin;
	public float pitchMax;



	public enum ContainerType {Random, Sequencer, Switch} 			   //Enumeration for the type of container when there are multiple audio clips
	public ContainerType containerType;

	void Start() {
		source = GetComponent<AudioSource> ();
        SAM.playSound += PlaySound;
	}

    private void OnDestroy()
    {
        SAM.playSound -= PlaySound;
    }


    public void PlaySound (string targetGroup) {                                              //Function called to play the sound held by the audio source according to the type of container
        if (this.groups.Contains(targetGroup))
        {
            if (randomVolume)                                                      //Randomize the volume, if random volume is selected
                source.volume = Random.Range(volumeMin, source.volume);
            if (randomPitch)                                                       //Randomize the pitch, if random pitch is selected
                source.pitch = Random.Range(pitchMin, pitchMax);
            if (sounds.Count == 1)
            {                                      //If the source has only one sound to choose from, play that sound
                source.clip = sounds[0];
                source.Play();
            }
            else if (sounds.Count > 1)
            {                                  //If the audio source can choose from more that one sound
                if (containerType == ContainerType.Random)
                {              //Pick one clip to play at random, if the selected container type is random
                    source.clip = sounds[Random.Range(0, sounds.Count)];
                    source.Play();
                }
            }
            else
                return;
        }

	}
	

	void Update () {
		
	}
}
