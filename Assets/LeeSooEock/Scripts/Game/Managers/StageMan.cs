using System;
using System.Collections.Generic;
using UnityEngine;
using SimpleStateMachine;
using System.Collections;

namespace SUOKI
{
	public class StageMan : SingletonMonoD<StageMan>
	{
		[HideInInspector]
		public StageController con;

		public List<Unit> units;

		//cached
		List<IMovable> _movables = new List<IMovable>();

		Coroutine moveCo = null;

		public enum State
		{
			init,		//게임 세팅
			idle,		//input 을 받을 수 있는단계
			move,		//이동하고 있는 페이즈
		}
		public SStateMachine<State> state;

		public void Init(StageController con)
		{
			this.con = con;
			con.Init();

			state = new SStateMachine<State>(gameObject, false);
			Transition( State.init );

			units = con.map.units;
			foreach ( var item in units ) {
				item.Init();
			}

			Transition( State.idle );
		}

		public void Fin()
		{
			con.Fin();
			foreach ( var item in units ) {
				item.Fin();
			}
		}


		public void AddUnit(Unit unit)
		{
			con.map.AddUnit( unit );
			units.Add( unit );
		}

		public void RemoveUnit(Unit unit )
		{
			con.map.RemoveUnit( unit );
			units.Remove( unit );
		}

		public List<IMovable> GetMovables()
		{
			_movables.Clear();
			foreach ( var item in units ) {
				var movable = item.GetComponent<IMovable>();
				if ( movable == null )
					continue;

				if ( !movable.IsMovable() ) 
					continue;
				
				if ( movable != null ) {
					_movables.Add( movable );
				}
			}
			return _movables;
		}

		public void Transition(State s )
		{
			//Todo When ChangedStates.
			switch ( s ) {
				case State.init:
					break;
				case State.idle:
					break;
				case State.move:
					break;
				default:
					break;
			}
			state.ChangeState( s );
		}

		public State GetCurrentState()
		{
			return state.CurrentState;
		}

		public void InputMove(InputDir dir)
		{
			if ( dir == InputDir.None )
				return;

			if ( GetCurrentState() != State.idle )
				return;

			Transition( State.move );

			if ( moveCo != null ) StopCoroutine( moveCo );
			moveCo = StartCoroutine( MoveCo( dir ) );
		}

		IEnumerator MoveCo( InputDir dir )
		{
			//케릭터들을 이동시키면
			//연계 이동이 벌어지기도한다.
			//모든 케릭터들이 멈춤에 상태에 놓이게되었음을 받아야
			//이동이 끝났다고 판단한다.

			//이 루틴이 맞다.
			//내이동 -> 토스 or 토스가 아니면 남은 내이동 -> 도착 지점에서 발생하는이벤트처리(두개의 알이 한곳에 만나거나 골에 도착하거나)
			//      -> 토스 or 토스가 아니면 남은 내이동 -> 도착 지점에서 발생하는이벤트처리(두개의 알이 한곳에 만나거나 골에 도착하거나) 
			//      -> 토스 or 토스가 아니면 남은 내이동 -> 도착 지점에서 발생하는이벤트처리(두개의 알이 한곳에 만나거나 골에 도착하거나) 

			//즉 디렉션을 넣어서 내 이동을하는 로직하나

			//토스로인해서 이동되는 로직 하나.


			var movables = GetMovables();
			foreach ( var item in movables ) {
				//이동가능한가.

				//어디로가는가.

				//이동 처리 완료했는가.

				//모두 이동했는가.

				if ( con.map.TryMoveTo( item, dir ) == true ) {

				}


				//if( item.IsMovePossible(dir) ) {
				//}
				yield return null;
			}
			yield break;
		}

	}
}
