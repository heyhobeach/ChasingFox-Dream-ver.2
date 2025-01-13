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
        calcelButton.onClick.AddListener(() => {
            confirmButton.onClick.RemoveAllListeners();
            gameObject.SetActive(false);
    });
    }

    private void OnEnable()
    {
        var pos = Camera.main.transform.position;
        pos.z = 0;
        transform.position = pos;
    }

    public void SetPopup(string info, System.Action confirmAction, bool enable = true)
    {
        SetConfirmButtonAction(confirmAction);
        SetInfoText(info);
        gameObject.SetActive(enable);
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
