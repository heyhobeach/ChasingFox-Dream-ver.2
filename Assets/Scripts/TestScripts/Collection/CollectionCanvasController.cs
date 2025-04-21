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

    public GameObject _image;

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
        this.gameObject.transform.localScale = new Vector3(0.01f, 0.01f, this.transform.localScale.z);
        this.gameObject.transform.position = vec;
    }

     public void SetContentText(string text)
    {
        contentText.text = text;
    }
     public void PopupEnd()
    {
        panel.SetActive(false);
        _image.SetActive(false);
    }
    public void ImagePopup()
    {
        _image.SetActive(true);
    }
    public void ImageSetPosition(Vector2 vec)
    {
        _image.transform.localScale = new Vector3(0.3f, 0.3f, this.transform.localScale.z);
        _image.transform.position = vec;
    }
}
