using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[InitializeOnLoad]
public class WrapperManager : EditorWindow
{

    [MenuItem("Window/Wrapper Manager")]
    static void Init()
    {
        WrapperManager window = (WrapperManager)GetWindow(typeof(WrapperManager));
        window.Show();
    }

    public class AudioWrapper
    {
        public enum ContainerType { Random, Sequence, Switch }

        public string name;
        public List<AudioClip> clips;
        public ContainerType container;
        public bool bypassFX;
        public bool bypassListenerFX;
        public bool bypassReverb;
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
        public float minDistance = 1;
        public float maxDistance = 500;

        public AudioWrapper(string name)
        {
            this.name = name;
            clips = new List<AudioClip>();
            container = ContainerType.Random;
            bypassFX = false;
            bypassListenerFX = false;
            bypassReverb = false;
            loop = false;
            priority = 128;
            volume = 1;
            randomVolume = false;
            volumeMin = 0.5f;
            volumeMax = 0.5f;
            pitch = 1;
            randomPitch = false;
            pitchMin = 1;
            pitchMax = 1;
            pan = 0;
            spatialBlend = 0;
            reverbMix = 1;
            dopplerLevel = 1;
            spread = 0;

        }
    }

    static List<AudioWrapper> wrappers;
    Vector2 listScroll;
    Vector2 editorScroll;
    Vector2 containerScroll;
    AudioWrapper currentWrapper;
    bool showSounds = true;
    bool show3D = true;

    //Funtion used to instatiate the newly created audio source, based on the wrapper's variables
    public static void InsantiateSource(AudioSource newSource, AudioWrapper wrapper)
    {
        
        newSource.bypassEffects = wrapper.bypassFX;
        newSource.bypassListenerEffects = wrapper.bypassListenerFX;
        newSource.bypassReverbZones = wrapper.bypassReverb;
        newSource.loop = wrapper.loop;
        newSource.playOnAwake = false;
        newSource.priority = wrapper.priority;
        newSource.volume = wrapper.volume;
        newSource.pitch = wrapper.pitch;
        newSource.panStereo = wrapper.pan;
        newSource.spatialBlend = wrapper.spatialBlend;
        newSource.reverbZoneMix = wrapper.reverbMix;
        newSource.dopplerLevel = wrapper.dopplerLevel;
        newSource.spread = wrapper.spread;

    }

    //Function called to create an audio source that will play a sound on the game object that posted the event
    public static void PlaySound(string target, GameObject gameObject)
    {
        var host = new GameObject(target+" Routine").AddComponent<CoroutineHost>();
        AudioWrapper targetWrapper = wrappers.Find((obj) => obj.name == target);

        if (targetWrapper == null) return;


        GameObject targetObj = new GameObject(targetWrapper.name);
        targetObj.transform.parent = gameObject.transform;

        AudioSource source = targetObj.AddComponent(typeof(AudioSource)) as AudioSource;
        InsantiateSource(source, targetWrapper);




        if (targetWrapper.randomVolume)                                                      //Randomize the volume, if random volume is selected
            source.volume = Random.Range(targetWrapper.volumeMin, targetWrapper.volumeMax);
        if (targetWrapper.randomPitch)                                                       //Randomize the pitch, if random pitch is selected
            source.pitch = Random.Range(targetWrapper.pitchMin, targetWrapper.pitchMax);
        //If the source has only one sound to choose from, play that sound
        source.clip = targetWrapper.clips[0];
        if (targetWrapper.clips.Count == 1)
        {                                      
            source.Play();
        }
        else if (targetWrapper.clips.Count > 1)
        {     
            source.clip = targetWrapper.clips[Random.Range(0, targetWrapper.clips.Count)];
            source.Play();
            /*
            //If the audio source can choose from more that one sound
            if (targetWrapper.containerType == ContainerType.Random)
            {              //Pick one clip to play at random, if the selected container type is random
                
            }
            */
        }
        else
            return;
        if (!source.loop)
            host.Execute(DestroyObject(targetObj, source.clip.length));
         



    }


