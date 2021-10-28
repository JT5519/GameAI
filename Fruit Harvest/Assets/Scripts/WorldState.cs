using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//The state of the garden can be defined as the state of 4 things: the flower bushel, the tree groves, the statue and the resource chest
public class WorldState
{
    //enums to enumerate states of the gardens resources
    /*There are four points of interest in the garden:
        - The flower bushel that needs to be watered
        - The Tree grove that needs its fruits plucked
        - The Statue that needs to be worshiped
        - The resource chest that contains the resources to pluck fruits (maybe a net) and water the flowers (a water can)*/
    public enum FlowerBushel : int
    {
        ignore = -1,
        dry = 0,
        watered = 1
    }
    public enum TreeGrove : int
    {
        ignore = -1,
        ripe = 0,
        plucked = 1
    }
    public enum StatueRegion : int
    {
        ignore = -1,
        defiled = 0,
        sanctified = 1
    }
    public enum ResourceChest : int
    {
        ignore = -1,
        filled = 0,
        empty = 1
    }

    //state parameters of the world
    public FlowerBushel flowerState;
    public TreeGrove treeState;
    public StatueRegion statueState;
    public ResourceChest chestState;

    public WorldState(FlowerBushel f, TreeGrove t, StatueRegion s, ResourceChest r)
    {
        this.flowerState = f;
        this.treeState = t;
        this.statueState = s;
        this.chestState = r;
    }
    public WorldState(WorldState stateToCopy)
    {
        this.flowerState = stateToCopy.flowerState;
        this.treeState = stateToCopy.treeState;
        this.statueState = stateToCopy.statueState;
        this.chestState = stateToCopy.chestState;
    }
    public bool Equals(WorldState state)
    {
        if (this.flowerState != FlowerBushel.ignore && state.flowerState != FlowerBushel.ignore && this.flowerState != state.flowerState)
            return false;
        if (this.treeState != TreeGrove.ignore && state.treeState != TreeGrove.ignore && this.treeState != state.treeState)
            return false;
        if (this.statueState != StatueRegion.ignore && state.statueState != StatueRegion.ignore && this.statueState != state.statueState)
            return false;
        if (this.chestState != ResourceChest.ignore && state.chestState != ResourceChest.ignore && this.chestState != state.chestState)
            return false;
        return true;
    }
    public override string ToString()
    {
        return this.flowerState.ToString()+" "+
               this.treeState.ToString() + " "+
               this.statueState.ToString() + " "+
               this.chestState.ToString() + " ";
    }

}
