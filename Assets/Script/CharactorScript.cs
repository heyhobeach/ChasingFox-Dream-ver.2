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
    public GameObject meleeAttack;//�����ϴ� ������Ʈ ����
    
    public class Charactor
    {
        public float speed;
        public bool isHuman = true;
        public Vector3 hidePos;
        public int damage = 0;




        public virtual void Die(Charactor charactor)
        {

        }

        public void SetInfo()//�ش� �Լ��� �� ü������ ���ϴ� �����̽� ���� �Լ��� ��Ƽ� �����ų����
        {
            Setspeed();
            Setdamage();
        }

        public virtual void Setspeed() { }
        public virtual void Setdamage() { }

        public virtual void Attack() { }
        public void Crouch(GameObject guard)//ref bool �� ����
        {

            Debug.Log("ũ���ġ");
            guard.SetActive(true);//ũ���ġ �ൿ�� ���� ������Ʈ�� Ȱ��ȭ ��Ŵ


        }//����� �ΰ� �Ѵ� ��ũ���� ���ٰ� �����ϱ⿡ �׳� �Ϲ��Լ��� ����

        public virtual void Reload() { }
    }
    public class WereWolf : Charactor
    {
        public int life = 2;//������ ������ 2�� ���������� �����δ� 1�̶� ���߿� ���� �ʿ���
        private static WereWolf instance;//�̱����� ���� ����
        //IEnumerator tTimer;
        public bool isAttacking = false;//���� �����ְ� �ϴ� �� ���� ���ݽð� �� üũ�Ҷ� ���Ǵ���

        //private bool showAttack = false;//�ش� ������ ��� �� �ؼ� �ּ�ó�� ��

        float t = 0;

        public static WereWolf Instance()//���� �ν���Ʈ�� ���� ��ü�ϱ� ���� ������ ����� �ΰ��� �ѹ������Ǹ� �� ������ �ȵɰ��̶�� ��������
        {
            if (instance == null)
            {
                //Debug.Log("����� ����");
                instance = new WereWolf();
                //���� ������ ���� �׳� �ڷ�ƾ�� ���۽����� �׷����� ���� ���ݻ��¿� ���� isAttacking�� ���� �ڷ�ƾ ���� �� �����? �� ������
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
        public override void Setdamage()
        {
            Debug.Log("���� ���ݷ�");
            this.damage = 2 ;
        }
        public override void Attack()//�ٽ� �����ϴ°Ϳ� ���� ������ �ָ����� �ϴ��� ���� ��������� �����ϱ�� �ߴµ� ������ ���� �����ϱ����
        {
            Debug.Log("���� ����");
            //float startAttack = Time.time;
            float startAttack =0;
            t = 0;
            ControllerScript.instance.meleeAttack.transform.position = ControllerScript.instance.attackPoint.transform.position;//meleeAttack������Ʈ�� ��ġ�� ó�� ������ attackPoint�� �̵���Ŵ


            //ControllerScript.instance.meleeAttack.SetActive(true);

            //if (!showAttack)
            //{
                if (!isAttacking)//������ ���� ������ �� ù �����ϰ��
                {
                    ControllerScript.instance.meleeAttack.SetActive(true);
                    Vector2 temp = (ControllerScript.instance.ClickPos() - ControllerScript.instance.transform.position).normalized;
                    temp = temp.normalized;//ª�� �Ÿ��� 1�� �����ֱ� ����
                    Debug.Log(temp);
                    // ControllerScript.instance.rg2d.AddForce(temp * 4, ForceMode2D.Impulse);//���ݽ� ���ư��°�? �̵��� ����, ������ �������� ��������⶧���� y�൵ ���� �־��� ���� �׽�Ʈ ������ ������ ���̾ ���� �̵��� ����� addForce�� �ʱ�ȭ�� �� �Ǿ��־ �׷��� ����
                    isAttacking = true;//(�������̶�� �Ǵ�)
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
        public int sign;
        IEnumerator timer()
        {
            while (true)
            {
                //���� ���� �����ϴ°� ����ϴٸ� �Ʒ� ���� ���ӽð��� ª�� �����μ� �̻����� �ٿ����� �׷��� �̻��ϴٸ� ���� �ʿ�
                if (isAttacking)//if�� ������ ���� ������ ������ Ŭ����ü�� �̺�Ʈ���� if�� , if�� ����� 1.x�ʿ��� Ŭ���ϸ� ��� ���̰� ������(�ش� ���̵��� ���������� �ڷ�ƾ�� ��� �����̱⿡ ��Ÿ���� ��Ȯ�ϰ� �����̷��� if�� �ʿ�����)
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
                    yield return new WaitForSeconds(0.5f);//0.5�� ���� ���� ������Ʈ�� �� ���̰� ��
                    
                    ControllerScript.instance.meleeAttack.SetActive(false);
                    //showAttack = false;
                    isAttacking = false;//���� ����� �Ǵ�

                }

                yield return null;
            }

        }
        public void SetFalse()//�̰Ŵ� �� ����� �ɵ� ������ �׳� ������ �� ó���ؼ� �װ� ��ü�ϰ��� �Ҷ� �ʿ���
        {
            ControllerScript.instance.meleeAttack.SetActive(false);
        }

        float Getspeed()//Ȥ�ó� �̵��ӵ� ������찡 ������� ���� �κ� ���� �������¾���
        {
            return this.speed;
        }

    }

    public class Human : Charactor
    {
        public int ammo = 2;
        public int spare_ammo = 0;//���� źȯ�� ����� ����
        private static Human instance;//���� ����� ���� ����



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
            if (ammo <= 0)//�Ѿ��� ������ ��� �Ұ�
            {
                Debug.Log("��ź�� ����");
                return;
            }
            Debug.Log("��� ���� ����");
            Vector3 temp = ControllerScript.instance.ClickPos();//Ŭ����ġ�� ��� ����
            GameObject _bullet = Instantiate(ControllerScript.instance.bullet, ControllerScript.instance.attackPoint.transform.position, ControllerScript.instance.transform.rotation);//�Ѿ��� ���������ǿ��� ������
            _bullet.GetComponent<BulletScript>().targetPos = temp;//Ŭ���� ��ġ�� Ÿ�� ��ġ��
            _bullet.GetComponent<BulletScript>().shootPos = ControllerScript.instance.transform.position;//���߿� �Ÿ����� �Ѿ��� �����Ҷ� �ʿ��Ѱ� ���Ƽ� �������, ������ �̵� ���⼳���� ���� �ʿ��� �κ�

            //Debug.Log(ControllerScript.instance.worldPosition);
            ammo--;//����� �̷�����ٸ� �Ѿ��� ����
            Debug.Log($"���� ��ź��{ammo}");

        }
        public override void Setspeed()
        {
            Debug.Log("��� �ӵ�");
            this.speed = 3.0f;
        }
        public override void Setdamage()
        {
            Debug.Log("��� ���ݷ�");
            this.damage = 2;
        }
        public void GetAmmo()//�ش��Լ��� �ϴ� ������ Human.Instance().GetAmmo�� �Ǿ������� ���߿� �����Լ��� ���� ���뿡���� ���� �����ϵ��� �ص� ������ ���̶�� ������ ������
        {
            if (ammo < 2)//�Ѿ��� 2�ߺ��� �Ʒ��϶� ���� �Ѿ��� �ø�
            {
                spare_ammo++;
            }
        }
        public override void Reload()//���ο��� ��� ���ư� �Լ�
        {
            if (isHuman && spare_ammo > 0)//�ΰ� �����̸� ���� �Ѿ��� 1�� �̻��̶�� ������ ����
            {
                if (ControllerScript.Instance._DrawReload(ref ControllerScript.instance.b_reload))//������ �ִϸ��̼� �׸��� �ش� �ִϸ��̼��� ���������� �׷����ٸ� �Ʒ� ���� �׸��� _DrawReload�Լ����� ������ ������ �ƴ��� �Ǵ��ϴ� b_reload���� ��Ʈ��
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
