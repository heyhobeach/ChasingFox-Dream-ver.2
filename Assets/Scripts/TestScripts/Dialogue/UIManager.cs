using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : BaseController
{
    public InteractionEvent interactionEvent;
    //public Text
    public TMP_Text namemesh;
    public TMP_Text content;

    IEnumerator co = null;

    [SerializeField]
    public float typing_speed = 0.05f;
    //public InteractionEvent interactionEvent;

    // Start is called before the first frame update
    void Start()
    {
        co = Typing("");
        //namemesh = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        //namemesh.text = "조현섭";
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        AddController();
        Next();
    }
    public void Disable()
    {
        RemoveController();
        gameObject.SetActive(false);
    }

    public override void Controller()
    {
        if(Input.GetKeyDown(KeyCode.F)) Next();
        if(Input.GetKeyDown(KeyCode.LeftArrow)) ChoiceLeft();
        else if(Input.GetKeyDown(KeyCode.RightArrow)) ChoiceRight();
        if(Input.GetKeyDown(KeyCode.X)) Disable();
    }

    private void Next()
    {
        var temp = interactionEvent.Next();
        Setname(temp[0]);
        SetContent(temp[1]);
    }
    public void ChoiceLeft() => SetContent(interactionEvent.ChoiceLeft());
    public void ChoiceRight() => SetContent(interactionEvent.ChoiceRight());

    public void Setname(string name)
    {
        namemesh.text = name;
    }

    public void SetContent(string _content)
    {
        StopCoroutine(co);
        co = Typing(_content);
        StartCoroutine(co);
        //content.text = _content;
    }

    IEnumerator Typing(string str)
    {
        content.text = null;
        if (str == "")
        {
            yield return null;
        }
        for (int i = 0; i < str.Length; i++)
        {
            content.text+= str[i];
            yield return new WaitForSeconds(typing_speed);
        }
    }

    public void test()
    {
        Debug.Log("call");
    }
}
