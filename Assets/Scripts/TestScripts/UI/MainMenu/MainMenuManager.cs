using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;
    public static MainMenuManager Instance { get; private set; }

    public Vector2 screanSize;

    void Start()
    {
        if(instance != null) Destroy(gameObject);
        instance = this;

        screanSize = new(Screen.width, Screen.height);
    }

    void OnDestroy() => instance = null;

    void Update()
    {
        
    }
}
