using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundIMG : MonoBehaviour
{
    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(GameManager.Instance.player.transform.position.x, transform.position.y, 0) + (Vector3.right * sprite.sprite.rect.width);
    }
}
