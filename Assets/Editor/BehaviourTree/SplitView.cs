using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits> {}
        public SplitView() {}
    }
}