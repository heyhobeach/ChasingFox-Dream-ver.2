using UnityEditor;
using UnityEngine;

public class curtain : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SpriteRenderer spriteRenderer;

    public float curtainTime = 8f;
    private Color color;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        color=spriteRenderer.color;
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
            await OpenRoom(curtainTime);
        }
    }
        private async void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //spriteRenderer.enabled = true;
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
            color = new Color(color.r/255f, color.g/255f, color.b / 255f, value_a/255f);
            //color.a = value_a;
        }
    }
}
