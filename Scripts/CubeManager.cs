using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace RUSH
{

    public enum DIRECTION
    {
        LEFT = 0,
        RIGHT = 1,
        FRONT = 2,
        BACK = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public class CubeManager : MonoBehaviour
    {
        #region INITIALIZATION

        #region Private Variables
        private static CubeManager _instance;
        #endregion

        #region Public Variables
        public List<Cube> list = new List<Cube>();
        public float ratio;
        public float decreasingRatio;
        public Cube cubeFab;
        public static CubeManager instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region Events
        
        public CubeEvent onGameLost;
        public UnityEvent onNoMoreCube;
        public UnityEvent onReadyToRestart;
        #endregion

        #endregion

        #region LIFE CYCLE

        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de CubeManager alors que c'est un singleton.");
            }
            _instance = this;
            onGameLost = new CubeEvent();
            onNoMoreCube = new UnityEvent();
            onReadyToRestart = new UnityEvent();
        }

        protected void Start()
        {
            Metronome.instance.onTic.AddListener(CheckFallAndWin);
            ActionObjectsManager.instance.onSpawnSignal.AddListener(SpawnCube);
            ActionObjectsManager.instance.onTileTriggered.AddListener(ChangeCubeBehavior);
            MenuManager.instance.onQuitLevel.AddListener(ClearAllCubes);
        }

        protected void Update()
        {
            // Met à jour le ratio de temps entre deux tics
            ratio = (CustomTimer.instance.elapsedTime) / Metronome.instance.Tempo;
            decreasingRatio = 1 - ratio;
            // Execute les DoAction des cubes
            if(list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].DoAction();
                }
            }
            
        }

        protected void OnDestroy()
        {
            _instance = null;
        }
        #endregion

        #region VARIOUS PRIVATE METHODS
        
        // Reçoit le tic uniquement pour vérifier le Fall et si il y a encore des cubes
        private void CheckFallAndWin()
        {
            if (list.Count != 0 && GameManager.instance.hasGameBegun) SfxManager.manager.PlaySfx("8");
            if (list.Count == 0 && GameManager.instance.hasGameBegun) onNoMoreCube.Invoke();

            for (int i = 0; i < list.Count; i++)
            {
                list[i].LockPosition();
                if (list[i].isFalling())
                {
                    
                    if (list[i].isGoneFromLevel() )
                    {
                        if (!list[i].isInStepAction)
                        {
                            list[i].currentState = STATE.RESTARTING;
                            SendLooseEvent(list[i]);
                        }
                        
                    }
                    else list[i].currentState = STATE.FALLING;



                    list[i].BeginCycle();
                }
            }
        }


        private void SpawnCube(Vector3 position, Quaternion rotation, COLOR color)
        {
            if (GameManager.instance.hasGameBegun == false) GameManager.instance.hasGameBegun = true;
            cubeFab.tag = color.ToString();
            cubeFab.color = color;
            cubeFab.transform.position = position;
            cubeFab.transform.rotation = rotation;
            Instantiate(cubeFab);
        }




        
        // Déclenché quand un ActionObject a été activé
        private void ChangeCubeBehavior(RaycastHit hit, ActionObject actionObject)
        {

            for (int i = 0; i < list.Count; i++)
            {
                Cube cube = list[i];
                if (cube.transform.position == hit.transform.position)
                {
                    if (!cube.isInStepAction)
                    {
                        switch (actionObject.currentState)
                        {
                            case ACTION_STATE.ARROW:
                                cube.transform.rotation = actionObject.transform.rotation;
                                cube.currentState = STATE.MOVE;
                                break;

                            case ACTION_STATE.CONVEYOR:
                                cube.currentState = STATE.CONVEYED;
                                cube.translationVector = actionObject.transform.forward;
                                break;

                            case ACTION_STATE.STOP:
                                if (!cube.hasBeenStopped) cube.currentState = STATE.STOPPED;
                                else
                                {
                                    cube.currentState = STATE.MOVE;
                                    cube.hasBeenStopped = false;
                                }

                                break;

                            case ACTION_STATE.SWITCH:
                                cube.currentState = STATE.MOVE;
                                if (actionObject.isSwitched) cube.Rotate(DIRECTION.RIGHT);
                                else cube.Rotate(DIRECTION.LEFT);
                                break;

                            case ACTION_STATE.TELEPORT:
                                if (cube.currentState != STATE.CHANGING_POSITION && !cube.hasBennTeleported)
                                {
                                    cube.currentState = STATE.TELEPORTING;
                                    cube.teleportingPos = new Vector3(actionObject.transform.position.x, actionObject.transform.position.y + 1, actionObject.transform.position.z);
                                }
                                break;

                            case ACTION_STATE.SPAWNER:
                            case ACTION_STATE.GROUND:
                                if (!cube.hasBeenConveyed) cube.currentState = STATE.MOVE;
                                else
                                {
                                    cube.currentState = STATE.STOPPED;
                                    cube.hasBeenConveyed = false;
                                }
                                break;

                            case ACTION_STATE.TARGET:
                                if (actionObject.color == cube.color) cube.currentState = STATE.DISAPPEARING;
                                else
                                {
                                    if (!cube.hasBeenConveyed) cube.currentState = STATE.MOVE;
                                    else
                                    {
                                        cube.currentState = STATE.STOPPED;
                                        cube.hasBeenConveyed = false;
                                    }
                                }
                                break;

                            case ACTION_STATE.RESTART:
                                cube.currentState = STATE.RESTARTING;
                                break;
                        }
                    }
                    

                    cube.BeginCycle();


                }
            }
        }

        private void ClearAllCubes()
        {
            for (int i = list.Count-1; i >= 0 ; i--)
            {
                
                Destroy(list[i].gameObject);
                list.Remove(list[i]);
            }
        }


        //Envoie Un Event au GameManager quand c'est perdu (avec une info sur le cube concerné pour lui appliquer un Halo) 
        public void SendLooseEvent(Cube cube)
        {
            onGameLost.Invoke(cube);
        }

        // Envoie un event au GM pour lui dire que tout les cubes on été détruit après avoir cliqué sur restart et qu'il peut passer en mode reflexion
        public void SendRestartEvent()
        {
           if (list.Count == 0)
            {
                onReadyToRestart.Invoke();
                
            }
                
        }

        #endregion

        #region VARIOUS PUBLIC METHODS

        #endregion



    }
}