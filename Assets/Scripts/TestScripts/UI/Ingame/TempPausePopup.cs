using UnityEngine;
using UnityEngine.UI;

public class TempPausePopup : MonoBehaviour
{
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        Set();
    }

    public void Set()
    {
        // inventoryButton.onClick.AddListener(() => );
        restartButton.onClick.AddListener(() => GameManager.Instance.RetryScene());
    }
}
