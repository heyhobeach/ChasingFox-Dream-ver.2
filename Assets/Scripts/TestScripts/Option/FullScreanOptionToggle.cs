using UnityEngine;
using UnityEngine.UI;

public class FullScreanOptionToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    private void Start()
    {
        toggle.isOn = SystemManager.Instance.optionData.fullscreen;
    }

    public void OnToggle()
    {
        SystemManager.Instance.SetFullScrean(toggle.isOn);
    }
}
