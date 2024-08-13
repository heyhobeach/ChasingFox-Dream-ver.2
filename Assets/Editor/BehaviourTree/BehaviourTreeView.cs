using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using BehaviourTree;
using System.Linq;

namespace BehaviourTree.Editor
{
    public class BehaviourTreeView : GraphView
    {
        public Action<NodeView> OnNodeSelected;
        public BehaviourTree behaviourTree;

        private Vector2 mousePos;

        public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> {}
        public BehaviourTreeView() 
        {
            Insert(0, new GridBackground());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.RegisterCallback<ContextualMenuPopulateEvent>(x => mousePos = Event.current.mousePosition);

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTree/BehaviourTreeEditor.uss");
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            PopulateView(behaviourTree);
            AssetDatabase.SaveAssets();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) => ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();

        internal void PopulateView(BehaviourTree tree) // 저장된 트리 불러오기
        {
            behaviourTree = tree;

            viewTransform.scale = behaviourTree.viewScale;
            viewTransform.position = behaviourTree.viewPos;

            viewTransformChanged -= OnViewTransformChanged;
            viewTransformChanged += OnViewTransformChanged;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if(tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                AssetDatabase.SaveAssets();
            }
            EditorUtility.SetDirty(tree);

            tree.nodes.ForEach(n => CreateNodeView(n));
            tree.nodes.ForEach(n => CreateEdge(n));
        }

        public NodeView FindNodeView(BehaviourNode node) => GetNodeByGuid(node.guid) as NodeView;

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) // 요소 변경 시 실행
        {
            if(graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(e => {
                    var nodeView = e as NodeView;
                    if(nodeView != null) behaviourTree.DeleteNode(nodeView.behaviourNode);
                    var edge = e as Edge;
                    if(edge != null)
                    {
                        var parentView = edge.output.node as NodeView;
                        var childView = edge.input.node as NodeView;
                        behaviourTree.RemoveChild(parentView.behaviourNode, childView.behaviourNode);
                    }
                });

            }

            if(graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge => {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;
                    behaviourTree.AddChild(parentView.behaviourNode, childView.behaviourNode);
                });
            }

            if(graphViewChange.movedElements != null)
            {
                nodes.ForEach((n) => {
                    var nodeView = n as NodeView;
                    nodeView.SortChildren();
                });
            }

            return graphViewChange;
        }

        private void OnViewTransformChanged(GraphView graphView)
        {
            behaviourTree.viewPos = graphView.viewTransform.position;
            behaviourTree.viewScale = graphView.viewTransform.scale;
        }

        private void CreateNodeView(BehaviourNode node)
        {
            NodeView nodeView = new(node);
            RootNode rootNode = node as RootNode;
            if(rootNode) nodeView = new RootNodeView(node);
            ActionNode actionNode = node as ActionNode;
            if(actionNode) nodeView = new ActionNodeView(node);
            CompositeNode compositeNode = node as CompositeNode;
            if(compositeNode) nodeView = new CompositeNodeView(node);
            DecoratorNode decoratorNode = node as DecoratorNode;
            if(decoratorNode) nodeView = new DecoratorNodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        private void CreateEdge(BehaviourNode node)
        {
            var children = behaviourTree.GetChildren(node);
            foreach(var child in children)
            {
                var parentView = FindNodeView(node);
                var childView = FindNodeView(child);

                var edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            // base.BuildContextualMenu(evt);
            evt.menu.AppendSeparator();
            CreateMenu(evt, TypeCache.GetTypesDerivedFrom<ActionNode>());
            CreateMenu(evt, TypeCache.GetTypesDerivedFrom<CompositeNode>());
            CreateMenu(evt, TypeCache.GetTypesDerivedFrom<DecoratorNode>());
            evt.menu.AppendSeparator();
        }

        private void CreateMenu(ContextualMenuPopulateEvent evt, TypeCache.TypeCollection types)
        {
            if(types.Count <= 0) return;
            foreach(var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType.Name.Replace("BehaviourTree.", "")}/{type.Name}", (a) => CreateNode(type));
            }
        }

        private void CreateNode(Type type)
        {
            var node = behaviourTree.CreateNode(type);
            CreateNodeView(node);
            var view = FindNodeView(node);
            var ctPos = contentViewContainer.WorldToLocal(mousePos);
            view.SetPosition(new Rect(ctPos, Vector2.zero));
        }

        public void NodeStateUpdate()
        {
            nodes.ForEach(node => {
                NodeView nodeView = node as NodeView;
                nodeView.UpdateState();
            });
        }
    }
}