using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : DecisionNode
{
    private System.Action action;

    public ActionNode(System.Action action)
    {
        this.action = action;
    }

    public override DecisionNode Evaluate()
    {
        action?.Invoke();
        return null; // Action nodes terminate the evaluation chain
    }
}
