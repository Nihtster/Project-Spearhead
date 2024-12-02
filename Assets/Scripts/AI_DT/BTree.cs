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
<<<<<<< HEAD
            // @TODO per unit
=======
            // @TODO per root action
>>>>>>> origin/main
            //_root = SetupTree();
        }

        private void Update()
        {
            if (_root != null)
<<<<<<< HEAD
                _root.Evaluate();
        }
        // @TODO per unit
=======
            {
                _root.Evaluate();
            }
        }
        // @TODO per root action
>>>>>>> origin/main
        //protected abstract Node SetupTree();
    }
    
}

