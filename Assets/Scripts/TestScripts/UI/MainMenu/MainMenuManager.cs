using Com.LuisPedroFonseca.ProCamera2D;
using MainMenuUI;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;
    public static MainMenuManager Instance { get => instance; }

    [SerializeField] private WindowNode currentNode;
    private Stack<WindowNode> nodeStack = new();
    private Vector3 prevPosition;
    [SerializeField, DisableInInspector] private Vector3 targetPosition;
    public float moveSpeed = 1f;
    private float progress = 0;
    private bool _isMoving = false;
    public bool isMoving { get => _isMoving; }

    [SerializeField] private MainPopupUI popupUI;
    [SerializeField] private GameObject eventSystem;
    [SerializeField] private SaveSlot[] saveSlots;

    void Start()
    {
        if(instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        nodeStack.Push(currentNode);
        targetPosition = currentNode.GetCenterPosition();
        prevPosition = Camera.main.transform.position;
    }

    private void Update()
    {
        if(!_isMoving) return;
        if(progress < moveSpeed)
        {
            progress += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(prevPosition, targetPosition, Com.LuisPedroFonseca.ProCamera2D.Utils.EaseFromTo(0, moveSpeed, progress));
            eventSystem.SetActive(false);
        }
        else 
        {
            progress = 0;
            Camera.main.transform.position = targetPosition;
            prevPosition = targetPosition;
            eventSystem.SetActive(true);
            _isMoving = false;
        }
    }

    void OnDestroy() => instance = null;

    public void MoveNextNode(WindowNode node)
    {
        if(nodeStack.Peek() == node) nodeStack.Pop();
        nodeStack.Push(node);
        currentNode = node;
        targetPosition = currentNode.GetCenterPosition();
        _isMoving = true;
    }

    public void MoveBackNode()
    {
        if(nodeStack.Count <= 1 || progress > 0) return; 
        nodeStack.Pop();
        MoveNextNode(nodeStack.Peek());
    }


    //--------------------------------------------------------------------------------

    public void SetOption() => SystemManager.Instance.SetOption();
    public void ResetOptionData() => SystemManager.Instance.ResetOptionData();
    public void ResolutionLeft() => SystemManager.Instance.ResolutionLeft();
    public void ResolutionRight() => SystemManager.Instance.ResolutionRight();
    public void LanguageLeft() => SystemManager.Instance.LanguageLeft();
    public void LanguageRight() => SystemManager.Instance.LanguageRight();

    public void SetKeybind() => SystemManager.Instance.SetKeybind();
    public void ResetKeybindData() => SystemManager.Instance.ResetKeybindData();

    public void PlayGame(int idx) => StartCoroutine(LoadScene(idx));
    private IEnumerator<WaitForSeconds> LoadScene(int idx)
    {
        SystemManager.Instance.SetSaveIndex(idx);
        var save = SystemManager.Instance.saveData;
        if(save == null) 
        {
            SystemManager.Instance.CreateData(idx);
            save = SystemManager.Instance.saveData;
            PageManger.Instance.LoadScene("Chp0", false);
        }
        else PageManger.Instance.LoadScene(save.chapter, false);
        PageManger.Instance.aoComplatedAction = () =>{
            ServiceLocator.Get<GameManager>().ApplySaveData();
        };
        yield return new WaitForSeconds(moveSpeed);
        PageManger.Instance.SceneActive();
    }
    public void QuitGame() => StartCoroutine(Quit());
    private IEnumerator<WaitForSeconds> Quit()
    {
        yield return new WaitForSeconds(moveSpeed);
        Application.Quit();
    }

    public void SaveDelete(int idx) => popupUI.SetPopup("정말 삭제하시겠습니까?\n <color=#f00000>삭제한 데이터는 복구할 수 없습니다.</color>", () => saveSlots[idx].DeleteData());
}
