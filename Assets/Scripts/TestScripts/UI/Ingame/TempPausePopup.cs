using UnityEngine;
using UnityEngine.UI;

public class TempPausePopup : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        Set();
    }

    public void Set()
    {
        continueButton.onClick.AddListener(() => 
        {
            GameManager.Instance.Pause(false);
            gameObject.SetActive(false);
        });
        restartButton.onClick.AddListener(() => {
            GameManager.Instance.RetryScene();
            gameObject.SetActive(false);
        });
        inventoryButton.onClick.AddListener(() =>GameManager.Instance.InventoryEnable() );
        // optionButton.onClick.AddListener(() => );
        mainMenuButton.onClick.AddListener(() => {
            GameManager.Instance.LoadScene("MainMenu");
            gameObject.SetActive(false);
        });
    }

    public void SetRestartButton(bool enabled)
    {
        restartButton.interactable = enabled;
    }
}
