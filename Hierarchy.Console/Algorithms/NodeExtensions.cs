using System;
using System.Collections.Generic;
using System.Linq;

namespace Hierarchy.Console.Algorithms
{
    public static class NodeExtensions
    {
        public static List<Node<TNode>> GetNodes<TNode, TNodeIdentifier>(this IEnumerable<TNode> items,
            Func<TNode, TNodeIdentifier> nodeIdentifier,
            Func<TNode, TNodeIdentifier> parentNodeIdentifier) where TNode : class where TNodeIdentifier : struct
        {
            if (items == null || nodeIdentifier == null || parentNodeIdentifier == null)
            {
                return new List<Node<TNode>>();
            }

            var collection = items as List<TNode> ?? new List<TNode>();
            var rootNodes = new Dictionary<TNodeIdentifier, Node<TNode>>();
            var visitedNodes = new Dictionary<TNodeIdentifier, Node<TNode>>();

            collection.ForEach(item =>
            {
                var id = nodeIdentifier(item);
                var parentId = parentNodeIdentifier(item);

                if (id.Equals(parentId))
                {
                    SetVisitedNode(visitedNodes, id, item);
                    SetRootNode(rootNodes, id, visitedNodes[id]);
                }
                else
                {
                    SetVisitedNode(visitedNodes, id, item);
                    AddSubNode(visitedNodes, parentId, id);
                }
            });


            return rootNodes.Select(x => x.Value).ToList();
        }

        private static void AddSubNode<TNode, TNodeIdentifier>(Dictionary<TNodeIdentifier, Node<TNode>> visitedNodes, TNodeIdentifier parentId, TNodeIdentifier id)
            where TNode : class where TNodeIdentifier : struct
        {
            if (visitedNodes == null)
            {
                return;
            }

            if (visitedNodes.ContainsKey(parentId))
            {
                visitedNodes[parentId].AddSubNode(visitedNodes[id]);
            }
            else
            {
                var parentItem = new Node<TNode>(null);
                parentItem.AddSubNode(visitedNodes[id]);
                visitedNodes.Add(parentId, parentItem);
            }
        }

        private static void SetRootNode<TNode, TNodeIdentifier>(Dictionary<TNodeIdentifier, Node<TNode>> rootNodes, TNodeIdentifier id, Node<TNode> node) where TNode : class where TNodeIdentifier : struct
        {
            if (rootNodes == null || node == null)
            {
                return;
            }

            if (rootNodes.ContainsKey(id))
            {
                rootNodes[id] = node;
            }
            else
            {
                rootNodes.Add(id, node);
            }
        }

        private static void SetVisitedNode<TNode, TNodeIdentifier>(Dictionary<TNodeIdentifier, Node<TNode>> dictionary, TNodeIdentifier key, TNode node) where TNode : class where TNodeIdentifier : struct
        {
            if (dictionary == null || node == null)
            {
                return;
            }

            if (dictionary.ContainsKey(key))
            {
                dictionary[key].SetNode(node);
            }
            else
            {
                dictionary.Add(key, new Node<TNode>(node));
            }
        }
    }
}