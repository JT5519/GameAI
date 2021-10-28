using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public WorldState currentWorldState; //Current State of the World
    public WorldState desiredWorldState; //Goal of the Agent

    public enum actionNames : int
    {
        pluckFruit,
        waterFlowers,
        prayStatue,
        collectResources
    }
    public List<Actions> possibleActions; //Actions agent can do
    public Queue<Actions> actionQueue; //action queue filled up by the planner

    enum targetNames : int
    {
        trees,flowers,statue,chest
    }
    SortedDictionary<targetNames, GameObject> possibleTargets; //targets for agent actions

    FSM fsm; //fsm
    bool pauseFSMUpdate;

    Planner planner; //planner

    public float moveSpeed;

    void fillActionArray()
    {
        //creating positions in list
        for (int i = 0; i < 4; i++)
        {
            possibleActions.Add(null);
        }
        //defining fruit plucking action
        possibleActions[(int)actionNames.pluckFruit] = new Actions(
            new WorldState(WorldState.FlowerBushel.ignore, WorldState.TreeGrove.ripe, WorldState.StatueRegion.ignore,WorldState.ResourceChest.empty),
            new WorldState(WorldState.FlowerBushel.ignore, WorldState.TreeGrove.plucked, WorldState.StatueRegion.ignore,WorldState.ResourceChest.ignore),
            1,true,possibleTargets[targetNames.trees]);
        //defining watering flower action
        possibleActions[(int)actionNames.waterFlowers] = new Actions(
            new WorldState(WorldState.FlowerBushel.dry, WorldState.TreeGrove.ignore, WorldState.StatueRegion.ignore,WorldState.ResourceChest.empty),
            new WorldState(WorldState.FlowerBushel.watered, WorldState.TreeGrove.ignore, WorldState.StatueRegion.ignore,WorldState.ResourceChest.ignore),
            2,true,possibleTargets[targetNames.flowers]);
        //defining pray statue action
        possibleActions[(int)actionNames.prayStatue] = new Actions(
            new WorldState(WorldState.FlowerBushel.ignore, WorldState.TreeGrove.ignore, WorldState.StatueRegion.defiled, WorldState.ResourceChest.ignore),
            new WorldState(WorldState.FlowerBushel.ignore, WorldState.TreeGrove.ignore, WorldState.StatueRegion.sanctified, WorldState.ResourceChest.ignore),
            3,true,possibleTargets[targetNames.statue]);
        //defining resource collecting action
        possibleActions[(int)actionNames.collectResources] = new Actions(
            new WorldState(WorldState.FlowerBushel.ignore, WorldState.TreeGrove.ignore, WorldState.StatueRegion.ignore, WorldState.ResourceChest.filled),
            new WorldState(WorldState.FlowerBushel.ignore, WorldState.TreeGrove.ignore, WorldState.StatueRegion.ignore, WorldState.ResourceChest.empty),
            4,true,possibleTargets[targetNames.chest]);
    }
    void fillTargetDict()
    {
        possibleTargets.Add(targetNames.trees, GameObject.Find("Trees"));
        possibleTargets.Add(targetNames.flowers, GameObject.Find("Flowers"));
        possibleTargets.Add(targetNames.statue, GameObject.Find("Statue"));
        possibleTargets.Add(targetNames.chest, GameObject.Find("Tool Chest"));
    }
    void Start()
    {
        //defining current state of the world
        currentWorldState = new WorldState(WorldState.FlowerBushel.dry, WorldState.TreeGrove.ripe, WorldState.StatueRegion.defiled,WorldState.ResourceChest.filled);
        //defining desired goal state of the world
        desiredWorldState = new WorldState(WorldState.FlowerBushel.watered, WorldState.TreeGrove.plucked, WorldState.StatueRegion.sanctified, WorldState.ResourceChest.ignore);
        //populating targets for actions
        possibleTargets = new SortedDictionary<targetNames, GameObject>();
        fillTargetDict();
        //populating action options of the agent
        possibleActions = new List<Actions>();
        fillActionArray();
        //Initializing action queue
        actionQueue = new Queue<Actions>();
        //FSM Object
        fsm = new FSM(FSM.FSMState.Idle);
        pauseFSMUpdate = false;
        //Planner object
        planner = new Planner();
    }

    // Update is called once per frame
    void Update()
    {
        if (actionQueue.Count == 0)
            actionQueue = planner.buildActionQueue(possibleActions, currentWorldState, desiredWorldState);
        FSMUpdate();
    }
    void FSMUpdate()
    {
        if (pauseFSMUpdate)
            return;
        if (actionQueue.Count == 0)
            fsm.SetState(FSM.FSMState.Idle);
        else
        {
            Actions nextAction = actionQueue.Dequeue();
            if(nextAction.requiresInRange && !nextAction.isInRange(gameObject))
            {
                fsm.SetState(FSM.FSMState.Move);
                pauseFSMUpdate = true;
                StartCoroutine(AgentMove(nextAction));
            }
            else
            {
                fsm.SetState(FSM.FSMState.Action);
                nextAction.doAction(currentWorldState);
            }
        }

    }
    public IEnumerator AgentMove(Actions action)
    {
        while(!action.isInRange(gameObject))
        {
            transform.Translate(moveSpeed * Vector3.Normalize(action.target.transform.position - transform.position)*Time.deltaTime);
            yield return null; 
        }
        if (pauseFSMUpdate)
            pauseFSMUpdate = false;
    }
}
