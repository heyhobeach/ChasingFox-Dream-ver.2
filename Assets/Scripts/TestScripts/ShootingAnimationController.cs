using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class ShootingAnimationController : MonoBehaviour
{
    public Animator headAnim;
    public Animator armAnim;
    public SpriteLibrary spriteLibrary;
    public SpriteLibraryAsset[] spriteLibraryAssets;
    public GameObject[] bodys;
    public GameObject shootPostion;

    private Coroutine attackCoroutine;
    private float angle;
    private SpriteRenderer body;
    private SpriteRenderer head;
    private SpriteRenderer arm;

    public bool isAttackAni { get => armAnim.GetCurrentAnimatorStateInfo(0).IsName("Attack"); }
    public bool isReloadAni { get => armAnim.GetCurrentAnimatorStateInfo(0).IsName("Reload"); }

    public float errorRange;

    private void OnDisable() => attackCoroutine = null;

    private void Start()
    {
        body = GetComponent<SpriteRenderer>();
        arm = bodys[0].transform.GetChild(0).GetComponent<SpriteRenderer>();
        head = bodys[1].GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if(!bodys[0].gameObject.activeSelf) return;
        if(body.flipX) foreach(var body in bodys)
        {
            var temp = body.transform.localPosition;
            temp.x = -temp.x;
            body.transform.localPosition = temp;
        }
        var thisPoint = bodys[0].transform.position;
        var tempPositon = (bodys[0].transform.position - transform.position).normalized * errorRange;
        thisPoint -= new Vector3(tempPositon.y, tempPositon.x);
        thisPoint.z = 0;
        shootPostion.transform.localPosition = new Vector3(shootPostion.transform.localPosition.x, arm.flipY ? -errorRange : errorRange);
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
        if(armAnim.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
        {
            arm.flipY = false;
            arm.flipX = body.flipX;
            bodys[0].transform.localEulerAngles = Vector2.zero;
        }
        else arm.flipX = false;
    }

    public void AttackAni()
    {
        if(attackCoroutine == null) attackCoroutine = StartCoroutine(Attacking());
        else waitTime = 0;
    }

    public void NomalAni()
    {
        if(attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        SpriteAssetChange(0);
    }
    float waitTime;
    private IEnumerator Attacking()
    {
        waitTime = 0;
        SpriteAssetChange(1);
        while(waitTime < 3)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        SpriteAssetChange(0);
        attackCoroutine = null;
    }

    public void Shoot() => armAnim.SetTrigger("attack");
    public void Reload() => armAnim.SetTrigger("reload");

    public void SpriteAssetChange(int i)
    {
        spriteLibrary.spriteLibraryAsset = spriteLibraryAssets[i];
        foreach(var go in bodys) go.SetActive(i == 0 ? false : true);
    }
    
    public Vector2 GetShootPosition() => (Vector2)shootPostion.transform.position;
    public Vector3 GetShootRotation() => shootPostion.transform.rotation.eulerAngles;
} 
