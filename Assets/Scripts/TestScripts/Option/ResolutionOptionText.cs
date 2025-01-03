using UnityEngine;

public class ResolutionOptionText : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI text;
    private Resolution[] resolutions;
    private int resolutionIndex;

    private void Start()
    {
        resolutions = SystemManager.Instance.resolutions;
        ChangeResolution();
    }

    public void ChangeResolution()
    {
        resolutionIndex = SystemManager.Instance.resolutionIndex;
        text.text = resolutions[resolutionIndex].width + "x" + resolutions[resolutionIndex].height;
    }
}
