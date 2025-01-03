using System.Collections;
using UnityEngine;

public class KeybindOptionText : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI text;
    [SerializeField] private GameObject bindingPanel;
    [SerializeField] private string keyName;

    private void Start()
    {
        text.text = SystemManager.Instance.GetKeybind(keyName).ToString();
    }

    public void ChangeKeybind()
    {
        StartCoroutine(WaitForKey());
    }

    private IEnumerator WaitForKey()
    {
        bindingPanel?.SetActive(true);
        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
            {
                SystemManager.Instance.SetKeybind(keyName, kcode);
                text.text = kcode.ToString();
                break;
            }
        }
        bindingPanel?.SetActive(false);
    }
}
