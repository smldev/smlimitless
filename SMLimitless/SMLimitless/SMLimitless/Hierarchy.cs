using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless
{
    /// <summary>
    /// provides a generic hierarchy collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Hierarchy<T> where T : class
    {
        #region fields and properties

        public T Data { get; set; }

        /// <summary>
        /// Next level up in the hierarchy. Use .Add() to assign a parent.
        /// </summary>
        public Hierarchy<T> Parent { get; private set; }

        /// <summary>
        /// Children of this node in the hierarchy
        /// </summary>
        public List<Hierarchy<T>> Children { get; private set; } 

        #endregion

        #region initialization

        public Hierarchy(T data) 
        {
            Children = new List<Hierarchy<T>>();
            Data = data;
        }

        #endregion

        #region methods

        public void Add(Hierarchy<T> node)
        {
            if (node.Parent != this && node.Parent != null)
            {
                node.Parent.Children.Remove(this);
            }
            Children.Add(node);
            node.Parent = this;
        }

        public Hierarchy<T> Add(T data)
        {
            var node = new Hierarchy<T>(data);

            Add(node);

            return node;
        }

        public void Remove(Hierarchy<T> node)
        {
            if (!Children.Contains(node))
                throw new Exception("Hierarchy<T>.Remove: Node not found.");

            Children.Remove(node);
        }

        /// <summary>
        /// calculate the zero-based depth of this node in the hierarchy
        /// </summary>
        /// <returns></returns>
        public int GetDepth()
        {
            if (Parent == null)
                return 0;
            else
                return 1 + Parent.GetDepth();
        }

        public Hierarchy<T> Search(T data)
        {
            if (Data == data) return this;

            foreach (Hierarchy<T> node in Children)
            {
                return node.Search(data);
            }

            return null;
        }

        #endregion
    }
}
