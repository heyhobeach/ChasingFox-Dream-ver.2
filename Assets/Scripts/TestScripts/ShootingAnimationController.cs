using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ShootingAnimationController : MonoBehaviour
{
    public Animator headAnim;
    public Animator armAnim;
    public SpriteLibrary spriteLibrary;
    public SpriteLibraryAsset[] spriteLibraryAssets;
    public GameObject[] bodys;

    private Coroutine attackCoroutine;
    private float angle;
    private SpriteRenderer body;
    private SpriteRenderer head;
    private SpriteRenderer arm;

    private void Start()
    {
        body = GetComponent<SpriteRenderer>();
        arm = bodys[0].transform.GetChild(0).GetComponent<SpriteRenderer>();
        head = bodys[1].GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(!bodys[0].activeSelf) return;
        var thisPoint = transform.position;
        thisPoint.z = 0;
        var screenPoint = Input.mousePosition;
        screenPoint.z = Camera.main.transform.position.z;
        screenPoint = Camera.main.ScreenToWorldPoint(screenPoint);
        screenPoint.z = 0;
        var resultPoint = screenPoint - thisPoint;
        angle = Quaternion.FromToRotation(Vector2.down, resultPoint).eulerAngles.z;
        headAnim.SetFloat("angle", Vector2.Angle(Vector2.down, resultPoint));
        var flip = Mathf.Sign(resultPoint.x) >= 0 ? false : true;
        arm.flipY = flip;
        head.flipX = flip;
        bodys[0].transform.localEulerAngles = new Vector3(0, 0, angle - 90);
    }

    private void LateUpdate()
    {
        if(body.flipX) foreach(var body in bodys)
        {
            var temp = body.transform.localPosition;
            temp.x = -temp.x;
            body.transform.localPosition = temp;
        }
    }

    public void AttackAni()
    {
        if(attackCoroutine == null) attackCoroutine = StartCoroutine(Attacking());
    }

    private IEnumerator Attacking()
    {
        SpriteAssetChange(1);
        yield return new WaitForSeconds(10000000f);
        SpriteAssetChange(0);
        attackCoroutine = null;
    }

    public void Shoot() => armAnim.SetTrigger("attack");

    public void SpriteAssetChange(int i)
    {
        spriteLibrary.spriteLibraryAsset = spriteLibraryAssets[i];
        foreach(var go in bodys) go.SetActive(i == 0 ? false : true);
    }
    
} 
