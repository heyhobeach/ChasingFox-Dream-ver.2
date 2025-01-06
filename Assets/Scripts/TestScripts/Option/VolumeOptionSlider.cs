using UnityEngine;

public class VolumeOptionSlider : MonoBehaviour
{
    public enum VolumeType
    {
        Master,
        Music,
        SFX
    }

    [SerializeField] private UnityEngine.UI.Slider slider;
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        switch(volumeType)
        {
            case VolumeType.Master:
                slider.value = SystemManager.Instance.optionData.masterVolume;
                break;
            case VolumeType.Music:
                slider.value = SystemManager.Instance.optionData.musicVolume;
                break;
            case VolumeType.SFX:
                slider.value = SystemManager.Instance.optionData.sfxVolume;
                break;
        }
    }

    public void ChangeVolume()
    {
        switch(volumeType)
        {
            case VolumeType.Master:
                SystemManager.Instance.SetVolume(slider.value);
                break;
            case VolumeType.Music:
                SystemManager.Instance.SetMusicVolume(slider.value);
                break;
            case VolumeType.SFX:
                SystemManager.Instance.SetSFXVolume(slider.value);
                break;
        }
    }
}
