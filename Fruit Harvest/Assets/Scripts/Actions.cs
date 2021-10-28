using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions
{
    public WorldState actionPreCondition;
    public WorldState actionEffect;
    public int actionCost;
    public bool requiresInRange;
    public GameObject target;

    public void doAction(WorldState state)
    {
        if(actionEffect.flowerState!=WorldState.FlowerBushel.ignore)
            state.flowerState = actionEffect.flowerState;
        if (actionEffect.treeState != WorldState.TreeGrove.ignore)
            state.treeState = actionEffect.treeState;
        if (actionEffect.statueState != WorldState.StatueRegion.ignore)
            state.statueState = actionEffect.statueState;
        if (actionEffect.chestState != WorldState.ResourceChest.ignore)
            state.chestState = actionEffect.chestState;
    }
    public bool isActionPossible(WorldState state)
    {
        if (actionPreCondition.flowerState != WorldState.FlowerBushel.ignore && actionPreCondition.flowerState != state.flowerState)
            return false;
        else if (actionPreCondition.treeState != WorldState.TreeGrove.ignore && actionPreCondition.treeState != state.treeState)
            return false;
        else if (actionPreCondition.statueState != WorldState.StatueRegion.ignore && actionPreCondition.statueState != state.statueState)
            return false;
        else if (actionPreCondition.chestState != WorldState.ResourceChest.ignore && actionPreCondition.chestState != state.chestState)
            return false;
        return true;
    }
    public bool isInRange(GameObject agentPosition)
    {
        if (!requiresInRange)
            return true;
        if (Vector2.Distance(target.transform.position, agentPosition.transform.position) > 1.0f)
            return false;
        return true;
    }

    public Actions(WorldState preC, WorldState actE, int actionC, bool needsR, GameObject targ = null)
    {
        this.actionPreCondition = preC;
        this.actionEffect = actE;
        this.actionCost = actionC;
        this.requiresInRange = needsR;
        this.target = targ;
    }
    public Actions(Actions actionToCopy)
    {
        this.actionPreCondition = actionToCopy.actionPreCondition;
        this.actionEffect = actionToCopy.actionEffect; ;
        this.actionCost = actionToCopy.actionCost; ;
        this.requiresInRange = actionToCopy.requiresInRange; ;
        this.target = actionToCopy.target; ;
    }

}
