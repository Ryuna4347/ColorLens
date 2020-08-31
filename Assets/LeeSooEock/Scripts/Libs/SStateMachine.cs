using UnityEngine;
using System;

namespace SimpleStateMachine
{
	public struct StateChangeEvent<T> where T : struct, IComparable, IConvertible, IFormattable
	{
		public GameObject Target;
		public SStateMachine<T> TargetStateMachine;
		public T NewState;
		public T PreviousState;

		public StateChangeEvent( SStateMachine<T> stateMachine )
		{
			Target = stateMachine.Target;
			TargetStateMachine = stateMachine;
			NewState = stateMachine.CurrentState;
			PreviousState = stateMachine.PreviousState;
		}
	}

	public interface IStateMachine
	{
		bool TriggerEvents { get; set; }
	}

	public class SStateMachine<T> : IStateMachine where T : struct, IComparable, IConvertible, IFormattable
	{
		public bool TriggerEvents { get; set; }
		public GameObject Target;

		public T CurrentState { get; protected set; }
		public T PreviousState { get; protected set; }
		public delegate void OnStateChangeDelegate();
		public OnStateChangeDelegate OnStateChange;

		public SStateMachine( GameObject target, bool triggerEvents )
		{
			this.Target = target;
			this.TriggerEvents = triggerEvents;
		}

		public virtual void ChangeState( T newState )
		{
			if ( newState.Equals( CurrentState ) ) {
				return;
			}
			PreviousState = CurrentState;
			CurrentState = newState;
			OnStateChange?.Invoke();
		}

		public virtual void RestorePreviousState()
		{
			CurrentState = PreviousState;
			OnStateChange?.Invoke();
		}
	}
}