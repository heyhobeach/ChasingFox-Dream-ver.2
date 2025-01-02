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
    public bool isMoving { get => progress > 0; }

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
        if(Vector3.Distance(Camera.main.transform.position, targetPosition) > 0.1f)
        {
            progress += moveSpeed * Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(prevPosition, targetPosition, progress);
            eventSystem.SetActive(false);
        }
        else 
        {
            progress = 0;
            Camera.main.transform.position = targetPosition;
            prevPosition = targetPosition;
            eventSystem.SetActive(true);
        }
    }

    void OnDestroy() => instance = null;

    public void MoveNextNode(WindowNode node)
    {
        if(nodeStack.Peek() == node) nodeStack.Pop();
        nodeStack.Push(node);
        currentNode = node;
        targetPosition = currentNode.GetCenterPosition();
    }

    public void MoveBackNode()
    {
        if(nodeStack.Count <= 1 || progress > 0) return; 
        nodeStack.Pop();
        MoveNextNode(nodeStack.Peek());
    }
}
