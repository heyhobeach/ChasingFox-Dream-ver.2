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
    [SerializeField] private Transform doorPos;
    [SerializeField] private Collider2D[] cols;

    private PlayableDirector timeline;
    private TrackAsset leftBehaviour;
    private TrackAsset rightBehaviour;
    private AnimationTrack animationTrack;

    private bool opened;

    private UnitBase target;
    private PlayerUnit player;
    private EnemyController enemy;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
        // timeline.timeUpdateMode = DirectorUpdateMode.Manual;
        var timelineAsset = timeline.playableAsset as TimelineAsset;
        leftBehaviour = timelineAsset.GetOutputTrack(1);
        rightBehaviour = timelineAsset.GetOutputTrack(2);
        animationTrack = timelineAsset.GetOutputTrack(3) as AnimationTrack;

        timeline.stopped += (x) => {
            if(target.CompareTag("Player")) ((IBaseController)this).RemoveController();
            else enemy.isStop = false;
            timeline.SetGenericBinding(leftBehaviour, null);
            timeline.SetGenericBinding(rightBehaviour, null);
            timeline.SetGenericBinding(animationTrack, null);
            cols[1].enabled = false;
        };
    }

    // private void FixedUpdate()
    // {
    //     if(timeline.state == PlayState.Playing) 
    //     {        
    //         if(target.CompareTag("Player")) 
    //         {
    //             player.SetHorizontalForce(0);
    //             player.SetHorizontalVelocity(0);
    //         }
    //         timeline.time += Time.fixedDeltaTime;
    //         timeline.Evaluate();
    //         if(timeline.time >= timeline.duration) timeline.Stop();
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(opened || !(collider.CompareTag("Player") || collider.CompareTag("Enemy"))) return;
        if(collider.GetComponent<UnitBase>().UnitState != UnitState.Default) return;
        target = collider.GetComponent<UnitBase>();
        target.SetAni(false);
        timeline.SetGenericBinding(animationTrack, target.transform.GetComponent<Animator>());
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
            enemy.blackboard.nodeIdx += 2;
            enemy.isStop = true;
        }
        
        switch(Mathf.Sign((collider.transform.position-transform.position).x))
        {
            case -1:
                rightPos.position += Vector3.right;
                doorPos.position += Vector3.left*target.BoxSizeX;
                cols[1].offset += Vector2.right*1.5f;
                timeline.SetGenericBinding(leftBehaviour, target.rg.transform);
            break;
            case 1:
                leftPos.position += Vector3.left;
                doorPos.position += Vector3.right*target.BoxSizeX;
                cols[1].offset += Vector2.left*1.5f;
                GetComponent<SpriteRenderer>().flipX = true;
                timeline.SetGenericBinding(rightBehaviour, target.rg.transform);
            break;
        }

        cols[0].enabled = false;

        timeline.Play();
        opened = true;
    }

    public void Controller()
    {
        if(Input.GetButtonDown("Cancel")) GameManager.Instance.Pause();
    }
}
