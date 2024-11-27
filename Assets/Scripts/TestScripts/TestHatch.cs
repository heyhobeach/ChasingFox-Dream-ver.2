using System.Collections;
using System.Collections.Generic;
using Damageables;
using UnityEngine;

public class TestHatch : MonoBehaviour, IDamageable
{
    public int _maxHealth;
    public bool invalidation { get; set; }
    public int maxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int health { get; set; }

    public GameObject off;
    public GameObject on;
    public PlayParticle playParticle;

    public void Death()
    {
        off.SetActive(false);
        on.SetActive(true);
        playParticle.Play();
    }

    void Start()
    {
        off.SetActive(true);
        on.SetActive(false);
        health = maxHealth;
    }
    public void DeathFeedBack(Vector2 dir)
    {
        //throw new System.NotImplementedException();
    }
}
