using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD

namespace BehaviorTree
{
    // inherits Node
    public class Sequence : Node
    {
        public Sequence() : base() { }  //standard constructor
        public Sequence(List<Node> children) : base(children) { } // constructor w/ list of children
        public override NodeState Evaluate()
        {
            // intialize bool for running children
            bool anyChildRunning = false;

            //check each child for running state
=======
namespace BehaviorTree
{
    public class Sequence : Node
    {
        public Sequence() : base() { }
        public Sequence(List<Node> children) : base(children) { }
        public override NodeState Evaluate()
        {
            bool anyChildRunning = false;

>>>>>>> origin/main
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
<<<<<<< HEAD
=======
                        
>>>>>>> origin/main
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
<<<<<<< HEAD
            // parent will return running state if any children are running
=======
>>>>>>> origin/main
            state = anyChildRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}