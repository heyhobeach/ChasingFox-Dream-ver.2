using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionCanvasController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    static private CollectionCanvasController instance;
    static public CollectionCanvasController Instance { get => instance; }

    public GameObject panel;
    public TMP_Text contentText;

    private void Awake()
    {
        if(Instance == null)
        {
            instance = this;
        }
    }

    public void Popup()
    {
        panel.SetActive(true);
    }
    public void SetPosition(Vector2 vec)

    {
        //Vector2 _vec = new Vector2(vec.x,vec.y+ypos);
        this.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, this.transform.localScale.z);
        this.gameObject.transform.position = vec;
        //panel.gameObject.transform.position = vec;
        //panel.gameObject.transform.localPosition = vec;
    }

     public void SetContentText(string text)
    {
        contentText.text = text;
    }

     public void PopupEnd()
    {
        panel.SetActive(false);
    }
}
