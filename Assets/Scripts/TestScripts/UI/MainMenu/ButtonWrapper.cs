using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWrapper : MonoBehaviour
{
    public Action onMouseEnter;
    [HideInInspector] public Button button;

    private void OnMouseEnter() => onMouseEnter?.Invoke();

    private void Awake() => GetComponent<Button>();
}
