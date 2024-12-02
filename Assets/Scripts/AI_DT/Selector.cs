using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
<<<<<<< HEAD
    // inherits Node
    public class Selector : Node
    {
        public Selector() : base() { } // standard constructor
        public Selector(List<Node> children) : base(children) { } // constructor w/ children


        public override NodeState Evaluate()
        {
            //check each child for running/success, return failure if not
=======
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
>>>>>>> origin/main
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }
<<<<<<< HEAD
            // if any child state is running/success, sets and returns current state as running/success
=======
>>>>>>> origin/main
            state = NodeState.FAILURE;
            return state;
        }
    }
}

