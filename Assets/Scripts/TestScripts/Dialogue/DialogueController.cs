using UnityEngine;

public class DialogueController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SetDialogueNum(DialogueIndex scriptorble)//호출이 안되는듯
    {
        //InteractionEvent.Instance.indexNum = indexNum;
        //InteractionEvent.Instance.num = num;
        Debug.Log("dialogue controller");
        InteractionEvent.Instance.indexNum = scriptorble.indexNum;
        InteractionEvent.Instance.num=scriptorble.num;
        Debug.Log(InteractionEvent.Instance.indexNum + "" + InteractionEvent.Instance.num);
        InteractionEvent.Instance.GetDialogue();
    }


}
