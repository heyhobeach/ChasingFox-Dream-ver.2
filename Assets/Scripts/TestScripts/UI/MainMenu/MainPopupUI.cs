using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainPopupUI : MonoBehaviour
{
    [SerializeField] private Button calcelButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text infoText;

    private void Start()
    {
        calcelButton.onClick.AddListener(() => gameObject.SetActive(false));
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.position = Camera.main.transform.position;
    }

    public void SetPopup(string info, System.Action confirmAction)
    {
        SetConfirmButtonAction(confirmAction);
        SetInfoText(info);
    }

    public void SetConfirmButtonAction(System.Action action)
    {
        confirmButton.onClick.AddListener(() =>
        {
            action?.Invoke();
            gameObject.SetActive(false);
        });
    }

    public void SetInfoText(string text)
    {
        infoText.text = text;
    }
}
