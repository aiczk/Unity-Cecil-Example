using System;
using Boo.Lang;
using UnityEngine;

namespace LINQ2Method.Helpers
{
    public interface INode<T>
    {
        void Add(INode<T> node);
        INode<T> Remove(T value);
        INode<T> FindLeafNode(T value);
        INode<T> FindSubNode(T value);

        T Item { get; }
    }

    public class SubNode<T> : INode<T>
    where T : IEquatable<T>
    {
        public T Item { get; }

        private List<INode<T>> components;
        private INode<T> holder;
        
        public SubNode(T value)
        {
            components = new List<INode<T>>();
            holder = this;
            Item = value;
        }

        void INode<T>.Add(INode<T> node) => components.Add(node);

        INode<T> INode<T>.Remove(T value)
        {
            var result = holder.FindLeafNode(value);

            if (result == null)
                return this;
            
            components.Remove(result);
            return holder;
        }

        INode<T> INode<T>.FindLeafNode(T value)
        {
            INode<T> leaf = default;

            foreach (var component in components)
            {
                var result = component.FindLeafNode(value);
                
                if (IsPossibleCast<LeafNode<T>>(result))
                    continue;
                
                if (!Equal(result, value))
                    continue;
                
                leaf = result;
                break;
            }

            return leaf;
        }

        INode<T> INode<T>.FindSubNode(T value)
        {
            INode<T> leaf = default;
            
            foreach (var component in components)
            {
                if(!IsPossibleCast<LeafNode<T>>(component))
                    continue;

                if (Equal(component, value))
                    continue;

                leaf = component;
                break;
            }

            return leaf;
        }
        
        private static bool Equal(in INode<T> node, in T value) => node.Item.Equals(value);
        private static bool IsPossibleCast<TV>(INode<T> type) where TV : class, INode<T> => (type as TV) == null;
    }
    
    public class LeafNode<T> : INode<T>
    where T : IEquatable<T>
    {
        public T Item { get; }
        
        public LeafNode(T item)
        {
            Item = item;
        }

        void INode<T>.Add(INode<T> node){}
        INode<T> INode<T>.Remove(T value) => this;
        INode<T> INode<T>.FindLeafNode(T value) => this;
        INode<T> INode<T>.FindSubNode(T value) => null;
    }

}
