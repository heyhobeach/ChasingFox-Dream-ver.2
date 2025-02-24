using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopNewsImage : MonoBehaviour,IPointerClickHandler
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject popImage;
    private GameObject background;
    void Start()
    {
        popImage = this.transform.GetChild(1).gameObject;
        background = this.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject gobj = eventData.pointerCurrentRaycast.gameObject;

        if (gobj.name == "Image")
        {
  
            popImage.SetActive(true);
            popImage.GetComponent<Image>().sprite = gobj.GetComponent<Image>().sprite;
            background.SetActive(true);
            return;
        }
        else if(gobj.name ==popImage.name||gobj.name==background.name)
        {
            Debug.Log("다른거");
            popImage.SetActive(false);
            background.SetActive(false);
            return;
        }
        //if (gobj.name == "BackImage")
        //{
        //    popImage.SetActive(false);
        //    background.SetActive(false);
        //    return;
        //}
        //if (Input.GetMouseButtonDown(0))
        //{
        //    popImage.SetActive(false);
        //    background.SetActive(false);
        //}


        Debug.Log(gobj.name);
    }
}
