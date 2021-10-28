using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planner
{
    public Queue<Actions> buildActionQueue(List<Actions> possibleActions, WorldState currentState, WorldState desiredState)
    {
        Queue<Actions> actionQueue = new Queue<Actions>();

        List<Node> leaves = new List<Node>();
        Node start = new Node(null, 0, currentState, null);
        bool success = buildAGraph(start, leaves, possibleActions, desiredState);
        if (!success)
            return actionQueue;

        Node cheapest = null;
        foreach(Node leaf in leaves)
        {
            if (cheapest == null)
                cheapest = leaf;
            else
            {
                if (leaf.runningCost < cheapest.runningCost)
                    cheapest = leaf;
            }
        }

        List<Actions> actionList = new List<Actions>();
        while(cheapest!=null)
        {
            if (cheapest.action != null)
                actionList.Insert(0, cheapest.action);
            cheapest = cheapest.parent;
        }

        foreach(Actions action in actionList)
        {
            actionQueue.Enqueue(action);
        }
        return actionQueue;
    }
    bool buildAGraph(Node parent, List<Node> leaves,List<Actions> possibleActions, WorldState desiredState)
    {
        bool foundAPath = false;
        foreach(Actions action in possibleActions)
        {
            if(action.isActionPossible(parent.state))
            {
                WorldState currentState = new WorldState(parent.state);
                action.doAction(currentState);
                Node node = new Node(parent, parent.runningCost + action.actionCost, currentState, action);
                if(currentState.Equals(desiredState))
                {
                    leaves.Add(node);
                    foundAPath = true;
                }
                else
                {
                    List<Actions> possibleActionSubset = new List<Actions>(possibleActions);
                    possibleActionSubset.Remove(action);
                    foundAPath = buildAGraph(node, leaves, possibleActionSubset, desiredState);
                }
            }
        }
        return foundAPath;
    }
    private class Node
    {
        public Node parent;
        public float runningCost;
        public WorldState state;
        public Actions action;

        public Node(Node p, float r, WorldState s, Actions a)
        {
            this.parent = p;
            this.runningCost = r;
            this.state = s;
            this.action = a;
        }
    }
}
