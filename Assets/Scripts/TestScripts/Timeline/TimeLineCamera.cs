using UnityEngine;
using UnityEngine.Playables;

public class TimeLineCamera : MonoBehaviour
{
    void Start()
    {
        var triggers = GameManager.Instance.eventTriggers;
        foreach(var trigg in triggers) 
        {
            PlayableDirector temp = null;
            trigg.TryGetComponent(out temp);
            if(temp) 
            {
                temp.played += (x) => gameObject.SetActive(true);
                temp.stopped += (x) => gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }
}
