using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MysteryCanvas : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IEndDragHandler,IDragHandler
{

    public GameObject gobj;

    bool wordClicked=false;
    //click과 drag둘다 필요할까? 하다면 클릭으로 해야하지 않는가
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject gobj = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log("클릭" + gobj.name);
        if (gobj.name == "word")
        {
            wordClicked = true;
        }
        else
        {
            wordClicked = false;
        }
    }


    //작동하는동안 움직이도록
    public void OnDrag(PointerEventData eventData)
    {

        if(wordClicked)
        {
            eventData.pointerPressRaycast.gameObject.transform.position=eventData.position;
            Debug.Log(eventData.position);
        }
        Debug.Log("드래그중");
    }


    //놓는 시점에서 위치 확인하고 넣을수 있다면 넣도록
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("드래그 종료");
        wordClicked = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void OnPointerClick(PointerEventData eventData)
    {

    }

}
