using System.Collections;
using UnityEngine;

public class CameraFadeTransition : MonoBehaviour
{
    public Camera mainCamera;
    public Camera secondaryCamera;
    public float transitionTime = 1.0f;

    void OnEnable() => StartCoroutine(FadeToCamera(secondaryCamera));

    IEnumerator FadeToCamera(Camera targetCamera)
    {
        float elapsedTime = 0f;

        // 시작 시 두 카메라의 투명도 (예를 들어 black overlay 사용 가능)
        CanvasGroup canvasGroup = new GameObject("FadeCanvas").AddComponent<CanvasGroup>();
        canvasGroup.gameObject.AddComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        canvasGroup.transform.SetParent(mainCamera.transform);
        canvasGroup.alpha = 0f;

        // 페이드 아웃
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / transitionTime);
            yield return null;
        }

        // 카메라 전환
        mainCamera.gameObject.SetActive(false);
        targetCamera.gameObject.SetActive(true);

        // 페이드 인
        elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / transitionTime);
            yield return null;
        }

        // 완료 후 삭제
        Destroy(canvasGroup.gameObject);
    }
}
