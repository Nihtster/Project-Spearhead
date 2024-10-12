using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree 
{ 
    //enum for node state
    public enum NodeState
    {
        RUNNING,
        FAILURE,
        SUCCESS
    }
    //actual class
    public class Node
    {
        protected NodeState state;  //state container
        public Node parent;         //tree structure
        protected List<Node> children;  // child nodes, tree structure

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>(); //easy way to navigate/store node data for tree creation per unit
        //base constructor
        public Node()
        {
            parent = null;
        }
        //base constructor for existing children
        public Node(List<Node> children)
        {
            foreach(Node child in children)
            {
                _Attach(child);
            }
        }
        // adds node to children list
        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(node);
        }
        // base method for evaluate for the sake of overrides per node/check/action type
        public virtual NodeState Evaluate() => NodeState.FAILURE;
        //sets node data, shared data in dictionary
        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }
        //recursive, allows to either find data we were looking for by going up branch until found
        //or stopping if we reach the root of the tree.
        public object GetData(string key)
        {
            // if node exists, get data
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;
            //
            Node node = parent;
            //moving up the tree
            while(node != null)
            {
                value = node.GetData(key);
                if(value != null) 
                    return value;
                node = node.parent;
            }
            //not found and has reached root
            return null;
        }
        // same as get data, move up until found or null return
        public bool ClearData(string key)
        {
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }
            Node node = parent;
            while(node != null)
            {
                bool cleared = node.ClearData(key);
                if(cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }

    
}