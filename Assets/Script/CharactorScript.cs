using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Timers;
using TMPro;

public partial class ControllerScript : MonoBehaviour
{
    [SerializeField] GameObject attackPoint;//공격지점
    public GameObject bullet;//총알 오브젝트 연결
    public GameObject meleeAttack;//공격하는 오브젝트 연결

    public class Charactor//상속의 구조가 그냥 값에 대한 복사이기 때문에 생기는 문제
    {
        public float speed;
        public bool isHuman = true;
        public Vector3 hidePos;
        public int damage = 0;
        public float reloadTime = 1;

        public static int ammo = 2;
        public int spare_ammo = 0;//여분 탄환을 만들기 위함




        public virtual void Die(Charactor charactor)
        {

        }

        public void SetInfo()//해당 함수로 폼 체인지시 변하는 스테이스 설정 함수를 모아서 실행시킬것임
        {
            Setspeed();
            Setdamage();
            SetReloadTime();
            Debug.Log("장탄수" + ammo);
        }

        public virtual void Setspeed() { }
        public virtual void Setdamage() { }
        public virtual void SetReloadTime() { }

        public virtual void Attack() { }
        public void Crouch(GameObject guard)//ref bool 로 접근
        {

            Debug.Log("크라우치");
            guard.SetActive(true);//크라우치 행동시 가드 오브젝트를 활성화 시킴


        }//늑대와 인간 둘다 웅크림이 같다고 가정하기에 그냥 일반함수로 구현

        public void Reload()
        {
            if (spare_ammo > 0)//인간 상태이며 여분 총알이 1발 이상이라면 재장전 실행
            {
                if (ControllerScript.Instance._DrawReload(ref ControllerScript.instance.b_reload))//재장전 애니메이션 그리고 해당 애니메이션이 성공적으로 그려졌다면 아래 실행 그리고 _DrawReload함수에서 재장전 중인지 아닌지 판단하는 b_reload변수 컨트롤
                {
                    this.spare_ammo--;
                    ammo++;
                }
            }
        }
        public void GetAmmo()//해당함수는 일단 접근이 Human.Instance().GetAmmo로 되어있지만 나중에 가상함수로 만들어서 늑대에서도 접근 가능하도록 해도 되지만 굳이라는 느낌이 존재함
        {
            if (ammo < 2)//총알이 2발보다 아래일때 여분 총알을 올림
            {
                spare_ammo++;
            }
        }
    }

    public class WereWolf : Charactor
    {
        public int life = 2;//늑대의 생명을 2로 설정했지만 실제로는 1이라서 나중에 수정 필요함
        private static WereWolf instance;//싱글턴을 위한 변수
                                         //IEnumerator tTimer;
        public bool isAttacking = false;//공격 보여주고 하는 용 변수 공격시간 등 체크할때 사용되는중

        //private bool showAttack = false;//해당 변수는 사용 안 해서 주석처리 함

        float t = 0;

        public static WereWolf Instance()//늑대 인스턴트등 쉽게 교체하기 위함 어차피 늑대와 인간은 한번생성되면 더 생성이 안될것이라고 생각했음
        {
            if (instance == null)
            {
                //Debug.Log("비었음 생성");
                instance = new WereWolf();
                //늑대 생성시 부터 그냥 코루틴을 시작시켰음 그럼으로 인해 공격상태에 대한 isAttacking을 통해 코루틴 시작 및 재시작? 을 조절함
                ControllerScript.instance.StartCoroutine(instance.timer());//코루틴 호출 부분이 조금 수정 할 필요있을것 같음 전지전능한 잼미니에게 물어보면 StartCorutine을 그냥 가상함수로 만들라고 하긴하더라

            }
            else
            {
                //Debug.Log("안 비었기때문에 기존거 전달");
            }
            return instance;
        }

        private WereWolf()
        {
            //if (instance == null) Debug.Log("늑대");
        }

