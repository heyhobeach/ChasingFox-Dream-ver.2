using UnityEngine;
using UnityEngine.UI;

public class FullScreanOptionToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;

    public void OnToggle()
    {
        SystemManager.Instance.SetFullScrean(toggle.isOn);
    }
}
