using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance;
    public static SystemManager Instance { get => instance; }

    public const string optionDataPath = "OptionData.json";
    public const string keybindDataPath = "KeybindData.json";

    public OptionData optionData;
    private OptionData defaultOptionData = new OptionData();

    public KeybindData keybindData;
    private KeybindData defaultKeybindData = new KeybindData();
    
    [SerializeField] private AudioMixer audioMixer;
    
    public Resolution[] resolutions { get; private set; }
    public int resolutionIndex { get; private set; }
    public string[] languages { get; private set; }
    public int languageIndex { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        defaultOptionData = new OptionData{
            fullscreen = true,
            resolutionIndex = 0,
            masterVolume = 1,
            musicVolume = 1,
            sfxVolume = 1,
            language = "KOR",
            lightEffect = true
        };
        defaultKeybindData = new KeybindData{

        };
        resolutions = new Resolution[]{
            new Resolution{width = 1920, height = 1080},
            new Resolution{width = 1600, height = 900},
            new Resolution{width = 1280, height = 720},
            new Resolution{width = 1024, height = 576},
            new Resolution{width = 800, height = 600},
            new Resolution{width = 640, height = 480}
        };
        languages = new string[]{ "KOR", "ENG" };

        try { optionData = LoadJson<OptionData>(optionDataPath); }
        catch (FileNotFoundException e)
        { 
            Debug.Log("OptionData not found. Creating new one.\n" + e);
            optionData = defaultOptionData; 
            SaveJson(optionDataPath, optionData); 
        }
        SetOption(optionData);
        try { keybindData = LoadJson<KeybindData>(keybindDataPath); }
        catch (FileNotFoundException e)
        { 
            Debug.Log("OptionData not found. Creating new one.\n" + e);
            keybindData = defaultKeybindData; 
            SaveJson(optionDataPath, optionData); 
        }

        resolutionIndex = optionData.resolutionIndex;
        languageIndex = System.Array.IndexOf(languages, optionData.language);

        if(DatabaseManager.instance && optionData.language != null) DatabaseManager.instance.ChangeLanguage(DatabaseManager.GetLangEnum(optionData.language));
    }

    public T LoadJson<T>(string fileName)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        string jsonData = File.ReadAllText(path);
        return JsonUtility.FromJson<T>(jsonData);
    }
    public void LoadJson<T>(string fileName, ref T data)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        string jsonData = File.ReadAllText(path);
        data = JsonUtility.FromJson<T>(jsonData);
    }

    public void SaveJson<T>(string fileName, T data)
    {
        string path = Path.Combine(Application.dataPath, fileName);
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, jsonData);
    }

    public void SetOption() => SetOption(optionData);
    public void SetOption(OptionData data)
    {
        SetFullScrean(data.fullscreen);
        SetVolume(data.masterVolume);
        SetMusicVolume(data.musicVolume);
        SetSFXVolume(data.sfxVolume);
        SetLightEffect(data.lightEffect);
        SaveJson(optionDataPath, optionData);
    }

    public void ResetOptionData()
    {
        optionData = defaultOptionData;
        SetOption(optionData);
    }

    public void ResetKeybindData()
    {
        keybindData = defaultKeybindData;
        SaveJson(keybindDataPath, keybindData);
    }

    public void SetVolume(float volume)
    {
        float vol = volume > 0 ? Mathf.Log10(volume) * 20 : -80;
        audioMixer.SetFloat("MasterVol", vol);
        optionData.masterVolume = vol;
    }
    public void SetMusicVolume(float volume)
    {
        float vol = volume > 0 ? Mathf.Log10(volume) * 20 : -80;
        audioMixer.SetFloat("BGVol", vol);
        optionData.musicVolume = vol;
    }
    public void SetSFXVolume(float volume)
    {
        float vol = volume > 0 ? Mathf.Log10(volume) * 20 : -80;
        audioMixer.SetFloat("SFXVol", vol);
        optionData.sfxVolume = vol;
    }
    // public static void PlaySFX(AudioClip audioClip) => Instance.musicsource[1].PlayOneShot(audioClip);

    public void SetFullScrean(bool isOn)
    {
        Screen.fullScreen = isOn;
        optionData.fullscreen = isOn;
    }

    public void SetLanguage(int index)
    {
        if(index < 0 || index >= languages.Length) return;
        optionData.language = languages[index];
        languageIndex = index;
        DatabaseManager.instance.ChangeLanguage(DatabaseManager.GetLangEnum(optionData.language));
    }
    public void LanguageLeft() => SetLanguage(languageIndex-1);
    public void LanguageRight() => SetLanguage(languageIndex+1);

    public void SetResolution(int index)
    {
        if(index < 0 || index >= resolutions.Length) return;
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        optionData.resolutionIndex = index;
        resolutionIndex = index;
    }
    public void ResolutionLeft() => SetResolution(optionData.resolutionIndex-1);
    public void ResolutionRight() => SetResolution(optionData.resolutionIndex+1);

    public void SetLightEffect(bool isOn)
    {
        optionData.lightEffect = isOn;
    }
}
