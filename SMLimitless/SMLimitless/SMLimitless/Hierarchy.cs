//-----------------------------------------------------------------------
// <copyright file="Hierarchy.cs" company="The Limitless Development Team">
//     Copyrighted under the MIT license.
// </copyright>
//-----------------------------------------------------------------------
namespace SMLimitless
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides a generic hierarchy collection.
    /// Please note that this class itself represents a node
    /// in a hierarchy, and contains a collection of child nodes.
    /// </summary>
    /// <typeparam name="T">Any class.</typeparam>
    public class Hierarchy<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Hierarchy{T}"/> class.
        /// </summary>
        /// <param name="data">The data of this node.</param>
        public Hierarchy(T data)
        {
            this.Children = new List<Hierarchy<T>>();
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets the data contained within this node.
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets the next level up in the hierarchy.
        /// Use .Add() to assign a parent.
        /// </summary>
        public Hierarchy<T> Parent { get; private set; }

        /// <summary>
        /// Gets a collection of the children of this node in the hierarchy.
        /// </summary>
        public List<Hierarchy<T>> Children { get; private set; } 

        /// <summary>
        /// Adds a new node to this hierarchy node as a child node.
        /// </summary>
        /// <param name="node">The node, optionally containing some data, to add to this node.</param>
        public void Add(Hierarchy<T> node)
        {
            if (node.Parent != this && node.Parent != null)
            {
                node.Parent.Children.Remove(this);
            }

            this.Children.Add(node);
            node.Parent = this;
        }

        /// <summary>
        /// Adds some data to a node, and adds that node to this node.
        /// </summary>
        /// <param name="data">The data to add to this node.</param>
        /// <returns>A new <see cref="Hierarchy"/> instance containing the data.</returns>
        public Hierarchy<T> Add(T data)
        {
            var node = new Hierarchy<T>(data);

            this.Add(node);

            return node;
        }

        /// <summary>
        /// Removes a specified node from this hierarchy node.
        /// This method is recursive, and will become slower with larger hierarchies.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        /// <exception cref="Exception">Thrown in the node is not contained within this hierarchy node.</exception>
        public void Remove(Hierarchy<T> node)
        {
            if (!this.Children.Contains(node))
            {
                throw new Exception("Hierarchy<T>.Remove: Node not found.");
            }

            this.Children.Remove(node);
        }

        /// <summary>
        /// Calculates the zero-based depth of this node in the hierarchy.
        /// This method is recursive, and will become slower with larger hierarchies.
        /// </summary>
        /// <returns>The zero-based depth of this node in the hierarchy.</returns>
        public int GetDepth()
        {
            if (this.Parent == null)
            {
                return 0;
            }
            else
            {
                return 1 + this.Parent.GetDepth();
            }
        }

        /// <summary>
        /// Searches a hierarchy from this node down.
        /// This method is recursive, and will become slower with larger hierarchies.
        /// </summary>
        /// <param name="data">The data to search for.</param>
        /// <returns>The node containing the data, or null if there is no match.</returns>
        public Hierarchy<T> Search(T data)
        {
            if (this.Data == data)
            {
                return this;
            }

            foreach (Hierarchy<T> node in this.Children)
            {
                return node.Search(data);
            }

            return null;
        }
    }
}
