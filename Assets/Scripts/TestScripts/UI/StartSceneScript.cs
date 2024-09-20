using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneScript : MonoBehaviour
{
    public GameObject go;

    void Start() => go.SetActive(true);
}
