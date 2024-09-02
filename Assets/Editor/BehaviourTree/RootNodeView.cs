using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTree.Editor
{
    public class RootNodeView : NodeView
    {
        public RootNodeView(BehaviourNode node) : base(node)
        {
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

        protected override void SetupClasses() => AddToClassList("root");

        protected override void CreateInputPorts() => base.CreateInputPorts();

        protected override void CreateOutputPorts()
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            base.CreateOutputPorts();
        }
    }
}
