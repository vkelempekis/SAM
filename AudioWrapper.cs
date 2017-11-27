using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//[RequireComponent(typeof(AudioSource))]
public class AudioWrapper : MonoBehaviour {

    public List<string> groups;                                       //List of groups the source belongs to. To receive broadcasts from the events
	public List<AudioClip> sounds;                                    //List of audio clips that can be played from the source
    List<AudioSource> sources = new List<AudioSource>();              //List that holds all the currently active audio sources 
    public bool mute;
    public bool bypassFX;
    public bool bypassListenerFX;
    public bool bypassReverb;
    public bool playOnAwake;
    public bool loop;
    public int priority;
    public float volume;
    public bool randomVolume;
    public float volumeMin;
    public float volumeMax;
    public float pitch;
	public bool randomPitch;                         // Must normalize random volume and pitch and have the initial value of the source unchanged
	public float pitchMin;
	public float pitchMax;
    public float pan;
    public float spatialBlend;
    public float reverbMix;
    public float dopplerLevel;
    public int spread;
    public int minDistance;
    public int maxDistance;






	public enum ContainerType {Random, Sequencer, Switch} 			   //Enumeration for the type of container when there are multiple audio clips
	public ContainerType containerType;

	void Start() {
		//source = GetComponent<AudioSource> ();
        SAM.playSound += PlaySound;
	}

    private void OnDestroy()
    {
        SAM.playSound -= PlaySound;
    }


    //Funtion used to instatiate the newly created audio source, based on the wrapper's variables
    public void InsantiateSource(AudioSource newSource){
        
        newSource.mute = mute;
        newSource.bypassEffects = bypassFX;
        newSource.bypassListenerEffects = bypassListenerFX;
        newSource.bypassReverbZones = bypassReverb;
        newSource.playOnAwake = playOnAwake;
        newSource.loop = loop;
        newSource.priority = priority;
        newSource.volume = volume;
        newSource.pitch = pitch;
        newSource.panStereo = pan;
        newSource.spatialBlend = spatialBlend;
        newSource.reverbZoneMix = reverbMix;
        newSource.dopplerLevel = dopplerLevel;
        newSource.spread = spread;
        newSource.minDistance = minDistance;
        newSource.maxDistance = maxDistance;

    }


    public void PlaySound (string targetGroup) {                                              //Function called to play the sound held by the audio source according to the type of container
        if (this.groups.Contains(targetGroup))
        {
            AudioSource source = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
            sources.Add(source);
            InsantiateSource(source);
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
        foreach(AudioSource source in sources){
            if(!source.isPlaying){
                sources.Remove(source);
                Destroy(source);

            }
        }
	}
}
