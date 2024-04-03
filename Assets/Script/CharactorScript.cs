using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Timers;
using TMPro;

public partial class ControllerScript : MonoBehaviour
{
    [SerializeField] GameObject attackPoint;//��������
     public GameObject bullet;//�Ѿ� ������Ʈ ����
    public GameObject meleeAttack;
    public Vector3 worldPosition;
    public class Charactor
    {
        public float speed;
        public bool isHuman = true;
        public Vector3 hidePos;




        public virtual void Die(Charactor charactor)
        {

        }

        public virtual void Setspeed() { }

        public virtual void Attack() { }
        public void Crouch(GameObject guard)//ref bool �� ����
        {

            Debug.Log("ũ���ġ");
            guard.SetActive(true);


        }//����� �ΰ� �Ѵ� ��ũ���� ���ٰ� �����ϱ⿡ �׳� �Ϲ��Լ��� ����

        public virtual void Reload() { }
    }
    public class WereWolf : Charactor
    {
        public int life = 2;
        private static WereWolf instance;
        //IEnumerator tTimer;
        public bool isAttacking = false;

        private bool showAttack = false;

        float t = 0;

        public static WereWolf Instance()
        {
            if (instance == null)
            {
                //Debug.Log("����� ����");
                instance = new WereWolf();
                ControllerScript.instance.StartCoroutine(instance.timer());//�ڷ�ƾ ȣ�� �κ��� ���� ���� �� �ʿ������� ���� ���������� ��̴Ͽ��� ����� StartCorutine�� �׳� �����Լ��� ������ �ϱ��ϴ���

            }
            else
            {
                //Debug.Log("�� ����⶧���� ������ ����");
            }
            return instance;
        }

        private WereWolf()
        {
            //if (instance == null) Debug.Log("����");
        }

        public override void Setspeed()
        {
            Debug.Log("���� �ӵ�");
            this.speed = 5.0f;
        }
        public override void Attack()//�ٽ� �����ϴ°��� ������ �ٴڿ� ���� ���������? �׷��� isground
        {
            Debug.Log("���� ����");
            //float startAttack = Time.time;
            float startAttack =0;
            t = 0;
            ControllerScript.instance.meleeAttack.transform.position = ControllerScript.instance.attackPoint.transform.position;


            //ControllerScript.instance.meleeAttack.SetActive(true);

            //if (!showAttack)
            //{
                if (!isAttacking)
                {
                    ControllerScript.instance.meleeAttack.SetActive(true);
                    Vector2 temp = (ControllerScript.instance.ClickPos() - ControllerScript.instance.transform.position).normalized;
                    temp = temp.normalized;//ª�� �Ÿ��� 1�� �����ֱ� ����
                    Debug.Log(temp);
                    ControllerScript.instance.rg2d.AddForce(temp * 4, ForceMode2D.Impulse);
                    isAttacking = true;
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

            //Debug.Log("��");
            //invoke(eraseMelee,1);//���� invoke�� ����ٸ�

        }
        IEnumerator timer()
        {
            while (true)
            {
                //���� ���� �����ϴ°� ����ϴٸ� �Ʒ� ���� ���ӽð��� ª�� �����μ� �̻����� �ٿ����� �׷��� �̻��ϴٸ� ���� �ʿ�
                if (isAttacking)//if�� ������ ���� ������ ������ Ŭ����ü�� �̺�Ʈ���� if�� , if�� ����� 1.x�ʿ��� Ŭ���ϸ� ��� ���̰� ������
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
                    //���� ������ �����ؼ� ���� �� ���ٸ� �Ʒ� �κ� �׷��� �Ƹ� ���� �κ� �����ؼ� ����°� �ڿ��������
                    Debug.Log("����");
                    yield return new WaitForSeconds(2);
                    
                    ControllerScript.instance.meleeAttack.SetActive(false);
                    //showAttack = false;
                    isAttacking = false;

                }

                yield return null;
            }

        }
        public void SetFalse()
        {
            ControllerScript.instance.meleeAttack.SetActive(false);
        }

        float Getspeed()
        {
            return this.speed;
        }

    }

    public class Human : Charactor
    {
        public int ammo = 2;
        public int spare_ammo = 0;
        private static Human instance;



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
            if (ammo <= 0)
            {
                Debug.Log("��ź�� ����");
                return;
            }
            Debug.Log("��� ���� ����");
            //GameObject _bullet = Instantiate(ControllerScript.instance.bullet, ControllerScript.instance.attackPoint.transform.position, ControllerScript.instance.transform.rotation);
            //_bullet.transform.SetParent(ControllerScript.instance.gameObject.transform);
            Vector3 temp = ControllerScript.instance.ClickPos();
            GameObject _bullet = Instantiate(ControllerScript.instance.bullet, ControllerScript.instance.attackPoint.transform.position, ControllerScript.instance.transform.rotation);

            //var screenPoint = Input.mousePosition;//���콺 ��ġ ������
            //screenPoint.z = Camera.main.transform.position.z;
            //ControllerScript.instance.worldPosition = Camera.main.ScreenToWorldPoint(screenPoint);
            _bullet.GetComponent<BulletScript>().targetPos = temp;
            _bullet.GetComponent<BulletScript>().shootPos = ControllerScript.instance.transform.position;

            //Debug.Log(ControllerScript.instance.worldPosition);
            ammo--;
            Debug.Log($"���� ��ź��{ammo}");

        }
        public override void Setspeed()
        {
            Debug.Log("��� �ӵ�");
            this.speed = 3.0f;
        }
        public void GetAmmo()
        {
            if (ammo < 2)
            {
                spare_ammo++;
            }
        }
        public override void Reload()
        {
            if (isHuman && spare_ammo > 0)
            {
                if (ControllerScript.Instance._DrawReload(ref ControllerScript.instance.b_reload))
                {
                    this.spare_ammo--;
                    this.ammo++;
                }
                //if (!UIController.Instance.startCor)
                //{
                //    UIController.Instance.StartCoroutine(UIController.Instance.DrawReload());
                //}
                //
                ////Debug.Log(UIController.Instance.reloadDoon);
                //if (UIController.Instance.reloadDoon)//1��
                //{
                //    UIController.Instance.StopCoroutine(UIController.Instance.DrawReload());
                //    //ControllerScript.Instance._DrawReload();
                //    //UIController.Instance._DrawReload();
                //    Debug.Log("�ΰ� ���� ������");
                //    spare_ammo--;
                //    ammo++;
                //    Debug.Log($"���� ��ź��{ammo}");//������ ��Ÿ�� �ʿ�
                //}
                //Debug.Log("�ΰ� ���� ������");//2��
                //spare_ammo--;
                //ammo++;
                //Debug.Log($"���� ��ź��{ammo}");//������ ��Ÿ�� �ʿ�
                //UIController.Instance._DrawReload();//������
                //UIController.Calltest();
            }
            //ammo++;

        }
    }
}
