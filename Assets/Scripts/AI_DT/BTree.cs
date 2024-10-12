using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class BTree : MonoBehaviour
    {
        private Node _root = null;
        protected void Start()
        {
            // @TODO per unit
            //_root = SetupTree();
        }

        private void Update()
        {
            if (_root != null)
                _root.Evaluate();
        }
        // @TODO per unit
        //protected abstract Node SetupTree();
    }
    
}

