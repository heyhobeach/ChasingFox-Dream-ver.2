using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    //�̱��� ����? �ٸ� ui�ý��� ��ũ��Ʈ�� �־�ΰ� ���⼭ ���� Ȥ�� static���� �ҷ��� �ٵ� static���� �ҷ����°ͺ��ٴ� �̱����� ���ƺ���
    [SerializeField] private UnityEngine.UI.Image image;
    public bool reloadDoon = true;
    [SerializeField] private float duration = 1f;
    public bool startCor = false;
    // Start is called before the first frame update

    private static UIController instance=null;
    public static UIController Instance 
    { 
        get 
        {
            if(instance == null) { return null; }
            return instance; 
        } 
    }

    private void Awake()
    {
        if(instance!= null)
        {
            Destroy(this.gameObject);
            return; 
        }instance = this;
        //DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ControllerScript.Instance.b_reload)
        {
            _DrawReload();
            Debug.Log("�׸�����");
        }
        else
        {
            Debug.Log("�׸���");
        }

        
        
    }

    public IEnumerator DrawReload()
    {
        Debug.Log("�ڷ�ƾ ����");
        float currentTime = 0.0f;
        float startFillAmout = 0f;
        float endFillAmout = 1.0f;
        startCor = true;

        while (currentTime <= duration)//������ Ȯ�ΰ� �����ִ��� üũ �Ƹ� currentTime<duration&&reloadStart
        {
            reloadDoon = false;
            float t = currentTime / duration;
            //float currentValue = Mathf.Lerp(startFillAmout,endFillAmout, t);
            //image.fillAmount = t;

            currentTime += Time.deltaTime;
            //Debug.Log("��������" + currentTime);
            yield return null;
        }

        if (currentTime >= duration)//�Ϸ����
        {
            //Debug.Log("�Ϸ�");
            reloadDoon = true;
            //currentTime = 0.0f; 
            Debug.Log("1�� ��");
            startCor = false;
            //yield return new WaitForSecondsRealtime(1);
            
        }
        //yield return true;
        //yield return null;//�̰� �ۿ��־���� ���� ��������
        
    }

    public void _DrawReload()//ControllerScript �� temp�� �޾Ƽ� ������
    {
        //������ �ִϸ��̼� �� �����
        image.fillAmount = ControllerScript.Instance.currentTime / ControllerScript.Instance.duration;
        
    }


}
