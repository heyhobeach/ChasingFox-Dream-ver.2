using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using static JsonUtils.Utils;

public class SystemManager : MonoBehaviour
{
    private static SystemManager instance;
    public static SystemManager Instance { get => instance; }

    public const string optionDataPath = "OptionData.od";
    public const string keybindDataPath = "KeybindData.kb";

    public OptionData optionData;
    private OptionData defaultOptionData = new OptionData();

    public KeybindData keybindData;
    private KeybindData defaultKeybindData = new KeybindData();

    private SaveData[] saveDatas = new SaveData[3];
    public int saveIndex = 0;
    public SaveData saveData { get => saveDatas[saveIndex]; }
    private SaveData defaultSaveData = new SaveData();
    
    [SerializeField] private AudioMixer audioMixer;
    
    public Resolution[] resolutions { get; private set; }
    public int resolutionIndex { get; private set; }
    public string[] languages { get; private set; }
    public int languageIndex { get; private set; }

    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();

    private void OnDistroy()
    {
        SaveJson(optionDataPath, optionData);
        SaveJson(keybindDataPath, keybindData);
    }

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
            masterVolume = 0.5f,
            musicVolume = 0.5f,
            sfxVolume = 0.5f,
            language = "KOR",
            lightEffect = true
        };
        defaultKeybindData = new KeybindData{
            moveLeft = KeyCode.A,
            moveRight = KeyCode.D,
            jump = KeyCode.W,
            crouch = KeyCode.S,
            attack = KeyCode.Mouse0,
            reload = KeyCode.R,
            dash = KeyCode.Space,
            formChange = KeyCode.LeftShift,
            retry = KeyCode.R
        };
        defaultSaveData = new SaveData{
            chapter = "Chp0",
            mapDatas = null,
            eventTriggerDatas = null,
            eventTriggerInstanceID = 0,
            eventIdx = 0,
            karma = 65
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
        try { keybindData = LoadJson<KeybindData>(keybindDataPath); }
        catch (FileNotFoundException e)
        { 
            Debug.Log("KeybindData not found. Creating new one.\n" + e);
            keybindData = defaultKeybindData; 
            SaveJson(keybindDataPath, keybindData); 
        }

        for(int i = 0; i < saveDatas.Length; i++) LoadData(i);

        resolutionIndex = optionData.resolutionIndex;
        languageIndex = System.Array.IndexOf(languages, optionData.language);

        SetOption(optionData);
        foreach(FieldInfo key in keybindData.GetType().GetFields())
        {
            keybinds.Add(key.Name, (KeyCode)key.GetValue(keybindData));
        }

        if(DatabaseManager.instance && optionData.language != null) DatabaseManager.instance.ChangeLanguage(DatabaseManager.GetLangEnum(optionData.language));
    }

    // ------------------- Option -------------------

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

    private float ConvertDbToLinear(float db)
    {
        return Mathf.Pow(10, db / 20);
    }

    private float ConvertLinearToDb(float linear)
    {
        return linear > 0 ? 20 * Mathf.Log10(linear) : -80;
    }

    public void SetVolume(float volume)
    {
        float vol = ConvertLinearToDb(volume);
        audioMixer.SetFloat("MasterVol", vol);
        optionData.masterVolume = volume;
    }
    public void SetMusicVolume(float volume)
    {
        float vol = ConvertLinearToDb(volume);
        audioMixer.SetFloat("BGVol", vol);
        optionData.musicVolume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        float vol = ConvertLinearToDb(volume);
        audioMixer.SetFloat("SFXVol", vol);
        optionData.sfxVolume = volume;
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
        DatabaseManager.instance?.ChangeLanguage(DatabaseManager.GetLangEnum(optionData.language));
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

    // ------------------- Keybind -------------------

    public void ResetKeybindData()
    {
        keybindData = defaultKeybindData;
        SetKeybind();
    }

    public KeyCode GetKeybind(string keyName) => keybinds[keyName];

    public void SetKeybind()
    {
        foreach(FieldInfo key in keybindData.GetType().GetFields())
        {
            SetKeybind(key.Name, keybinds[key.Name]);
        }
        SaveJson(keybindDataPath, keybindData);
    }
    public void SetKeybind(string keyName, KeyCode keyCode)
    {
        keybinds[keyName] = keyCode;
        FieldInfo fieldInfo = keybindData.GetType().GetField(keyName);
        fieldInfo.SetValue(keybindData, keyCode);
    }

    public static bool GetButton(string keyName) => Input.GetKey(instance.keybinds[keyName]);
    public static bool GetButtonDown(string keyName) => Input.GetKeyDown(instance.keybinds[keyName]);
    public static bool GetButtonUp(string keyName) => Input.GetKeyUp(instance.keybinds[keyName]);
    public static int GetAxis(string keyName)
    {
        if(keyName.Equals("Horizontal"))
        {
            if(Input.GetKeyDown(instance.keybinds["moveLeft"]) || Input.GetKey(instance.keybinds["moveLeft"])) return -1;
            else if(Input.GetKeyDown(instance.keybinds["moveRight"]) || Input.GetKey(instance.keybinds["moveRight"])) return 1;
        }
        
        return 0;
    }

    // ------------------- Save -------------------

    public void SetSaveIndex(int index) => saveIndex = index;
    public SaveData GetSaveData(int index) => saveDatas[index];

    public void SaveData(int index)
    {
        if(index < 0 || index >= saveDatas.Length) return;
        SaveJson("SaveData" + index + ".sv", saveDatas[index]);
    }
    public SaveData LoadData(int index)
    {
        if(index < 0 || index >= saveDatas.Length) return null;
        try { saveDatas[index] = LoadJson<SaveData>("SaveData" + index + ".sv"); }
        catch (FileNotFoundException e)
        { 
            Debug.Log("SaveData" + index + " not found.\n" + e);
            saveDatas[index] = null; 
        }
        return saveDatas[index];
    }
    public void DeleteData(int index)
    {
        if(index < 0 || index >= saveDatas.Length) return;
        try { DeleteJson("SaveData" + index + ".json"); }
        catch (FileNotFoundException e) { Debug.Log("SaveData" + index + " not found.\n" + e); }
        saveDatas[index] = null;
    }
    public void CreateData(int index)
    {
        if(index < 0 || index >= saveDatas.Length) return;
        saveDatas[index] = new SaveData();
        saveDatas[index].Init(DateTime.Now, defaultSaveData);
        SaveData(index);
    }

    public SaveData GetData() => saveDatas[saveIndex];

    public void UpdateDataForEventTrigger(int eventTriggerInstanceID, int eventIdx)
    {
        if(saveData == null) return;
        saveData.eventTriggerInstanceID = eventTriggerInstanceID;
        saveData.eventIdx = eventIdx;
    }
}
