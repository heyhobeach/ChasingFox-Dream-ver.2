using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damageables
{
    [RequireComponent(typeof(Animator))]
    public class RunAnimation : MonoBehaviour, IDamageable
    {
        public bool invalidation { get; set; }
        public int _maxHealth;
        public int maxHealth { get => _maxHealth; set => _maxHealth = value; }
        public int health { get; set; }
        private Animator animator;

        void OnEnable()
        {
            health = _maxHealth;
            animator = GetComponent<Animator>();
        }

        public void Death() => animator.SetTrigger("On");
    }
}
