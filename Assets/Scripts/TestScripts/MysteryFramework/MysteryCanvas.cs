using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MysteryCanvas : MonoBehaviour,IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject gobj = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log("클릭"+gobj.name);
    }
}
