using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelector : MonoBehaviour, IBaseController
{
    private Action _onDown;
    public Action onDown { get => _onDown; set => throw new NotImplementedException(); }
    private Action _onUp;
    public Action onUp { get => _onUp; set => throw new NotImplementedException(); }
    
    public Vector2Int arraySize = new Vector2Int(0, 1);

    [SerializeField] private Vector2Int buttonIdx = Vector2Int.zero;
    [SerializeField] public ButtonWrapper[] buttons = new ButtonWrapper[0];

    private int GetIndex(Vector2Int idx) => idx.x * arraySize.y + idx.y;
    private int GetIndex(int i, int j) => i * arraySize.y + j;

    public void SetButtonIndex(Vector2Int idx)
    {
        if(idx.x < 0 || idx.y < 0 || idx.x >= arraySize.x || idx.y >= arraySize.y || buttons[GetIndex(idx)] == null) return;
        buttonIdx = idx;
        EventSystem.current.SetSelectedGameObject(buttons[GetIndex(buttonIdx)].gameObject);
    }
    public void ButtonSelectMove(Vector2Int idx) => SetButtonIndex(buttonIdx + idx);
    public void ButtonSelect() => buttons[GetIndex(buttonIdx)].button?.Select();

    public void Controller()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)) ButtonSelectMove(Vector2Int.left);
        else if(Input.GetKeyDown(KeyCode.DownArrow)) ButtonSelectMove(Vector2Int.right);
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) ButtonSelectMove(Vector2Int.down);
        else if(Input.GetKeyDown(KeyCode.RightArrow)) ButtonSelectMove(Vector2Int.up);
        else if(Input.GetKeyDown(KeyCode.Return)) ButtonSelect();
    }

    private void Awake()
    {
        for(int i=0; i<arraySize.x; i++)
        {
            for(int j=0; j<arraySize.y; j++)
            {
                if(buttons[GetIndex(i, j)] != null) buttons[GetIndex(i, j)].onMouseEnter = () => SetButtonIndex(new Vector2Int(i, j));
            }
        }
    }
    private void Start() => SetButtonIndex(buttonIdx);
    private void OnEnable() => ((IBaseController)this).AddController();
}
