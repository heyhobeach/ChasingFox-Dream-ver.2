using UnityEngine;
using UnityEditor;
using System.IO;

public class SoundClipPackageCreator
{
    [MenuItem("Assets/Create/SoundClipPackage", false, 10)]
    private static void CreateSoundClipFromAudioClip()
    {
        SoundClip[] soundClips = Selection.GetFiltered<SoundClip>(SelectionMode.Assets);
        if (soundClips.Length == 0)
        {
            Debug.LogWarning("Please select at least one SoundClip asset.");
            return;
        }

        SoundClipPackage soundClipPackageInstance = ScriptableObject.CreateInstance<SoundClipPackage>();
        soundClipPackageInstance.soundClips = new SoundClip[soundClips.Length];
        for (int i = 0; i < soundClips.Length; i++)
        {
            soundClipPackageInstance.soundClips[i] = soundClips[i];
        }

        string path = AssetDatabase.GetAssetPath(soundClips[0]);
        string directory = Path.GetDirectoryName(path);
        string assetName = "New_SoundClipPackage.asset";
        string uniquePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, assetName));

        AssetDatabase.CreateAsset(soundClipPackageInstance, uniquePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = soundClipPackageInstance;

        Debug.Log($"SoundClipPackage created at: {uniquePath}");
    }

    [MenuItem("Assets/Create/SoundClipPackage", true)]
    private static bool ValidateCreateSoundClipFromAudioClip()
    {
        return Selection.GetFiltered<SoundClip>(SelectionMode.Assets).Length > 0;
    }
}
