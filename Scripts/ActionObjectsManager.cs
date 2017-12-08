using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

namespace RUSH
{


    public class ActionObjectsManager : MonoBehaviour
    {

        #region Const
        public static readonly int CUBE_NUMBER = 5;
        #endregion
        
        private static ActionObjectsManager _instance;
        public List<Spawner> spawnerList;
        public  List<ActionObject> actionObjectsList;
        public static ActionObjectsManager instance
        {
            get
            {
                return _instance;
            }
        }

        public ActionEvent onTileTriggered;
        public PositioningEvent onSpawnSignal;
        public UnityEvent onAllSpawnersEmpty;
        
        public int emptySpawnerCounter = 0;

        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de InteractiveObjectsManager alors que c'est un singleton.");
            }
            _instance = this;
            onTileTriggered = new ActionEvent();
            spawnerList = new List<Spawner>();
            onSpawnSignal = new PositioningEvent();
            onAllSpawnersEmpty = new UnityEvent();
        }

        protected void Start()
        {
            
            Metronome.instance.onTic.AddListener(BeginCycle);
            MenuManager.instance.onRestart.AddListener(SwitchToDestroyState);
            CubeManager.instance.onReadyToRestart.AddListener(SwitchToNormalState);
            MenuManager.instance.onQuitLevel.AddListener(ClearAllActionsAndRestartSpawners);
            MenuManager.instance.onClearActions.AddListener(ClearActions);
        }



        protected void OnDestroy()
        {
            _instance = null;
        }


        // Passe tout les ActionObject en state restart pour qu'ils detruisent tout les cubes
        protected void SwitchToDestroyState()
        {
            GameManager.instance.hasGameBegun = false;
            

            foreach (ActionObject aObj in actionObjectsList)
            {
                aObj.previousState = aObj.currentState;
                aObj.currentState = ACTION_STATE.RESTART;
            }

            foreach (Spawner spawner in spawnerList)
            {
                spawner.cubeCounter = 0;
            }

        }

        // Remets tous les AObj à leur state normal 
        protected void SwitchToNormalState()
        {
            foreach (ActionObject aObj in actionObjectsList)
            {
                aObj.currentState = aObj.previousState;
                aObj.isSwitched = false;
            }
                
        }


        //Vide les tableaux et remets les compteurs des Spawners à 0
        private void ClearAllActionsAndRestartSpawners()
        {
            emptySpawnerCounter = 0;

            for (int i = spawnerList.Count - 1; i >= 0; i--)
            {

                spawnerList[i].cubeCounter = 0;
                spawnerList.Remove(spawnerList[i]);
            }

            for (int i = actionObjectsList.Count - 1; i >= 0; i--)
            {

                foreach (Transform child in actionObjectsList[i].transform)
                {
                    Destroy(child.gameObject);
                }
                if (actionObjectsList[i].currentState != ACTION_STATE.SPAWNER) actionObjectsList[i].transform.rotation = Quaternion.identity;
                actionObjectsList.Remove(actionObjectsList[i]);
            }

        }


        //Enleve toutes les plaques d'actions posées par le joueur
        protected void ClearActions()
        {
            foreach (ActionObject aObj in actionObjectsList)
            {
                foreach (Transform child in aObj.transform)
                {
                    Destroy(child.gameObject);
                }
                if (aObj.currentState != ACTION_STATE.SPAWNER && aObj.currentState != ACTION_STATE.TARGET && aObj.currentState != ACTION_STATE.TELEPORT)
                {
                    aObj.transform.rotation = Quaternion.identity;
                    aObj.currentState = ACTION_STATE.GROUND;
                }
                aObj.isFixed = false;
                    
            }
        }

        //Déclenché à chaque tic
        private void BeginCycle()
        {
            
            if (emptySpawnerCounter == spawnerList.Count) onAllSpawnersEmpty.Invoke();
            foreach (Spawner spawner in spawnerList) spawner.SpawningProcess();
            foreach (ActionObject aObj in actionObjectsList) aObj.CheckForCube();
        }


        // Event envoyé au CubeManager pour qu'il spawn un cube
        public void SendSpawnSignal(Vector3 position, Quaternion rotation, COLOR color)
        {
            onSpawnSignal.Invoke(position, rotation, color);

        }
        // Event envoyé au CubeManager pour qu'il donne un comportement au cube correspondant
        public void SendActionEvent(RaycastHit hit, ActionObject aObj)
        {
            
            onTileTriggered.Invoke(hit, aObj);
        }
    }
}