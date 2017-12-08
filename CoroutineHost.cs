using UnityEngine;
using System.Collections;

public class CoroutineHost : MonoBehaviour
{
	public void Execute(IEnumerator coroutine)
	{
        StartCoroutine(CoroutineHandler(coroutine));
	}

    IEnumerator CoroutineHandler(IEnumerator coroutine){
        yield return coroutine;
        Destroy(gameObject);
    }
}