    void OnInspectorUpdate()
    {
        if(wrappers == null)
        {
            Load();
        }
    }

    public static void Subscribe()
    {
        SAM.playSound += PlaySound;
    }

    public static void Load()
    {
        wrappers = new List<AudioWrapper>();
        StreamReader reader = new StreamReader("/Users/vasilis/Projects/Nightmares/Assets/_Complete-Game/Scripts/Audio/wrappers.txt");
        int wrapperCount = int.Parse(reader.ReadLine());
        int clipCount;
        for (int i = 0; i < wrapperCount; i++)
        {
            AudioWrapper wrapperBuffer = new AudioWrapper("");
            wrapperBuffer.name = reader.ReadLine();
            clipCount = int.Parse(reader.ReadLine());
            for (int j = 0; j < clipCount; j++)
            {
                AudioClip clipBuffer = new AudioClip();
                string temp = reader.ReadLine();
                clipBuffer = Resources.Load(temp, typeof(AudioClip)) as AudioClip;
                wrapperBuffer.clips.Add(clipBuffer);

            }
            wrapperBuffer.bypassFX = bool.Parse(reader.ReadLine());
            wrapperBuffer.bypassListenerFX = bool.Parse(reader.ReadLine());
            wrapperBuffer.bypassReverb = bool.Parse(reader.ReadLine());
            wrapperBuffer.loop = bool.Parse(reader.ReadLine());
            wrapperBuffer.priority = int.Parse(reader.ReadLine());
            wrapperBuffer.volume = float.Parse(reader.ReadLine());
            wrapperBuffer.randomVolume = bool.Parse(reader.ReadLine());
            wrapperBuffer.volumeMin = float.Parse(reader.ReadLine());
            wrapperBuffer.volumeMax = float.Parse(reader.ReadLine());
            wrapperBuffer.pitch = float.Parse(reader.ReadLine());
            wrapperBuffer.randomPitch = bool.Parse(reader.ReadLine());
            wrapperBuffer.pitchMin = float.Parse(reader.ReadLine());
            wrapperBuffer.pitchMax = float.Parse(reader.ReadLine());
            wrapperBuffer.pan = float.Parse(reader.ReadLine());
            wrapperBuffer.spatialBlend = float.Parse(reader.ReadLine());
            wrapperBuffer.reverbMix = float.Parse(reader.ReadLine());
            wrapperBuffer.dopplerLevel = float.Parse(reader.ReadLine());
            wrapperBuffer.spread = int.Parse(reader.ReadLine());
            wrapperBuffer.minDistance = float.Parse(reader.ReadLine());
            wrapperBuffer.maxDistance = float.Parse(reader.ReadLine());

            wrappers.Add(wrapperBuffer);
        }
        reader.Close();
    }

    void Save()
    {
        StreamWriter writer = new StreamWriter("/Users/vasilis/Projects/Nightmares/Assets/_Complete-Game/Scripts/Audio/wrappers.txt");
        writer.WriteLine(wrappers.Count);
        foreach(AudioWrapper wrapper in wrappers)
        {
            writer.WriteLine(wrapper.name);
            writer.WriteLine(wrapper.clips.Count);
            foreach(AudioClip clip in wrapper.clips)
            {
                string path = AssetDatabase.GetAssetPath(clip);
                path = path.Replace("Assets/Resources/", "");
                path = path.Replace(".wav", "");
                writer.WriteLine(path);
            }
            writer.WriteLine(wrapper.bypassFX);
            writer.WriteLine(wrapper.bypassListenerFX);
            writer.WriteLine(wrapper.bypassReverb);
            writer.WriteLine(wrapper.loop);
            writer.WriteLine(wrapper.priority);
            writer.WriteLine(wrapper.volume);
            writer.WriteLine(wrapper.randomVolume);
            writer.WriteLine(wrapper.volumeMin);
            writer.WriteLine(wrapper.volumeMax);
            writer.WriteLine(wrapper.pitch);
            writer.WriteLine(wrapper.randomPitch);
            writer.WriteLine(wrapper.pitchMin);
            writer.WriteLine(wrapper.pitchMax);
            writer.WriteLine(wrapper.pan);
            writer.WriteLine(wrapper.spatialBlend);
            writer.WriteLine(wrapper.reverbMix);
            writer.WriteLine(wrapper.dopplerLevel);
            writer.WriteLine(wrapper.spread);
            writer.WriteLine(wrapper.minDistance);
            writer.WriteLine(wrapper.maxDistance);
        }
        writer.Close();
    }

