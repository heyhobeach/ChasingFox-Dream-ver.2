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
    [SerializeField, DisableInspector] private Vector3 targetPosition;
    public float moveSpeed = 1f;
    private float progress = 0;
    private bool _isMoving = false;
    public bool isMoving { get => _isMoving; }

    public MainPopupUI popupUI;
    public GameObject eventSystem;

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
            Camera.main.transform.position = Vector3.Lerp(prevPosition, targetPosition, Utils.EaseFromTo(0, moveSpeed, progress));
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

    public void PlayGame(string scene) => StartCoroutine(LoadScene(scene));
    private IEnumerator<WaitForSeconds> LoadScene(string scene)
    {
        yield return new WaitForSeconds(moveSpeed);
        PageManger.Instance.LoadScene(scene);
    }
    public void QuitGame() => StartCoroutine(Quit());
    private IEnumerator<WaitForSeconds> Quit()
    {
        yield return new WaitForSeconds(moveSpeed);
        Application.Quit();
    }

}
