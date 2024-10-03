using System.Collections;
using System.Collections.Generic;
using BehaviourTree;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviourTree.Editor
{
    public class DecoratorNodeView : NodeView
    {
        public DecoratorNodeView(BehaviourNode node) : base(node)
        {
            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
        }

        protected override void SetupClasses() => AddToClassList("decorator");

        protected override void CreateInputPorts()
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            base.CreateInputPorts();
        }

        protected override void CreateOutputPorts()
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            base.CreateOutputPorts();
        }
    }
}