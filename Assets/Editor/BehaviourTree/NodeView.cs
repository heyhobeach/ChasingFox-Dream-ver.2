using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace BehaviourTree.Editor
{
    public class NodeView : Node
    {
        public Action<NodeView> OnNodeSelected;
        public BehaviourNode behaviourNode;
        public Port input;
        public Port output;
        public NodeView(BehaviourNode node) : base("Assets/Editor/BehaviourTree/NodeView.uxml") 
        {
            behaviourNode = node;
            this.title = node.name;
            viewDataKey = node.guid;

            style.left = node.positon.x;
            style.top = node.positon.y;

            Label descriptionLabel= this.Q<Label>("description");
            descriptionLabel.bindingPath = "description";
            descriptionLabel.Bind(new SerializedObject(node));
        }

        protected virtual void SetupClasses() { }

        protected virtual void CreateInputPorts() // 노드의 포트 설정
        {
            if(input != null)
            {
                input.portName = "";
                inputContainer.Add(input);
            }
        }

        protected virtual void CreateOutputPorts() // 노드의 포트 설정
        {
            if(output != null)
            {
                output.portName = "";
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Undo.RecordObject(behaviourNode, "behaviourTree SetPosition");
            behaviourNode.positon.x = newPos.xMin;
            behaviourNode.positon.y = newPos.yMin;
            EditorUtility.SetDirty(behaviourNode);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if(OnNodeSelected != null) OnNodeSelected.Invoke(this);
        }

        public void SortChildren()
        {
            CompositeNode compositeNode = behaviourNode as CompositeNode;
            if(compositeNode) compositeNode.children.Sort(SortByHorizontalPosition);
        }

        private int SortByHorizontalPosition(BehaviourNode left, BehaviourNode right) => left.positon.x < right.positon.x ? -1 : 1;

        public void UpdateState()
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");
            if(!Application.isPlaying) return;
            switch(behaviourNode.state)
            {
                case BehaviourNode.NodeState.RUNNING:
                if(behaviourNode.isStarted) AddToClassList("running");
                break;
                case BehaviourNode.NodeState.FAILURE:
                AddToClassList("failure");
                break;
                case BehaviourNode.NodeState.SUCCESS:
                AddToClassList("success");
                break;
            }
        }
    }
}
