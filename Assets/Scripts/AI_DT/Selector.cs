using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    // inherits Node
    public class Selector : Node
    {
        public Selector() : base() { } // standard constructor
        public Selector(List<Node> children) : base(children) { } // constructor w/ children


        public override NodeState Evaluate()
        {
            //check each child for running/success, return failure if not


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
            // if any child state is running/success, sets and returns current state as running/success
            state = NodeState.FAILURE;
            return state;
        }
    }
}

