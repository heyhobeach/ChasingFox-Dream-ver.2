using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    public float destroyTime;

    private async void Start()
    {
        await DelayDestroy();
    }

    private async Awaitable DelayDestroy()
    {
        await Awaitable.WaitForSecondsAsync(destroyTime);
        Destroy(gameObject);
    }
}
