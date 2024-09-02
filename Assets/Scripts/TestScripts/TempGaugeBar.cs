using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempGaugeBar : MonoBehaviour
{
    public Image image;
    public Player player;

    // Update is called once per frame
    void Update() => image.fillAmount = player.changeGage / player.brutalData.maxGage;
}
