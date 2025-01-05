using UnityEngine;

public class SingletonFrame<T> : MonoBehaviour where T :SingletonFrame<T>
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
