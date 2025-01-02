using System.IO;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance;
    public static SystemManager Instance;

    public const string optionDataPath = "OptionData.json";
    public const string keybindDataPath = "KeybindData.json";

    public OptionData optionData;
    private OptionData defaultOptionData = new OptionData();

    public KeybindData keybindData;
    private KeybindData defaultKeybindData = new KeybindData();

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
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
        DontDestroyOnLoad(gameObject);

        optionData = LoadJson<OptionData>(optionDataPath);
        keybindData = LoadJson<KeybindData>(keybindDataPath);

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

    public void ResetOptionData()
    {
        optionData = defaultOptionData;
        SaveJson(keybindDataPath, optionData);
    }

    public void ResetKeybindData()
    {
        keybindData = defaultKeybindData;
        SaveJson(keybindDataPath, keybindData);
    }
}
