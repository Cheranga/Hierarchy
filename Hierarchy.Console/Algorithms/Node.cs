﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Hierarchy.Console.Algorithms
{
    [DebuggerDisplay("{" + nameof(DisplayName) + "}")]
    public class Node<TNode> where TNode : class
    {
        private readonly List<Node<TNode>> _subNodes;

        public Node(TNode node)
        {
            _data = node;
            _subNodes = new List<Node<TNode>>();
        }

        private TNode _data;
        public TNode Data => _data;

        public IReadOnlyList<Node<TNode>> SubNodes => _subNodes.AsReadOnly();

        public void AddSubNode(Node<TNode> dependent)
        {
            if (dependent != null)
            {
                _subNodes.Add(dependent);
            }
        }

        public Node<TNode> SetNode(TNode item)
        {
            _data = item;

            return this;
        }

        public string DisplayName => _data == null ? "" : _data.ToString();
    }
}