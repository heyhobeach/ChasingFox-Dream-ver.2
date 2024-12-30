using System.IO;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance;
    public static SystemManager Instance;

    public OptionData optionData;
    private OptionData defaultOptionData = new OptionData();

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
        DontDestroyOnLoad(gameObject);

        LoadJson("OptionData.json", ref optionData);

        if(DatabaseManager.instance && optionData.language != null) DatabaseManager.instance.ChangeLanguage(DatabaseManager.GetLangEnum(optionData.language));
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
        SaveJson("OptionData.json", optionData);
    }
}
