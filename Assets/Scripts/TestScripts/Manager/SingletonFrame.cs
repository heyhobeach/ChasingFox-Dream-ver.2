using UnityEngine;

public class SingletonFrame<T> : MonoBehaviour where T :SingletonFrame<T>//두개의 타입이 동일한지 판단함
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static T instance;
     protected void Awake()
    {
        if(instance == null)
        {
            instance = (T)this;
        }
    }
}
