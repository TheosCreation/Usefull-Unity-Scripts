using System.Collections;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public void SetTimer(float delay, System.Action callback)
    {
        StartCoroutine(TimerCoroutine(delay, callback));
    }

    private IEnumerator TimerCoroutine(float delay, System.Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }
}
