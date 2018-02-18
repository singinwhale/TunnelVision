using System;
using System.Collections.Generic;
using lib.Data.Scenario;
using lib.View.Level.Nodes;
using UnityEngine;

namespace lib.System.Level
{
	/// <summary>
	/// Abstraction of the node queue in the scenario. This class has no clue about distances or geometry. It Tells the
	/// View that it is active but not much more
	/// </summary>
	public class LevelNodeController
	{
		//fields
		//---------------------------------------------
		
		/// <summary> The step of this node. The Model part.</summary>
		private Scenario.IScenarioStep _step;
		
		/// <summary>The View part</summary>
		private LevelNode _levelNode;

		private int _length;

		/// <summary>
		/// Maps a Model class to their corresponding View Class. In this case it maps Steps to Nodes
		/// </summary>
		public readonly static Dictionary<Type, Type> ScenarioStepToLevelNodeTypeDictionary = new Dictionary<Type, Type>
		{
			{typeof(Scenario.TextStep), typeof(TextNode)},
			{typeof(Scenario.NodeStep), typeof(PlayerTaskNode)}
		};


		//properties
		//---------------------------------------------

		public int Length
		{
			get { return _length; }
			set
			{
				_length = value;
				_levelNode.Shaper.Start = Offset;
				OnLengthChanged(this);
			}
		}

		/// <summary>
		/// The Staring point of this 
		/// </summary>
		public int Offset
		{
			get
			{
				int offset = 0;
				var prev = Previous;
				while (prev != null)
				{
					offset += prev.Length;
					prev = prev.Previous;
				}

				return offset;
			}
		}

		public LevelNodeController Previous { get; private set; }

		/// <see cref="_step"/>
		public Scenario.IScenarioStep Step
		{
			get { return _step; }
		}

		/// <summary>The View part</summary>
		public LevelNode LevelNode
		{
			get { return _levelNode; }
		}

		
		//events
		//---------------------------------------------
		
		public delegate void LevelNodeControllerEvent(LevelNodeController sourceNode);

		public event LevelNodeControllerEvent OnFinished;
		public event LevelNodeControllerEvent OnLengthChanged;
		
		//Methods
		//---------------------------------------------
		public LevelNodeController()
		{
			Previous = null;
			_step = null;
			_length = 0;
			_levelNode = World.Instance.Level.CreateLevelNode<DefaultNode>(this);
			OnLengthChanged += _levelNode.OnPreviousNodeChangedLength;
		}

		public void Update()
		{
			if (LevelNode.CouldBeVisible())
			{
				LevelNode.gameObject.SetActive(true);
				LevelNode.Tick();
			}
			else
			{
				LevelNode.gameObject.SetActive(false);
			}
		}

		public LevelNodeController(LevelNodeController previous, Scenario.IScenarioStep step)
		{
			Previous = previous;
			_step = step;
			_length = step.DefaultLength;

			//instantiate the corresponding LevelNode for the step by calling Level's templated method via reflection
			Type type = ScenarioStepToLevelNodeTypeDictionary[step.GetType()];
			Debug.Assert(type.IsSubclassOf(typeof(LevelNode)));
														// ReSharper disable once PossibleNullReferenceException
			var genericMethod = typeof(View.Level.Level).GetMethod("CreateLevelNode").MakeGenericMethod(type);
			_levelNode = (LevelNode)genericMethod.Invoke(World.Instance.Level, new object[]{this});
		}

		/// <summary>
		/// Called when this node's logic should take over
		/// </summary>
		public void Enable()
		{
			
		}
	}
}