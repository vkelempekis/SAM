using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAM
{
    public delegate void PlaySound(string targetGroup, GameObject gameObjects);  //Can I make this accept different kinds of variables? So to have delegate BroadcastedEvent
    public static event PlaySound playSound;

    public static SAM Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SAM();
            }
            return instance;
        }
    }
    static SAM instance;

    SAM()
    {
        WrapperManager.Load();
        WrapperManager.Subscribe();
        EventManager.Load();
        EventManager.OptionsUpdate();
    }


    public void PostEvent(string eventName, GameObject gameObject)
    {
        var host = new GameObject().AddComponent<CoroutineHost>();
        host.Execute(PostEventRoutine(eventName, gameObject));

    }

    // Post event with given name
    IEnumerator PostEventRoutine(string eventName, GameObject gameObject)
    {
        for (int i = 0; i < EventManager.events.Count; i++)
        {
            if (EventManager.events[i].name == eventName)
            {
                foreach (EventManager.Target target in EventManager.events[i].targets)
                {
                    yield return new WaitForSeconds(target.delayTime);     // The delay before the event is posted
                    if (target.type == EventManager.Target.EventType.PostEvent)
                    {
                        PostEvent(target.targetName, gameObject);
                    }
                    else
                    {
                        if (Random.value <= target.probability) // Post the event according to the probability
                        {
                            switch (target.type)
                            {
                                case EventManager.Target.EventType.Play:
                                    playSound(target.targetName, gameObject);
                                    break;
                            }

                        }
                    }


                }
                break;
            }
        }
    }

}
