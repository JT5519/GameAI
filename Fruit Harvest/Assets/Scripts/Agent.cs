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

    public float moveSpeed; //agent movement speed

    enum ActionCompletionState : int
    {
        ignore, toDo, done
    } //enum to check action completion status
    ActionCompletionState actionCompletionState;

    //function to populate agents allowed action set
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
    //function to populate targets dictionary 
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
        //action completion status
        actionCompletionState = ActionCompletionState.ignore;
    }

    // Update is called once per frame
    void Update()
    {
        //if action queue is empty, call for a new plan
        if (actionQueue.Count == 0)
            actionQueue = planner.buildActionQueue(possibleActions, currentWorldState, desiredWorldState);
        //update the FSM based on actions in the action queue
        FSMUpdate();
    }
    void FSMUpdate()
    {
        //return if FSM update is paused for now
        if (pauseFSMUpdate)
            return;
        //if planner returned an empty action queue, set state to idle
        if (actionQueue.Count == 0)
            fsm.SetState(FSM.FSMState.Idle);
        else
        {
            Actions nextAction = actionQueue.Dequeue();
            //Debug.Log("Action to be done: " + (Agent.actionNames)(nextAction.actionCost - 1));

            //if next action needs moving to a position then set FSM state to move
            if (nextAction.requiresInRange && !nextAction.isInRange(gameObject))
            {
                //Debug.Log("Moving towards target: "+nextAction.target.name);
                fsm.SetState(FSM.FSMState.Move);
                pauseFSMUpdate = true; //wait while movememnt is happening
                actionCompletionState = ActionCompletionState.toDo;
                StartCoroutine(AgentMove(nextAction)); //begin move
            }
            else
                AgentAct(nextAction);
        }

    }
    void AgentAct(Actions action) //do action on the word
    {
        fsm.SetState(FSM.FSMState.Action);
        //Debug.Log("World state before action: " + currentWorldState.ToString());
        action.doAction(currentWorldState);
        //Debug.Log("Action commited");
        //Debug.Log("World state after action: " + currentWorldState.ToString());
        if (actionCompletionState == ActionCompletionState.toDo)
            actionCompletionState = ActionCompletionState.done;
    }
    //coroutine to control player movement
    public IEnumerator AgentMove(Actions action)
    {
        while(!action.isInRange(gameObject))
        {
            transform.Translate(moveSpeed * Vector3.Normalize(action.target.transform.position - transform.position)*Time.deltaTime);
            yield return null; 
        }
        if (pauseFSMUpdate)
            pauseFSMUpdate = false;
        //Debug.Log("Movement complete");
        //movemement complete, time to act
        if (actionCompletionState == ActionCompletionState.toDo)
            AgentAct(action);
    }
}
