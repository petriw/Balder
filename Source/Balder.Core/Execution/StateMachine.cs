#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Balder.Core.Utils;

namespace Balder.Core.Execution
{
	/// <summary>
	/// Represents the different phases of statechanging
	/// </summary>
	/// <remarks>
	/// A state has different phases : 
	/// 
	/// Enter - Occurs before a State goes into run phase. This phase runs only once
	/// Run - Running phase of a state - occurs until a statechange has been issued
	/// Leave - Occurs after a running state has changed the state of a statemachine. This phase runs only once.
	/// </remarks>
	public enum StatePhase
	{
		Enter = 1,
		Run,
		Leave
	}

	/// <summary>
	/// A statemachine implementation based upon a generic state type
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>
	/// The enum used as a generic type parameter for the class represents methods
	/// in a specific pattern within the class. 
	/// 
	/// A state consists of 3 phases : Enter, Run, Leave. 
	/// Enter occurs before a statechange is completed, Run is the running phase of the
	/// state and leave is when one is leaving the state and entering a new state.
	/// 
	/// Every state has 3 methods you can choose to implement in the following pattern : 
	/// 
	/// On[StateName]Enter
	/// On[StateName]Run
	/// On[StateName]Leave
	/// 
	/// If you do not need one or more of the phases, just skip implementing the method.
	/// 
	/// The methods implemented need to be public and instance members of the class.
	/// See the example for more information
	/// </remarks>
	/// <example>
	/// 
	/// public enum MyStates
	/// {
	///		Idle=1,
	///		DoSomething,
	///		Die
	/// }
	/// 
	/// public class MyStateMachine<MyStates>
	/// {
	///		public void OnIdleRun()
	///		{
	///			ConsoleKeyInfo keyInfo = Console.ReadKey();
	///			if( keyInfo == ConsoleKey.Spacebar ) 
	///			{
	///				ChangeState(MyStates.DoSomething);
	///			}
	///		}
	///		
	///		public void OnDoSomethingEnter()
	///		{
	///			Console.WriteLine("DoSomething - Enter");
	///		}
	///		
	///		public void OnSomethingRun()
	///		{
	///			ConsoleKeyInfo keyInfo = Console.ReadKey();
	///			if( keyInfo == ConsoleKey.Enter ) 
	///			{
	///				ChangeState(MyStates.Die);
	///			}
	///		}
	///		
	///		public void OnDoSomethingEnter()
	///		{
	///			Console.WriteLine("DoSomething - Leave");
	///		}
	///		
	///		public void OnDieEnter()
	///		{
	///			System.Diagnostics.Process.GetCurrentProcess().Kill();
	///		}
	///		
	///		public MyStates DefaultState { get { return MyStates.Idle; } }
	/// 
	/// }
	/// 
	/// </example>
	public abstract class StateMachine<T> : IStateMachine, IDisposable
	{
		#region Inner class(es)
		private class StateMethods
		{
			private T _state;
			private StateMachine<T> _stateMachine;
			private Dictionary<StatePhase, MethodInfo> _methods; 

			internal StateMethods(T state, StateMachine<T> stateMachine)
			{
				_state = state;
				_stateMachine = stateMachine;
				_methods = new Dictionary<StatePhase, MethodInfo>();
			}

			internal void SetMethod(StatePhase phase, MethodInfo method)
			{
				_methods[phase] = method;
			}

			internal void Invoke(StatePhase phase)
			{
				if (_methods.ContainsKey(phase))
				{
					MethodInfo method = _methods[phase];
					if (null != method)
					{
						method.Invoke(_stateMachine, null);
					}
				}
			}
		}
		#endregion

		#region Private fields
		private Dictionary<T, StateMethods> _methods;
		#endregion

		#region Public properties
		/// <summary>
		/// Gets the previous state
		/// </summary>
		public T PreviousState { get; private set; }

		/// <summary>
		/// Gets the current state
		/// </summary>
		public T CurrentState { get; private set; }

		/// <summary>
		/// Gets the next state
		/// </summary>
		protected T NextState { get; private set; }
		#endregion

		#region Constructor(s)
		protected StateMachine()
		{
			Type type = typeof(T);

			// We do not support anything else but Enums
			if (!type.IsEnum)
			{
				throw new ArgumentException("Generic parameter for a StateMachine must be an enum. The enum defines the states available.");
			}

			_methods = new Dictionary<T, StateMachine<T>.StateMethods>();

			// Get all methods from this type - we only support public and instance methods
			MethodInfo[] methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

			// Get all supported phases
			StatePhase[] phases = EnumHelper.GetValues<StatePhase>();

			// Get all supported states
			T[] states = EnumHelper.GetValues<T>();
			foreach (T state in states)
			{
				StateMethods stateMethods = new StateMachine<T>.StateMethods(state, this);

				foreach (StatePhase phase in phases)
				{
					MethodInfo method = GetMethod(methods, state, phase);
					stateMethods.SetMethod(phase, method);
				}

				_methods[state] = stateMethods;
			}

			// Register this statemachine
			StateMachineManager.Instance.Register(this);

			// OnInitialize states
			PreviousState = DefaultState;
			CurrentState = DefaultState;
			NextState = DefaultState;

			ChangeStateImplementation(DefaultState,false);
		}
		#endregion

		#region Private Methods

		private MethodInfo GetMethod(MethodInfo[] methods, T state, StatePhase phase)
		{
			string methodName = "On" + state.ToString() + phase.ToString();

			var method = (from m in methods
			              where m.Name.Equals(methodName)
			              select m).SingleOrDefault();
			return method;
		}

		private void Execute(T state, StatePhase phase)
		{
			if (_methods.ContainsKey(state))
			{
				StateMethods methods = _methods[state];
				methods.Invoke(phase);
			}
		}

		private void ChangeStateImplementation(T nextState, bool executeLeave)
		{
			NextState = nextState;
			if (executeLeave)
			{
				Execute(CurrentState, StatePhase.Leave);
			}

			PreviousState = CurrentState;
			CurrentState = nextState;

			Execute(CurrentState, StatePhase.Enter);
		}

		#endregion

		#region Protected methods
		protected virtual void OnRun()
		{
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Changes states and handles the different state phases
		/// </summary>
		/// <param name="nextState">State to change to</param>
		public void ChangeState(T nextState)
		{
			ChangeStateImplementation(nextState, true);
		}

		/// <summary>
		/// Gets the Default state for the statemachine
		/// </summary>
		public abstract T DefaultState { get; }

		/// <summary>
		/// Execute statemachine
		/// </summary>
		public void Execute()
		{
			Execute(CurrentState, StatePhase.Run);
			OnRun();
		}
		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}