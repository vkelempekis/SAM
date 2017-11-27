using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SAM : MonoBehaviour {

    public delegate void PlaySound(string targetGroup);  //Can I make this accept different kinds of variables? So to have delegate BroadcastedEvent
    public static event PlaySound playSound;


    private void Start()
    {
        //Load the events from file
        EventManager.Load();
    }



    public void PostEvent(string eventName){
        StartCoroutine(PostEventRoutine(eventName));
    }

    // Post event with given name
    IEnumerator PostEventRoutine(string eventName){
        for (int i = 0; i < EventManager.events.Count; i++)
        {
            if (EventManager.events[i].name == eventName)
            {
                foreach(EventManager.Target target in EventManager.events[i].targets){
                    yield return new WaitForSeconds(target.delayTime);     // The delay before the event is posted
                    if (target.type == EventManager.Target.EventType.PostEvent)
                    {
                        PostEvent(target.targetName);
                    }
                    else
                    {
                        if (Random.value <= target.probability) // Post the event according to the probability
                        {
                            switch (target.type)
                            {
                                case EventManager.Target.EventType.Play:
                                    playSound(target.targetName);
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
