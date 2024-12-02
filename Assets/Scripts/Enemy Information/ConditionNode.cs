using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionNode : DecisionNode
{
    private System.Func<bool> condition;
    private DecisionNode trueNode;
    private DecisionNode falseNode;

    public ConditionNode(System.Func<bool> condition, DecisionNode trueNode, DecisionNode falseNode)
    {
        this.condition = condition;
        this.trueNode = trueNode;
        this.falseNode = falseNode;
    }

    public override DecisionNode Evaluate()
    {
        return condition() ? trueNode : falseNode;
    }
}