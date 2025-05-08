using System;
using System.Collections;
using System.Collections.Generic;
using MyUtiles;
using Unity.VisualScripting;
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
    private List<UnitBase> waitingUnits = new List<UnitBase>();
    private PlayerUnit player;
    private EnemyController enemy;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
        var timelineAsset = timeline.playableAsset as TimelineAsset;
        leftBehaviour = timelineAsset.GetOutputTrack(1);
        rightBehaviour = timelineAsset.GetOutputTrack(2);
        animationTrack = timelineAsset.GetOutputTrack(3) as AnimationTrack;

        timeline.stopped += (x) => {
            if(target.CompareTag("Player")) ((IBaseController)this).RemoveController();
            else target.UnitState = UnitState.Default;
            foreach(var unit in waitingUnits) 
            {
                if(target.CompareTag("Player")) ((IBaseController)this).RemoveController();
                else target.UnitState = UnitState.Default;
            }
            waitingUnits.Clear();
            timeline.SetGenericBinding(leftBehaviour, null);
            timeline.SetGenericBinding(rightBehaviour, null);
            timeline.SetGenericBinding(animationTrack, null);
            cols[0].enabled = false;
        };
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(opened || !(collider.CompareTag("Player") || collider.CompareTag("Enemy"))) return;
        target = collider.GetComponent<UnitBase>();
        if(target.UnitState != UnitState.Default) return;
        var interactable = collider.gameObject.GetInterface<IDoorInteractable>();

        target.SetAni(false);
        if (interactable == null || !interactable.canInteract)
        {
            if(target.CompareTag("Enemy")) 
            {
                target.UnitState = UnitState.Pause;
                waitingUnits.Add(target);
            }
            return;
        }
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
            target.UnitState = UnitState.Pause;
        }
        timeline.SetGenericBinding(animationTrack, target.transform.GetComponent<Animator>());
        
        switch(Mathf.Sign((collider.transform.position-transform.position).x))
        {
            case -1:
                cols[0].offset += Vector2.right*1.5f;
                timeline.SetGenericBinding(leftBehaviour, target.rg.transform);
            break;
            case 1:
                cols[0].offset += Vector2.left*1.5f;
                GetComponent<SpriteRenderer>().flipX = true;
                timeline.SetGenericBinding(rightBehaviour, target.rg.transform);
            break;
        }

        cols[1].enabled = false;

        timeline.Play();
        opened = true;
    }

    public void Controller()
    {
        if(Input.GetButtonDown("Cancel")) ServiceLocator.Get<GameManager>().Pause();
    }
}
