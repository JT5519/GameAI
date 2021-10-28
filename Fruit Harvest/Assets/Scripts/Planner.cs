using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//planner to build agent plan
public class Planner
{
    public Queue<Actions> buildActionQueue(List<Actions> possibleActions, WorldState currentState, WorldState desiredState)
    {
        Queue<Actions> actionQueue = new Queue<Actions>(); //initialize action queue to be returned

        List<Node> leaves = new List<Node>(); //leaf nodes to remember all possible actions sequences that give the desired world state
        Node start = new Node(null, 0, currentState, null); //root node 
        bool success = buildAGraph(start, leaves, possibleActions, desiredState); //begin building
        if (!success) //if no successful path found, return empty actionq queue
            return actionQueue;

        Node cheapest = null; //finding cheapest solution path
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

        //creating list of ordered actions
        List<Actions> actionList = new List<Actions>();
        while(cheapest!=null)
        {
            if (cheapest.action != null)
                actionList.Insert(0, cheapest.action);
            cheapest = cheapest.parent;
        }

        //adding actions to the action queue
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
            if(action.isActionPossible(parent.state)) //if action can be done now, do it 
            {
                //new state and new node made after action is commited
                WorldState currentState = new WorldState(parent.state);
                action.doAction(currentState);
                Node node = new Node(parent, parent.runningCost + action.actionCost, currentState, action);
                if(currentState.Equals(desiredState)) //if state is desired then add node as leaf node
                {
                    leaves.Add(node);
                    foundAPath = true;
                }
                else //not reached desired state yet, keep going
                {
                    //remove commited action from possible actions
                    List<Actions> possibleActionSubset = new List<Actions>(possibleActions);
                    possibleActionSubset.Remove(action);
                    foundAPath = buildAGraph(node, leaves, possibleActionSubset, desiredState); 
                }
            }
        }
        return foundAPath;
    }
    //class for Nodes of the action graph
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