        public override void Setspeed()
        {
            Debug.Log("늑대 속도");
            this.speed = 5.0f;
        }
        public override void Setdamage()
        {
            Debug.Log("늑대 공격력");
            ControllerScript.instance.meleeAttack.SetActive(false);
            this.damage = 2;
        }
        public override void SetReloadTime()
        {
            this.reloadTime = 2;
        }
        public override void Attack()//다시 공격하는것에 대한 기준이 애매했음 일단은 대충 내생각대로 설정하기는 했는데 오류가 조금 존재하기는함
        {
            Debug.Log("늑대 공격");
            //float startAttack = Time.time;
            float startAttack = 0;
            t = 0;
            ControllerScript.instance.meleeAttack.transform.position = ControllerScript.instance.attackPoint.transform.position;//meleeAttack오브젝트의 위치를 처음 설정한 attackPoint로 이동시킴


            //ControllerScript.instance.meleeAttack.SetActive(true);

            //if (!showAttack)
            //{
            if (!isAttacking)//공격한 적이 없을때 즉 첫 공격일경우
            {
                //
                ControllerScript.instance.meleeAttack.SetActive(true);
                Vector2 temp = (ControllerScript.instance.ClickPos() - ControllerScript.instance.transform.position).normalized;
                temp = temp.normalized;//짧은 거리도 1로 맞춰주기 귀함
                Debug.Log(temp);
                if (ControllerScript.instance.isGround)
                {
                    ControllerScript.instance.rg2d.velocity = Vector2.zero;//addforce를 계속하면 남아있는 velocity에 계속 합산이 되어서 초기화 시키고 다시 날리기위함
                    ControllerScript.instance.rg2d.AddForce(temp * 4, ForceMode2D.Impulse);//공격시 날아가는것? 이동을 위함, 공격한 방향으로 움직여햐기때문에 y축도 같이 주었고 대충 테스트 했을때 공격이 연이어서 들어가면 이동이 뺠라짐 addForce가 초기화가 안 되어있어서 그런것 같음
                }
                isAttacking = true;//(공격중이라고 판단)
            }
            //else
            //{
            //    temp = new Vector2(temp.x, 0);
            //    ControllerScript.instance.rg2d.AddForce(temp * 4, ForceMode2D.Impulse);
            //}
            //}
            //isAttacking= true;
            //showAttack = true;




            //ControllerScript.instance.StartCoroutine(tTimer);

            //Debug.Log("끝");
            //invoke(eraseMelee,1);//만약 invoke로 만든다면

        }
        IEnumerator timer()
        {
            while (true)
            {
                //만약 연속 공격하는게 어색하다면 아래 공격 지속시간을 짧게 함으로서 이상함을 줄여보고 그래도 이상하다면 수정 필요
                if (isAttacking)//if를 지워도 되지 않을까 어차피 클릭자체가 이벤트이자 if임 , if를 지우면 1.x초에서 클릭하면 잠깐 보이고 없어짐(해당 아이디어는 괜찮았으나 코루틴이 계속 움직이기에 쿨타임이 정확하게 움직이려면 if가 필요했음)
                {

                    //if (t < 1.0f)
                    //{
                    //    ControllerScript.instance.meleeAttack.SetActive(true);
                    //    t += Time.deltaTime;
                    //
                    //}
                    //else
                    //{
                    //    ControllerScript.instance.meleeAttack.SetActive(false);
                    //    showAttack = false;
                    //    t = 0;
                    //}
                    //만약 공격이 연속해서 나갈 수 없다면 아래 부분 그래도 아마 위에 부분 응용해서 만드는게 자연스러울듯
                    Debug.Log("보임");
                    yield return new WaitForSeconds(0.5f);//0.5초 이후 공격 오브젝트를 안 보이게 함

                    ControllerScript.instance.meleeAttack.SetActive(false);
                    //showAttack = false;
                    isAttacking = false;//공격 종료로 판단

                }

                yield return null;
            }

        }
        public void SetFalse()//이거는 암 ㅏ없어도 될듯 위에서 그냥 문구로 다 처리해서 그걸 대체하고자 할때 필요함
        {
            ControllerScript.instance.meleeAttack.SetActive(false);
        }

