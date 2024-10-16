using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour, IBaseController
{
    private VisualElement root;
    private VisualElement[] buttons = new VisualElement[4];
    [SerializeField, DisableInspector] private int _buttonIdx = 0;
    private int buttonIdx { get => _buttonIdx; set => _buttonIdx = Mathf.Clamp(value, 0, buttons.Length); }

    public void Controller()
    {
        if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) OnButtonHover(++buttonIdx);
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) OnButtonHover(--buttonIdx);
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) OnButtonClick(buttonIdx);
    }

    // Start is called before the first frame update
    void Awake()
    {
        ((IBaseController)this).AddController();

        root = GetComponent<UIDocument>().rootVisualElement;
        buttons[0] = root.Q("Button-NewGame");
        buttons[1] = root.Q("Button-LoadGame");
        buttons[2] = root.Q("Button-Option");
        buttons[3] = root.Q("Button-Quit");

        buttons[0].RegisterCallback<ClickEvent>((evt) => OnButtonClick(0));
        buttons[1].RegisterCallback<ClickEvent>((evt) => OnButtonClick(1));
        buttons[2].RegisterCallback<ClickEvent>((evt) => OnButtonClick(2));
        buttons[3].RegisterCallback<ClickEvent>((evt) => OnButtonClick(3));

        buttons[0].RegisterCallback<MouseOverEvent>((evt) => OnButtonHover(0));
        buttons[1].RegisterCallback<MouseOverEvent>((evt) => OnButtonHover(1));
        buttons[2].RegisterCallback<MouseOverEvent>((evt) => OnButtonHover(2));
        buttons[3].RegisterCallback<MouseOverEvent>((evt) => OnButtonHover(3));
    }

    private void OnButtonClick(int idx)
    {
        switch(idx)
        {
            case 0:
                GameManager.Instance.LoadScene("Chp0");
                break;
            case 1:
                GameManager.Instance.LoadScene("Chp0");
                break;
            case 2:
                break;
            case 3:
                Application.Quit();
                break;
        }
    }
    private void OnButtonHover(int idx)
    {
        buttonIdx = idx;
        foreach(var button in buttons) button.RemoveFromClassList("OnHover");
        buttons[idx].AddToClassList("OnHover");
    }

    void OnDestroy() => ((IBaseController)this).RemoveController();
}
