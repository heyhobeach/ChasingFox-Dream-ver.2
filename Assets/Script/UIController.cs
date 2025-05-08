using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour//해당 스크립트는 UI를 그리는데 필요함
{
    //싱글톤 구현? 다른 ui시스템 스크립트를 넣어두고 여기서 관리 혹은 static으로 불러옴 근데 static으로 불러오는것보다는 싱글톤이 좋아보임
    [SerializeField] private UnityEngine.UI.Image image;//UI그림 이미지 담는 변수
    public bool reloadDoon = true;//재장전 완료를 알려주는 변수
    [SerializeField] private float duration = 1f;//그림 그리는데 필요한 시간
    public bool startCor = false;

    public bool is_dialogue_on = false;
    // Start is called before the first frame update
    public Canvas DialogueCanvas;

    public GameObject backgroundImage;

    // private static UIController instance = null;
    // public static UIController Instance
    // {
    //     get
    //     {
    //         if (instance == null) { return null; }
    //         return instance;
    //     }
    // }
    private void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }
    private void Awake()
    {
        // if (instance != null)
        // {
        //     Destroy(this.gameObject);
        //     return;
        // }
        // instance = this;
        ServiceLocator.Register(this);
        DialogueCanvasSetFalse();
        //DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        image.fillAmount = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator DrawReload()
    {
        Debug.Log("코루틴 시작");
        float currentTime = 0.0f;
        // float startFillAmout = 0f;
        // float endFillAmout = 1.0f;
        startCor = true;

        while (currentTime <= duration)//재장전 확인값 던져주는지 체크 아마 currentTime<duration&&reloadStart
        {
            reloadDoon = false;
            float t = currentTime / duration;
            //float currentValue = Mathf.Lerp(startFillAmout,endFillAmout, t);
            //image.fillAmount = t;

            currentTime += Time.deltaTime;
            //Debug.Log("재장전중" + currentTime);
            yield return null;
        }

        if (currentTime >= duration)//완료상태
        {
            //Debug.Log("완료");
            reloadDoon = true;
            //currentTime = 0.0f; 
            Debug.Log("1초 후");
            startCor = false;
            //yield return new WaitForSecondsRealtime(1);

        }
        //yield return true;
        //yield return null;//이게 밖에있어야함 무한 루프기준

    }

    public void _DrawReload()//ControllerScript 의 temp를 받아서 가능함 재장전 그리는 애니메이션
    {
        //재장전 애니메이션 및 내용들
        image.fillAmount = ControllerScript.Instance.currentTime / ControllerScript.Instance.duration;//시간 관련 변수들은 ContollerScript의 _DrawReload함수에서 측정중

    }

    public void DrawReload(float fill) => image.fillAmount = fill;

    public void ImageSetFalse()
    {
        image.transform.gameObject.SetActive(false);
    }
    public void ImageSetTrue()
    {
        image.transform.gameObject.SetActive(true);
    }

    public void DialogueCanvasSetFalse()
    {
        Debug.Log("SetFalse");
        //DialogueCanvas.transform.gameObject.SetActive(false);
        is_dialogue_on = false;

        if (DialogueCanvas.transform.gameObject.GetComponent<Canvas>() == null)
        {
            Debug.LogError("Null입니다");
        }
        DialogueCanvas.GetComponent<UIManager>().Setclear();
        DialogueCanvas.transform.gameObject.GetComponent<Canvas>().enabled = false;
        backgroundImage?.SetActive(false);
    }
    public void DialogueCanvasSetTrue()
    {
        Debug.Log("SetTrue"); 
        is_dialogue_on= true;
        //DialogueCanvas.transform.gameObject.SetActive(true);
        //bool is_alone = false;
        //InteractionEvent.Instance.SetSkip(true);
        DialogueCanvas.GetComponent<UIManager>().LoadImage();
        DialogueCanvas.transform.gameObject.GetComponent<Canvas>().enabled = true;
        backgroundImage.transform.localPosition = new Vector3(0, 0, 1);
        backgroundImage.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.88f);
        backgroundImage?.SetActive(true);
    }
    public void TestSignal()
    {
        Debug.Log("TestSignal");
    }


}
