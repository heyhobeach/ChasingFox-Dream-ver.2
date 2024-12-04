using Unity.VisualScripting;
using UnityEngine;

public class DialogueController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public InteractionEvent interaction;

    public void SetDialogueNum(DialogueIndex scriptorble)//호출이 안되는듯
    {
        //InteractionEvent.Instance.indexNum = indexNum;
        //InteractionEvent.Instance.num = num;
        Debug.Log("dialogue controller");

        interaction.indexNum = scriptorble.indexNum;
        
        Debug.Log(interaction.indexNum + "," + interaction.num);
        interaction.GetDialogue();
        interaction.num = scriptorble.num;
        //InteractionEvent.Instance.indexNum = scriptorble.indexNum;
        //InteractionEvent.Instance.num=scriptorble.num;
        ////InteractionEvent.Instance.dialogue.line.x = InteractionEvent.Instance.dialogue.line.y + 1;
        //Debug.Log(InteractionEvent.Instance.indexNum + "," + InteractionEvent.Instance.num);
        //InteractionEvent.Instance.GetDialogue();
    }

    public void EventTest()
    {
        Debug.Log("event");
    }


}
