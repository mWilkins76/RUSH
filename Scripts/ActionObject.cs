using UnityEngine;
using UnityEngine.Events;
namespace RUSH
{

    public enum ACTION_STATE
    {
        ARROW = 0,
        CONVEYOR = 1,
        STOP = 2,
        SWITCH = 3,
        TELEPORT = 4,
        GROUND = 5,
        TARGET = 6,
        SPAWNER = 7,
        RESTART = 8
    }

    /// </summary>
    public class ActionObject : LevelObject
    {

        public ACTION_STATE currentState;
        public ACTION_STATE previousState;

        public bool isSwitched;
        public bool isFixed = false;
        public int btnRef;

        protected virtual void Start()
        {
            SetColor();
            if (currentState == ACTION_STATE.SWITCH) isSwitched = false;
            ActionObjectsManager.instance.actionObjectsList.Add(this);
            
        }


        // donne comportement au cube à chaque tic
        public void CheckForCube()
        {
            
            RaycastHit lHit;
            if (Physics.Raycast(transform.position, transform.up, out lHit,1f, 1 << LayerMask.NameToLayer("cube")))
            {

                GiveBehavior(lHit);
            }
        }

        protected virtual void GiveBehavior(RaycastHit hit)
        {
            ActionObjectsManager.instance.SendActionEvent(hit, this);
            if (currentState == ACTION_STATE.SWITCH) isSwitched = !isSwitched;
        }


    }
}