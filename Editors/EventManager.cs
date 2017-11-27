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


    public static AudioEvent CurrentEvent { get; set; }    //The event being edited

    public Vector2 scrollPosition;


    //The list that holds all the audio events
    public static List<AudioEvent> events = new List<AudioEvent>();

    //Doesn't work for some reason
    //public string path = "/Users/vasilis/Projects/Nightmares/Assets/_Complete-Game/Scripts/Audio/events.txt";



    [MenuItem("Window/Event Manager")]
	public static void Init () 
    {
        GetWindow(typeof(EventManager));
        GetWindow(typeof(EventEditor));
	}

    // Load the events from file. This is called by a button in the event manager and whenever the game starts IF the scene contains a SAM component
    public static void Load()
    {
        events.Clear();
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
                groupBuffer.type = (EventManager.Target.EventType)System.Enum.Parse(typeof(Target.EventType), reader.ReadLine());
                groupBuffer.delayTime = float.Parse(reader.ReadLine());
                groupBuffer.probability = float.Parse(reader.ReadLine());
                groupBuffer.fadeTime = float.Parse(reader.ReadLine());
                eventBuffer.targets.Add(groupBuffer);
                    
            }
            events.Add(eventBuffer);
        }
        reader.Close();
    }




    //Save the current list of events to file
    private void Save()
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

    private void OnGUI()
    {
        
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height));
        GUILayout.Label("Events", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        //When pressed, calls the load function
        if (GUILayout.Button("Load", GUILayout.Width(50), GUILayout.Height(20)))
        {
            Load();
        }
        //Button that when pressed, creates a new event and opens up the editor for that event
        if(GUILayout.Button("+ Add", GUILayout.Width(50),GUILayout.Height(20)))
        {
            AudioEvent newEvent = new AudioEvent("Event" + events.Count);
            events.Add(newEvent);
            CurrentEvent = newEvent;
            EventEditor.ShowWindow();
        }
        //When pressed, calls the save function to store current event settings
        if (GUILayout.Button("Save", GUILayout.Width(50), GUILayout.Height(20)))
        {
            Save();
        }
        GUILayout.EndHorizontal();
        //List the events in the window
        foreach (AudioEvent audioEvent in events)
        {
            
            //Event interface. When the event is pressed, the editor is opened with that events parameters
            GUILayout.BeginHorizontal("box");
            if(GUILayout.Button(audioEvent.name, "box"))
            {
                CurrentEvent = audioEvent;
                EventEditor.ShowWindow();
            }
            // X deletes the event from the list
            if(GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)))
            {
                events.Remove(audioEvent);
                CurrentEvent = null;
                EventEditor.ShowWindow();
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndScrollView();
    }


    private void Update()
    {
        Repaint();
    }
}

