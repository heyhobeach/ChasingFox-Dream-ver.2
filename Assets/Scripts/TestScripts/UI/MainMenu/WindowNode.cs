using System.Collections.Generic;
using UnityEngine;

namespace MainMenuUI
{
    public class WindowNode : MonoBehaviour
    {
        [SerializeField] private Vector2 centerPosition;
        private Vector2 size;
        [SerializeField] private List<WindowNode> nextNodes;

        private void OnEnable()
        {
            // size = MainMenuManager.Instance.screanSize;
        }
    }
}