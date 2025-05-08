using UnityEngine;
using UnityEditor; // 에디터 관련 네임스페이스
using System.IO;   // 파일 경로 관련 네임스페이스

public class SoundClipCreator
{
    // 메뉴 아이템 정의: "Assets/Create/SoundClip from AudioClip" 경로에 메뉴 추가
    // priority 값은 메뉴 순서를 결정 (낮을수록 위)
    [MenuItem("Assets/Create/SoundClip from AudioClip", false, 10)]
    private static void CreateSoundClipFromAudioClip()
    {
        // 현재 선택된 에셋 가져오기
        Object selectedObject = Selection.activeObject;

        // 선택된 에셋이 AudioClip인지 확인
        if (selectedObject is AudioClip selectedClip)
        {
            // 1. SoundClip 인스턴스 생성
            SoundClip soundClipInstance = ScriptableObject.CreateInstance<SoundClip>();

            // 2. 선택된 AudioClip 할당
            soundClipInstance.clip = selectedClip;

            // 3. playRange 설정 (SoundClip의 OnValidate 로직과 유사하게)
            soundClipInstance.playRange = new Vector2(0, selectedClip.length);

            // 4. 저장 경로 및 파일 이름 결정
            string path = AssetDatabase.GetAssetPath(selectedClip); // 원본 AudioClip 경로
            string directory = Path.GetDirectoryName(path); // 원본 디렉토리
            // 새 파일 이름 제안 (예: MyAudio_SoundClip.asset)
            string assetName = Path.GetFileNameWithoutExtension(path) + "_SoundClip.asset";
            // 중복되지 않는 고유한 경로 생성
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(directory, assetName));

            // 5. ScriptableObject 에셋 파일 생성
            AssetDatabase.CreateAsset(soundClipInstance, uniquePath);
            AssetDatabase.SaveAssets(); // 변경사항 저장
            AssetDatabase.Refresh();    // 프로젝트 뷰 갱신

            // 6. (선택 사항) 생성된 에셋을 선택하고 포커스
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = soundClipInstance;

            Debug.Log($"SoundClip created at: {uniquePath}");
        }
        else
        {
            Debug.LogWarning("Please select an AudioClip asset first.");
        }
    }

    // 메뉴 아이템 유효성 검사 함수
    // 이 함수가 true를 반환할 때만 위의 메뉴 아이템이 활성화됨
    [MenuItem("Assets/Create/SoundClip from AudioClip", true)]
    private static bool ValidateCreateSoundClipFromAudioClip()
    {
        // 선택된 에셋이 AudioClip일 때만 메뉴 활성화
        return Selection.activeObject is AudioClip;
    }
}