using UnityEngine;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    public GameObject enableObj;
    public GameObject disableObj;

    public Image slotImage;
    public TMPro.TextMeshProUGUI progressDateText;
    public TMPro.TextMeshProUGUI createdDateText;

    public Sprite[] images;

    public int slotIdx;
    private SaveData saveData;

    void Start()
    {
        saveData = SystemManager.Instance.GetSaveData(slotIdx);
        SlotUpdate();
    }

    public void SlotUpdate()
    {
        if(SystemManager.Instance.GetSaveData(slotIdx) == null)
        {
            enableObj.SetActive(false);
            disableObj.SetActive(true);
        }
        else
        {
            enableObj.SetActive(true);
            disableObj.SetActive(false);
            progressDateText.text = saveData.chapter;
            createdDateText.text = saveData.createdTime.ToString("yyyy-MM-dd HH:mm:ss");
            int index = Mathf.Clamp((int)(saveData.karma * (images.Length - 1) / 100f), 0, images.Length - 1);
            slotImage.sprite = images[index];
        }
    }
}
