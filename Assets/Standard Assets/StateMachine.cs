using UnityEngine;
using System.Collections;

public class State {
	
	public virtual void OnEnter () {
	}
	
	public virtual void OnUpdate () {
	}
	
	public virtual void OnExit () {
	}
	
	/*
	public virtual void OnGUI () {
	}
	*/
}

public class StateMachine : MonoBehaviour {
	
	protected Hashtable states = new Hashtable();
	
	private class IdleState : State {
		
		StateMachine sm;
		
		public IdleState (StateMachine sm) {
			this.sm = sm;
		}
		
		public override void OnUpdate () {
			if (sm.queue.Count>0) {
				sm.Change((State)sm.queue.Dequeue());
			}
		}
	}
	
	private class WaitState : State {
		
		StateMachine sm;
		public float seconds = 0;
		
		public WaitState (StateMachine sm, float seconds) {
			this.sm = sm;
			this.seconds = seconds;
		}
		
		public override void OnUpdate () {
			seconds -= Time.deltaTime;
			if (seconds<0) {
				sm.Change(new IdleState(sm));
			}
		}
	}
	
	public State state = new State();
	public Queue queue = new Queue();
	public string _state = "";
	
	public bool InIdle () {
		return state is IdleState;
	}
	
	public void Idle () {
		IdleState idleState = (IdleState)states[typeof(IdleState)];
		if (idleState == null) {
			idleState = new IdleState(this);
			states[typeof(IdleState)] = idleState;
		}
		Change(idleState);
	}
	
	public void QueueIdle () {
		QueueChange(new IdleState(this));
	}
	
	public void Wait (float seconds) {
		WaitState waitState = (WaitState)states[typeof(WaitState)];
		if (waitState == null) {
			waitState = new WaitState(this,0);
			states[typeof(WaitState)] = waitState;
		}
		waitState.seconds = seconds;
		Change(waitState);
	}
	
	public void QueueWait (float seconds) {
		QueueChange(new WaitState(this,seconds));
	}
	
	public void QueueChange (State state) {
		queue.Enqueue(state);
	}
	
	public virtual void Change (State state) {
		this.state.OnExit();
		this._state = state.GetType().ToString();
		this.state = state;
		this.state.OnEnter();
	}

}
