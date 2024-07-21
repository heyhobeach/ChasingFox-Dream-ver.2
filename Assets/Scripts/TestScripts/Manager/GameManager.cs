using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SoundManager soundManager;

    private static GameManager instance;
    public static GameManager Instance { get => instance; }

    public ProCamera2DShake proCamera2DShake;
    public ProCamera2DPointerInfluence proCamera2DPointerInfluence;

    void Awake()
    {
        if(instance == null) instance = this;
        proCamera2DShake = Camera.main.GetComponent<ProCamera2DShake>();
        proCamera2DPointerInfluence = Camera.main.GetComponent<ProCamera2DPointerInfluence>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
