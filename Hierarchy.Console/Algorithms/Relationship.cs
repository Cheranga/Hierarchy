using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hierarchy.Console.Algorithms
{
    public class Nodes<TNode> where TNode : class
    {
        private readonly List<Nodes<TNode>> _subNodes;

        public Nodes(TNode node)
        {
            _item = node;
            _subNodes = new List<Nodes<TNode>>();
        }

        private TNode _item;
        public TNode Item => _item;

        public IReadOnlyList<Nodes<TNode>> SubNodes => _subNodes.AsReadOnly();

        public void AddSubNode(Nodes<TNode> dependent)
        {
            if (dependent != null)
            {
                _subNodes.Add(dependent);
            }
        }

        public Nodes<TNode> SetNode(TNode item)
        {
            _item = item;

            return this;
        }
    }

    public static class EnumerableExtensions
    {
        public static List<Nodes<TItem>> GetRelationships<TItem, TItemIdentifier>(this IEnumerable<TItem> items,
            Func<TItem, TItemIdentifier> nodeIdentifier,
            Func<TItem, TItemIdentifier> parentNodeIdentifier) where TItem : class where TItemIdentifier : struct
        {
            if (items == null || nodeIdentifier == null || parentNodeIdentifier == null)
            {
                return new List<Nodes<TItem>>();
            }

            var collection = items as List<TItem> ?? new List<TItem>();
            var rootNodes = new Dictionary<TItemIdentifier, Nodes<TItem>>();
            var visitedNodes = new Dictionary<TItemIdentifier, Nodes<TItem>>();

            collection.ForEach(item =>
            {
                var id = nodeIdentifier(item);
                var parentId = parentNodeIdentifier(item);

                if (id.Equals(parentId))
                {
                    if (visitedNodes.ContainsKey(id))
                    {
                        visitedNodes[id].SetNode(item);
                    }
                    else
                    {
                        visitedNodes.Add(id, new Nodes<TItem>(item));
                    }

                    if (rootNodes.ContainsKey(id))
                    {
                        rootNodes[id] = visitedNodes[id];
                    }
                    else
                    {
                        rootNodes.Add(id, visitedNodes[id]);
                    }
                }
                else
                {
                    if (visitedNodes.ContainsKey(id))
                    {
                        visitedNodes[id].SetNode(item);
                    }
                    else
                    {
                        visitedNodes.Add(id, new Nodes<TItem>(item));
                    }

                    if (visitedNodes.ContainsKey(parentId))
                    {
                        visitedNodes[parentId].AddSubNode(visitedNodes[id]);
                    }
                    else
                    {
                        var parentItem = new Nodes<TItem>(null);
                        parentItem.AddSubNode(visitedNodes[id]);
                        visitedNodes.Add(parentId, parentItem);
                    }
                }
            });


            return rootNodes.Select(x => x.Value).ToList();
        }
    }
}