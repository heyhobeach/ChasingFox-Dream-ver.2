using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DoorOpen : MonoBehaviour, IBaseController
{
    public Action onDown { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public Action onUp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [SerializeField] private Transform leftPos;
    [SerializeField] private Transform rightPos;
    [SerializeField] private Collider2D[] cols;

    private PlayableDirector timeline;
    private TrackAsset leftBehaviour;
    private TrackAsset rightBehaviour;

    private bool opened;

    private GameObject target;
    private PlayerUnit player;
    private EnemyController enemy;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
        timeline.timeUpdateMode = DirectorUpdateMode.Manual;
        var timelineAsset = timeline.playableAsset as TimelineAsset;
        leftBehaviour = timelineAsset.GetOutputTrack(1);
        rightBehaviour = timelineAsset.GetOutputTrack(2);

        timeline.stopped += (x) => {
            if(target.CompareTag("Player")) ((IBaseController)this).RemoveController();
            else enemy.isStop = false;
            timeline.SetGenericBinding(leftBehaviour, null);
            timeline.SetGenericBinding(rightBehaviour, null);
            foreach(var col in cols) col.enabled = false;
        };
    }

    private void FixedUpdate()
    {
        if(timeline.state == PlayState.Playing) 
        {        
            if(target.CompareTag("Player")) 
            {
                player.SetHorizontalForce(0);
                player.SetHorizontalVelocity(0);
            }
            timeline.time += Time.fixedDeltaTime;
            timeline.Evaluate();
            if(timeline.time >= timeline.duration) timeline.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(opened || !(collider.CompareTag("Player") || collider.CompareTag("Enemy"))) return;
        if(collider.GetComponent<UnitBase>().UnitState != UnitState.Default) return;
        target = collider.gameObject;
        if(target.CompareTag("Player")) 
        {
            ((IBaseController)this).AddController();
            player = target.GetComponent<PlayerUnit>();
            player.SetHorizontalForce(0);
            player.SetHorizontalVelocity(0);
        }
        else
        {
            enemy = target.GetComponent<EnemyController>();
            enemy.isStop = true;
        }

        switch(Mathf.Sign((collider.transform.position-transform.position).x))
        {
            case -1:
                rightPos.position += Vector3.right;
                timeline.SetGenericBinding(leftBehaviour, collider.GetComponent<UnitBase>().rg.transform);
            break;
            case 1:
                leftPos.position += Vector3.left;
                timeline.SetGenericBinding(rightBehaviour, collider.GetComponent<UnitBase>().rg.transform);
            break;
        }

        timeline.Play();
        opened = true;
    }

    public void Controller()
    {
        if(Input.GetButtonDown("Cancel")) GameManager.Instance.Pause();
    }
}
