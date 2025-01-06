using UnityEngine;

public class LanguageOptionText : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    private string[] languages;
    private int languageIndex;

    private void Start()
    {
        languages = SystemManager.Instance.languages;
        ChangeLanguage();
    }

    public void ChangeLanguage()
    {
        languageIndex = SystemManager.Instance.languageIndex;
        text.text = languages[languageIndex];
    }
}
