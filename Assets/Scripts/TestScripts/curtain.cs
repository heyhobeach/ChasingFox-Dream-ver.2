using UnityEditor;
using UnityEngine;

public class curtain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SpriteRenderer spriteRenderer;

    public float curtainTime = 8f;
   // private Color color;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //color=spriteRenderer.color;
    }

    private async void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //spriteRenderer.enabled = false;
        }
    }


    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //spriteRenderer.color = Color.white;
            await OpenRoom(curtainTime);
        }
    }
        private async void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            await ColseRoom(curtainTime);
        }
    }

    public async Awaitable OpenRoom(float time)//지금 스프라이트 랜더러에서 값이 변경이 안되는듯함
    {
        float current = 0;
        float value_a = 255;
        while(current < time)
        {
            await Awaitable.EndOfFrameAsync();
            current += Time.deltaTime / time;
            value_a=Mathf.Lerp(255,0, current);
            Debug.Log("알파값" + value_a);
            spriteRenderer.color = new Color(spriteRenderer.color.r/255f, spriteRenderer.color.g/255f, spriteRenderer.color.b / 255f, value_a/255f);
            //spriteRenderer.color.a= value_a;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r / 255f, spriteRenderer.color.g / 255f, spriteRenderer.color.b / 255f, 0);
    }

    public async Awaitable ColseRoom(float time)//지금 스프라이트 랜더러에서 값이 변경이 안되는듯함
    {
        float current = 0;
        float value_a = 255;
        while (current < time)
        {
            await Awaitable.EndOfFrameAsync();
            current += Time.deltaTime / time;
            value_a = Mathf.Lerp(0, 255, current);
            Debug.Log("알파값" + value_a);
            spriteRenderer.color = new Color(spriteRenderer.color.r / 255f, spriteRenderer.color.g / 255f, spriteRenderer.color.b / 255f, value_a / 255f);
            //spriteRenderer.color.a= value_a;
        }
        spriteRenderer.color = new Color(spriteRenderer.color.r / 255f, spriteRenderer.color.g / 255f, spriteRenderer.color.b / 255f, 1);
    }
}
