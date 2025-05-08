using UnityEngine;
using UnityEngine.UI;

public class TempPausePopup : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
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
            ServiceLocator.Get<GameManager>().Pause(false);
            gameObject.SetActive(false);
        });
        restartButton.onClick.AddListener(() => {
            if(ServiceLocator.Get<GameManager>().RetryScene())
            {
                PageManger.Instance.SceneActive();
                gameObject.SetActive(false);
            }
        });
        // optionButton.onClick.AddListener(() => );
        mainMenuButton.onClick.AddListener(() => {
            ServiceLocator.Get<GameManager>().LoadScene("MainMenu");
            gameObject.SetActive(false);
        });
    }

    public void SetRestartButton(bool enabled)
    {
        restartButton.interactable = enabled;
    }
}