        float Getspeed()//혹시나 이동속도 얻을경우가 있을까봐 만든 부분 현재 사용되지는않음
        {
            return this.speed;
        }

    }

    public class Human : Charactor
    {
        //public int ammo = 2;
        //public int spare_ammo = 0;//여분 탄환을 만들기 위함
        private static Human instance;//위에 늑대와 이유 동일



        private Human() { }
        public static Human Instance()
        {
            if (instance == null)
            {
                return instance = new Human();
            }
            else
            {
                return instance;
            }
        }
        public override void Attack()
        {
            if (ammo <= 0)//총알이 없을때 사격 불가
            {
                Debug.Log("장탄수 부족");
                return;
            }
            Debug.Log("사람 공격 빵야");
            Vector3 temp = ControllerScript.instance.ClickPos();//클릭위치를 담는 변수
            GameObject _bullet = Instantiate(ControllerScript.instance.bullet, ControllerScript.instance.attackPoint.transform.position, ControllerScript.instance.transform.rotation);//총알을 공격포지션에서 생성함
            _bullet.GetComponent<BulletScript>().targetPos = temp;//클릭한 위치가 타겟 위치임
            _bullet.GetComponent<BulletScript>().shootPos = ControllerScript.instance.transform.position;//나중에 거리따라서 총알을 삭제할때 필요한것 같아서 만들었음, 말고도 이동 방향설정을 위해 필요한 부분

            //Debug.Log(ControllerScript.instance.worldPosition);
            ammo--;//사격이 이루어졌다면 총알을 줄임
            Debug.Log($"남은 장탄수{ammo}");

        }
        public override void Setspeed()
        {
            Debug.Log("사람 속도");
            this.speed = 3.0f;
        }
        public override void Setdamage()
        {
            Debug.Log("사람 공격력");
            this.damage = 2;
        }
        public override void SetReloadTime()
        {
            this.reloadTime = 1;
        }
        //public void GetAmmo()//해당함수는 일단 접근이 Human.Instance().GetAmmo로 되어있지만 나중에 가상함수로 만들어서 늑대에서도 접근 가능하도록 해도 되지만 굳이라는 느낌이 존재함
        //{
        //    if (ammo < 2)//총알이 2발보다 아래일때 여분 총알을 올림
        //    {
        //        spare_ammo++;
        //    }
        //}
        //public override void Reload()//메인에서 계속 돌아갈 함수
        //{
        //    if ( spare_ammo > 0)//인간 상태이며 여분 총알이 1발 이상이라면 재장전 실행
        //    {
        //        if (ControllerScript.Instance._DrawReload(ref ControllerScript.instance.b_reload))//재장전 애니메이션 그리고 해당 애니메이션이 성공적으로 그려졌다면 아래 실행 그리고 _DrawReload함수에서 재장전 중인지 아닌지 판단하는 b_reload변수 컨트롤
        //        {
        //            this.spare_ammo--;
        //            this.ammo++;
        //        }
        //        //if (!UIController.Instance.startCor)
        //        //{
        //        //    UIController.Instance.StartCoroutine(UIController.Instance.DrawReload());
        //        //}
        //        //
        //        ////Debug.Log(UIController.Instance.reloadDoon);
        //        //if (UIController.Instance.reloadDoon)//1안
        //        //{
        //        //    UIController.Instance.StopCoroutine(UIController.Instance.DrawReload());
        //        //    //ControllerScript.Instance._DrawReload();
        //        //    //UIController.Instance._DrawReload();
        //        //    Debug.Log("인간 상태 재장전");
        //        //    spare_ammo--;
        //        //    ammo++;
        //        //    Debug.Log($"남은 장탄수{ammo}");//재장전 쿨타임 필요
        //        //}
        //        //Debug.Log("인간 상태 재장전");//2안
        //        //spare_ammo--;
        //        //ammo++;
        //        //Debug.Log($"남은 장탄수{ammo}");//재장전 쿨타임 필요
        //        //UIController.Instance._DrawReload();//재장전
        //        //UIController.Calltest();
        //    }
        //    //ammo++;
        //
        //}
    }
}

