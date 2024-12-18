using System.Collections.Generic;
using UnityEngine;

namespace MainMenuUI
{
    public class WindowNode : MonoBehaviour
    {
        [SerializeField] private Vector3 centerPosition;

        private void Awake() => centerPosition = new Vector3(transform.position.x, transform.position.y, -10);

        public Vector3 GetCenterPosition() => centerPosition;
    }
}