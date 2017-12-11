using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class EventManager : EditorWindow {

    //The event class that holds all event parameters
    public class AudioEvent
    {
        public string name { set; get; }
        public List<Target> targets = new List<Target>();

        public AudioEvent(string name)
        {
            this.name = name;
        }
    }

    public class Target 
    {
        public enum EventType { Play, Stop, StopAll, PostEvent }

        public string targetName { set; get; }
        public EventType type { set; get; }
        public float delayTime { set; get; }
        public float probability { set; get; }
        public float fadeTime { set; get; }

        public Target()
        {
            targetName = "";
            type = EventType.Play;
            delayTime = 0;
            probability = 1;
            fadeTime = 0;
        }
    }


    AudioEvent currentEvent;    //The event being edited

    public static string[] options;

    Vector2 listScroll;
    Vector2 editorScroll;


    //The list that holds all the audio events
    public static List<AudioEvent> events;

    //Doesn't work for some reason
    //public string path = "/Users/vasilis/Projects/Nightmares/Assets/_Complete-Game/Scripts/Audio/events.txt";


    [MenuItem("Window/Event Manager")]
	public static void Init () 
    {
        GetWindow(typeof(EventManager));
	}

    // Load the events from file. This is called by a button in the event manager and whenever the game starts IF the scene contains a SAM component
    public static void Load()
    {
        events = new List<AudioEvent>();
        StreamReader reader = new StreamReader("/Users/vasilis/Projects/Nightmares/Assets/_Complete-Game/Scripts/Audio/events.txt");
        int eventCount = int.Parse(reader.ReadLine());
        int groupCount;
        for (int i = 0; i < eventCount; i++)
        {
            AudioEvent eventBuffer = new AudioEvent("");
            eventBuffer.name = reader.ReadLine();
            groupCount = int.Parse(reader.ReadLine());
            for (int j = 0; j < groupCount; j++)
            {
                Target groupBuffer = new Target();
                groupBuffer.targetName = reader.ReadLine();
                groupBuffer.type = (Target.EventType)System.Enum.Parse(typeof(Target.EventType), reader.ReadLine());
                groupBuffer.delayTime = float.Parse(reader.ReadLine());
                groupBuffer.probability = float.Parse(reader.ReadLine());
                groupBuffer.fadeTime = float.Parse(reader.ReadLine());
                eventBuffer.targets.Add(groupBuffer);
                    
            }
            events.Add(eventBuffer);
        }
        reader.Close();
    }


    /* For a next version
    //Updates the target options based on the names in the list of wrappers
    public static void OptionsUpdate()
    {
        int size = WrapperManager.wrappers.Count + 1;
        options = new string[size];
        options[0] = "None";
        for (int i = 1; i < size;i++)
        {
            options[i] = WrapperManager.wrappers[i - 1].name;
        }

    }
    */

    //Save the current list of events to file
    void Save()
    {
        StreamWriter writer = new StreamWriter("/Users/vasilis/Projects/Nightmares/Assets/_Complete-Game/Scripts/Audio/events.txt");
        writer.WriteLine(events.Count);                      // The number of events
        foreach(AudioEvent audioEvent in events)
        {
            writer.WriteLine(audioEvent.name);
            writer.WriteLine(audioEvent.targets.Count);       //The number of target groups
            foreach(Target targetGroup in audioEvent.targets)
            {
                writer.WriteLine(targetGroup.targetName);    
                writer.WriteLine(targetGroup.type);
                writer.WriteLine(targetGroup.delayTime);
                writer.WriteLine(targetGroup.probability);
                writer.WriteLine(targetGroup.fadeTime);
            }

        }
        writer.Close();
    }

    void OnInspectorUpdate()
    {
        if(events == null)
        {
            Load();
        }
    }

    void ShowList()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(position.width / 4), GUILayout.Height(position.height));
        listScroll = GUILayout.BeginScrollView(listScroll, GUILayout.Width(position.width/4), GUILayout.Height(position.height));
        GUILayout.Label("Events", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        //When pressed, calls the load function to get event settings from file
        if (GUILayout.Button("Load", GUILayout.Width(50), GUILayout.Height(20)))
        {
            Load();
        }
        //Button that when pressed, creates a new event and opens up the editor for that event
        if (GUILayout.Button("+ Add", GUILayout.Width(50), GUILayout.Height(20)))
        {
            AudioEvent newEvent = new AudioEvent("Event" + events.Count);
            events.Add(newEvent);
            currentEvent = newEvent;
        }
        //When pressed, calls the save function to store current event settings
        if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(20)))
        {
            Save();
        }
        GUILayout.EndHorizontal();
        //List the events in the window
        if (events == null) return;
        foreach (AudioEvent audioEvent in events)
        {

            //Event interface. When the event is pressed, the editor is opened with that events parameters
            GUILayout.BeginHorizontal("box");
            if (GUILayout.Button(audioEvent.name, "box"))
            {
                currentEvent = audioEvent;
            }
            // X deletes the event from the list
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                if (audioEvent == currentEvent) currentEvent = null;
                events.Remove(audioEvent);
                break;
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void ShowEditor(){
        if (currentEvent == null) return;

        editorScroll = GUILayout.BeginScrollView(editorScroll, GUILayout.Width(position.width *3/4), GUILayout.Height(position.height));
        if (Event.current.Equals(Event.KeyboardEvent("return")))
        {
            GUI.FocusControl(null);
            Repaint();
        }



        //Field to edit the name of the event
        GUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        currentEvent.name = GUILayout.TextField(currentEvent.name);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("+ Add", GUILayout.Width(50), GUILayout.Height(20)))
        {
            Target newGroup = new Target();
            currentEvent.targets.Add(newGroup);
        }

        //Horizontal line
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        //Parameters for each target group
        for (int i = 0; i < currentEvent.targets.Count; i++)
        {
            // Button to remove the target group from the target group list
            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                currentEvent.targets.Remove(currentEvent.targets[i]);
                break;
            }

            //Field to edit the type of the event for this target group
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type");
            currentEvent.targets[i].type = (Target.EventType)EditorGUILayout.EnumPopup(currentEvent.targets[i].type);
            GUILayout.EndHorizontal();

            //Edit the target groups name
            GUILayout.BeginHorizontal();
            GUILayout.Label("Target");
            currentEvent.targets[i].targetName = GUILayout.TextField(currentEvent.targets[i].targetName, GUILayout.Width(200), GUILayout.MaxHeight(20));
            GUILayout.EndHorizontal();

            //Field to edit the delay time before the event gets posted for this group
            GUILayout.BeginHorizontal();
            GUILayout.Label("Delay");
            currentEvent.targets[i].delayTime = EditorGUILayout.Slider(currentEvent.targets[i].delayTime, 0, 10);
            GUILayout.EndHorizontal();

            if (currentEvent.targets[i].type != Target.EventType.PostEvent)
            {
                //Field to edit the probability of the event getting posted for this group
                GUILayout.BeginHorizontal();
                GUILayout.Label("Probability");
                currentEvent.targets[i].probability = EditorGUILayout.Slider(currentEvent.targets[i].probability, 0, 1);
                GUILayout.EndHorizontal();


                //Field to edit the fade time of the event getting posted
                GUILayout.BeginHorizontal();
                GUILayout.Label("Fade Time");
                currentEvent.targets[i].fadeTime = EditorGUILayout.Slider(currentEvent.targets[i].fadeTime, 0, 10);
                GUILayout.EndHorizontal();
            }



            //Horizontal Line
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        }



        GUILayout.EndScrollView();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        ShowList();
        ShowEditor();
        GUILayout.EndHorizontal();
    }


    void Update()
    {
        Repaint();
    }
}

