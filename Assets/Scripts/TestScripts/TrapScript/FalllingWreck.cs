using UnityEngine;
using Damageables;
using System;

public class FalllingWreck : MonoBehaviour
{
    [Tooltip ("함정 데미지 설정하는 변수")]
    public int damage = 3;//함정의 데미지

    [Tooltip("함정 지속시간 설정 변수")]
    public float trapDurationTime = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await TrapDuration(trapDurationTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isDamaged = false;//적에게 바로 충돌하는 함수
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))//
        {
            Debug.Log("적충돌");
            var temp = collision.gameObject.GetComponent<IDamageable>();
            Func<Collider2D, Vector2> func = null;
            //func += DamagedFeedBack;
            if (temp != null) 
            {
                isDamaged = temp.GetDamage(damage, collision, func);
            }
        
            if (isDamaged) { 
                Debug.Log("isDamaged is not null");
                //Destroy(gameObject);
            }
            else
            {
                Debug.Log("isDamaged is null");
            }
        }
    }

    public async Awaitable TrapDuration(float time)
    {
        await Awaitable.WaitForSecondsAsync(time);
        Destroy(this.gameObject);
    }
}
