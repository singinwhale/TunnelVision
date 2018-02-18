using System;
using lib.Data.Config;
using lib.System.Level;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace lib.System
{
    /// <summary>
    /// Provides communication between scenes and
    /// is the entry point for the whole loading process.
    /// </summary>
    public class World : MonoBehaviour
    {
        // properties
        //----------------------------------

        public LevelController LevelController
        {
            get { return _levelController; }
            private set { _levelController = value; }
        }


        public String ProcessID { get; set; }
        public String ScenarioID { get; set; }

        [SerializeField] private static World _instance;
        
        [SerializeField] private LevelController _levelController;

        public View.Level.Level Level
        {
            get { return LevelController.Level; }
        }
        
        /// <summary>
        /// Singleton like usage will create a new world in the current context if necessary. Should not be happening
        /// however as we will make sure the world exists prior to any level being loaded
        /// </summary>
        public static World Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<World>();
                }
                Debug.Assert(_instance != null, "World is missing!");
                return _instance;
            }
        }
        
        // methods
        //----------------------------------
        
        public void Awake()
        {
            ProcessID = "process_1";
            ScenarioID = "badThingsAreHappening";
            
            // get the required data from the config
            var process = Config.Instance.Processes.Find(p => p.ID == ProcessID);
            var scenario = process.Scenarios.Find(sce => sce.ID == ScenarioID);
            // load the level
            LevelController = new LevelController(); // the level controller takes it from here
            LevelController.Initialize(scenario);
        }
        
        
        public void Start()
        {
            // make sure the world is not unloaded once we change scenes
            hideFlags |= HideFlags.DontUnloadUnusedAsset;
        }


        public void Update()
        {
            LevelController.Update();
        }
    }
}