    void ShowList()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(position.width / 4), GUILayout.Height(position.height));
        listScroll = GUILayout.BeginScrollView(listScroll, GUILayout.Width(position.width / 4), GUILayout.Height(position.height));
        GUILayout.Label("Audio Wrappers", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        //When pressed, calls the save function to store current wrapper settings
        if (GUILayout.Button("Load", GUILayout.Width(50), GUILayout.Height(20)))
        {
            Load();
        }
        //Button that when pressed, creates a new audio wrapper and opens up the editor for that wrapper
        if (GUILayout.Button("+ Add", GUILayout.Width(50), GUILayout.Height(20)))
        {
            AudioWrapper newWrapper = new AudioWrapper("Wrapper" + wrappers.Count);
            wrappers.Add(newWrapper);
            currentWrapper = newWrapper;
        }
        //When pressed, calls the save function to store current wrapper settings
        if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(20)))
        {
            Save();
        }
        GUILayout.EndHorizontal();


        if (wrappers == null) return;
        //List the events in the window
        foreach (AudioWrapper wrapper in wrappers)
        {
            //Wrapper interface. When the wrapper is pressed, the editor is opened with that wrapper's parameters
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button(wrapper.name, "box"))
            {
                currentWrapper = wrapper;
            }
            // X deletes the wrapper from the list
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (wrapper == currentWrapper) currentWrapper = null;
                wrappers.Remove(wrapper);
                break;
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void ShowEditor()
    {
        if (currentWrapper == null) return;

        GUILayout.BeginVertical("box", GUILayout.Width(position.width * 3/8), GUILayout.Height(position.height));
        editorScroll = GUILayout.BeginScrollView(editorScroll, GUILayout.Width(position.width * 3 / 8), GUILayout.Height(position.height));
        if (Event.current.Equals(Event.KeyboardEvent("return")))
        {
            GUI.FocusControl(null);
            Repaint();
        }

        GUILayout.Label("Wrapper Editor", EditorStyles.boldLabel);

        //Field to edit the name of the wrapper
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        currentWrapper.name = GUILayout.TextField(currentWrapper.name);
        GUILayout.EndHorizontal();

        //Horizontal Line
        EditorGUILayout.Space();

        //Field to edit the audio clips
        showSounds = EditorGUILayout.Foldout(showSounds, "Audio Clips");
        if(showSounds)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add", GUILayout.Width(50), GUILayout.Height(20)))
            {
                AudioClip newClip = new AudioClip();
                currentWrapper.clips.Add(newClip);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            for (int i = 0; i < currentWrapper.clips.Count; i++)
            {
                GUILayout.BeginHorizontal();
                currentWrapper.clips[i] = (AudioClip)EditorGUILayout.ObjectField(currentWrapper.clips[i], typeof(AudioClip), true);
                if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    currentWrapper.clips.Remove(currentWrapper.clips[i]);
                    break;
                }
                GUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space();

        if(currentWrapper.clips.Count > 1)
        {
            currentWrapper.container = (AudioWrapper.ContainerType)EditorGUILayout.EnumPopup("Container Type", currentWrapper.container);

        }

        EditorGUILayout.Space();

        //Field to toggle bypass effects
        currentWrapper.bypassFX = EditorGUILayout.Toggle("Bypass Effects", currentWrapper.bypassFX);

        //Field to toggle bypass listener effects
        currentWrapper.bypassListenerFX = EditorGUILayout.Toggle("Bypass Listener Effects", currentWrapper.bypassListenerFX);

        //Field to toggle bypass reverb zones
        currentWrapper.bypassReverb = EditorGUILayout.Toggle("Bypass Reverb Zones", currentWrapper.bypassReverb);

        //Field to toggle loop
        currentWrapper.loop = EditorGUILayout.Toggle("Loop", currentWrapper.loop);

        EditorGUILayout.Space();

        //Field to edit the priority
        currentWrapper.priority = EditorGUILayout.IntSlider("Priority", currentWrapper.priority, 0, 256);

        EditorGUILayout.Space();

        //Field to edit the volume
        currentWrapper.volume = EditorGUILayout.Slider("Volume", currentWrapper.volume, 0, 1);

        EditorGUILayout.Space();

        //Enable or not random volume
        if (currentWrapper.randomVolume = EditorGUILayout.Toggle("Randomize Volume", currentWrapper.randomVolume))
        {
            currentWrapper.volumeMin = EditorGUILayout.Slider("Min Volume", currentWrapper.volumeMin, 0, 1);
            currentWrapper.volumeMax = EditorGUILayout.Slider("Max Volume", currentWrapper.volumeMax, 0, 1);
        }

        EditorGUILayout.Space();

        //Field to edit the pitch
        currentWrapper.pitch = EditorGUILayout.Slider("Pitch", currentWrapper.pitch, -3, 3);

        EditorGUILayout.Space();

        //Enable or not random pitch
        if (currentWrapper.randomPitch = EditorGUILayout.Toggle("Randomize Pitch", currentWrapper.randomPitch))
        {
            currentWrapper.pitchMin =  EditorGUILayout.Slider("Min Pitch", currentWrapper.pitchMin, -3, 3);
            currentWrapper.pitchMax = EditorGUILayout.Slider("Max Pitch", currentWrapper.pitchMax, -3, 3);
        }

        EditorGUILayout.Space();

        //Field to edit the stereo pan
        currentWrapper.pan = EditorGUILayout.Slider("Stereo Pan", currentWrapper.pan, 0, 1);

        EditorGUILayout.Space();

        //Field to edit the spatial blend
        currentWrapper.spatialBlend = EditorGUILayout.Slider("Spatial Blend", currentWrapper.spatialBlend, 0, 1);

        EditorGUILayout.Space();

        //Field to edit the reverb mix
        currentWrapper.reverbMix = EditorGUILayout.Slider("Reverb Zone Mix", currentWrapper.reverbMix, 0, 1.1f);

        EditorGUILayout.Space();

        //Foldout containing the 3D Sound settings
        show3D = EditorGUILayout.Foldout(show3D, "3D Sound Settings");
        if(show3D)
        {
            //Field to edit the reverb mix
            currentWrapper.dopplerLevel = EditorGUILayout.Slider("Doppler Level", currentWrapper.dopplerLevel, 0, 5);

            //Field to edit the spread
            currentWrapper.spread = EditorGUILayout.IntSlider("Spread", currentWrapper.spread, 0, 360);

            //Field to edit the min distance from the listener that the sound can be heard from
            currentWrapper.minDistance = EditorGUILayout.FloatField("Min Distance", currentWrapper.minDistance);

            //Field to edit the max distance from the listener that the sound can be heard from
            currentWrapper.maxDistance = EditorGUILayout.FloatField("Max Distance", currentWrapper.maxDistance);
        }


        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void ShowContainer()
    {
        if ((currentWrapper == null) || (currentWrapper.clips.Count <= 1)) return;

        GUILayout.BeginVertical("box", GUILayout.Width(position.width * 3/8), GUILayout.Height(position.height));
        containerScroll = GUILayout.BeginScrollView(containerScroll, GUILayout.Width(position.width * 3/8), GUILayout.Height(position.height));

        GUILayout.Label(currentWrapper.container + " Container", EditorStyles.boldLabel);


        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        ShowList();
        ShowEditor();
        ShowContainer();
        GUILayout.EndHorizontal();


    }

    void Update()
    {
        Repaint();
    }

    static IEnumerator DestroyObject(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(obj);
    }

}
