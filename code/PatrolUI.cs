// use this to realize patrolling
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tem.Action;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class PatrolUI : SSActionManager, ISSActionCallback, Observer {
    private Animator ani;
    private SSAction currentAction;
    public enum ActionState : int { IDLE, WALKLEFT, WALKFORWARD, WALKRIGHT, WALKBACK } // the 5 states of the action of the patrol
    private ActionState currentState;

    // the speed of walking and running
    private const float walkSpeed = 1f;
    private const float runSpeed = 2f;

	// Use this for initialization
	new void Start () {
        // when starting the patrol stays still
        ani = this.gameObject.GetComponent<Animator>();
        Publish publisher = Publisher.getInstance();
        publisher.add(this);
        currentState = ActionState.IDLE;
        idle();
	}
	
	// Update is called once per frame
	new void Update () {
        base.Update();
	}

    // use this to realize the circling
    public void SSEventAction(SSAction source, SSActionEventType events = SSActionEventType.COMPLETED, int intParam = 0, string strParam = null, Object objParam = null)
    {
        currentState = currentState > ActionState.WALKBACK ? ActionState.IDLE : (ActionState)((int)currentState + 1);
        switch (currentState)
        {
            // go forward
            case ActionState.WALKFORWARD:
                walkForward();
                break;
            // go back
            case ActionState.WALKBACK:
                walkBack();
                break;
            // go left
            case ActionState.WALKLEFT:
                walkLeft();
                break;
            // go right
            case ActionState.WALKRIGHT:
                walkRight();
                break;
            // stay still
            default:
                idle();
                break;
        }
    }

    public void idle()
    {
        currentAction = IdleAction.GetIdleAction(Random.Range(1, 1.5f), ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    public void walkLeft()
    {
        Vector3 target = Vector3.left * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }
    public void walkRight()
    {
        Vector3 target = Vector3.right * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    public void walkForward()
    {
        Vector3 target = Vector3.forward * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }
    
    public void walkBack()
    {
        Vector3 target = Vector3.back * Random.Range(3, 5) + this.transform.position;
        currentAction = WalkAction.GetWalkAction(target, walkSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    public void turnNextDirection()
    {
        currentAction.destory = true;
        switch (currentState)
        {
            case ActionState.WALKFORWARD:
                currentState = ActionState.WALKBACK;
                walkBack();
                break;
            case ActionState.WALKBACK:
                currentState = ActionState.WALKFORWARD;
                walkForward();
                break;
            case ActionState.WALKLEFT:
                currentState = ActionState.WALKRIGHT;
                walkRight();
                break;
            case ActionState.WALKRIGHT:
                currentState = ActionState.WALKLEFT;
                walkLeft();
                break;
        }
    }

    public void notified(ActorState state, int pos, GameObject actor)
    {
        if (state == ActorState.ENTER_AREA)
        {
            // judge whether the goal is in its sphere
            if (pos == this.gameObject.name[this.gameObject.name.Length - 1] - '0')
                getGoal(actor);
            else loseGoal();
        }
        // the goal gets caught and died
        else stop();
    }

    // when the goal appears in the patrol's sphere
    public void getGoal(GameObject gameobject)
    {
        currentAction.destory = true;
        currentAction = RunAction.GetRunAction(gameobject.transform, runSpeed, ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    // when the goal disappears from the patrol's sphere
    public void loseGoal()
    {
        currentAction.destory = true;
        idle();
    }

    public void stop()
    {
        currentAction.destory = true;
        currentAction = IdleAction.GetIdleAction(-1f, ani);
        this.runAction(this.gameObject, currentAction, this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject. name);
        Transform parent = collision.gameObject.transform.parent;
        if (parent != null && parent.CompareTag("Wall")) turnNextDirection();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door")) turnNextDirection();
    }
}
