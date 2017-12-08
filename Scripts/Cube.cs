using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace RUSH
{
    public enum STATE
    {
        MOVE = 0,
        STOPPED = 1,
        CHANGING_POSITION = 2,
        CONVEYED = 3,
        TELEPORTING = 4,
        APPEAR = 5,
        DESTROYING = 6,
        DISAPPEARING =7,
        FALLING = 8, 
        RESTARTING = 9,
        CLEARING = 10
    }

    public class Cube : LevelObject
    {

        #region INITIALIZATION
        #region Events
        #endregion

        #region Private Variables
        private Vector3 m_axis;
        private float m_degrees = 90;
        private Vector3 m_pivot;
        private float m_substractedAngle = 0;
        private Vector3 m_startPos;
        private Vector3 m_endPos;

        public GameObject explosion;

        public bool isInStepAction = false;
        public bool hasBennTeleported = false;
        public bool hasBeenStopped = false;
        public bool hasBeenConveyed = false;

        public Behaviour halo;

        public Vector3 teleportingPos;
        public Vector3 translationVector;

        public STATE currentState;


        #endregion
        #region Delegate
        public delegate void Action();
        public Action DoAction;
        public delegate void State();
        public State SelectMode;
        #endregion
        #endregion

        #region LIFE CYCLE

        // Use this for initialization
        void Start()
        {
            SetColor();
            DoAction = DoActionVoid;
            CubeManager.instance.list.Add(this);
            SetModeAppear();
            halo = (Behaviour)GetComponent("Halo");
        }





        #endregion

        #region STATE MACHINE


        // Déclenché après avoir reçu un comportement (donc à chaque tic)
        public void BeginCycle()
        {
            
            LockPosition();
            SetPivot(DIRECTION.FRONT);
            m_axis = transform.right;

            switch (currentState)
            {
                case STATE.CHANGING_POSITION:
                case STATE.APPEAR:
                    SelectMode = SetModeAppear;
                    break;

                case STATE.MOVE:
                    SelectMode = SetModeMove;
                    break;

                case STATE.STOPPED:
                    hasBeenStopped = true;
                    SelectMode = SetModeWait;
                    break;

                case STATE.CONVEYED:
                    SelectMode = SetModeTranslate;
                    break;

                case STATE.FALLING:
                    translationVector = -transform.up;
                    SelectMode = SetModeTranslate;
                    break;

                case STATE.DISAPPEARING:
                case STATE.TELEPORTING:
                case STATE.RESTARTING:
                    SelectMode = SetModeDisappear;
                    break;

                case STATE.CLEARING:
                case STATE.DESTROYING:
                    SelectMode = SetModeDestroy;
                    break;
            }

            

            SelectMode();

        }

        #region Set Modes


        // Sert à la fois pour le spawn et pour la téléportation
        public void SetModeAppear()
        {

            if (currentState == STATE.CHANGING_POSITION)
            {
                
                transform.position = teleportingPos;
                hasBennTeleported = true;
            }
                
            currentState = STATE.MOVE;
            DoAction = DoActionAppear;




        }

        public void SetModeMove()
        {
            
            DoAction = DoActionMove;
            hasBennTeleported = false;
            // Détection des murs
            if (Physics.Raycast(transform.position, transform.forward, 0.51f, 1 << LayerMask.NameToLayer("ground")))
            {
                Rotate(DIRECTION.RIGHT);
                SetModeWait();
            }
            
            
        }

        public void SetModeWait()
        {
            currentState = STATE.MOVE;
            SetPivot(DIRECTION.BACK);
            DoAction = DoActionWait;
        }

        // Sert à la fois pour le fall et pour le convoyeur
        public void SetModeTranslate()
        {
            if (currentState == STATE.CONVEYED) hasBeenConveyed = true;
             m_startPos = transform.position;
            m_endPos = new Vector3(transform.position.x + translationVector.x, transform.position.y + translationVector.y, transform.position.z + translationVector.z);
            
            
            
            DoAction = DoActionTranslate;
        }

        // Sert à la fois pour le Destroy, le Restart et le Teleport
        public void SetModeDisappear()
        {
            if (currentState == STATE.TELEPORTING)
            {
                currentState = STATE.CHANGING_POSITION;
                SfxManager.manager.PlaySfx("tp");
            }
                
            else if (currentState == STATE.DISAPPEARING)
            {
                currentState = STATE.DESTROYING;
                isInStepAction = true;
            }
            else if (currentState == STATE.RESTARTING)
            {
                currentState = STATE.CLEARING;
                isInStepAction = true;
            }

            DoAction = DoActionDisappear;
        }

        private void SetModeDestroy()
        {
            explosion.transform.position = transform.position;
            SfxManager.manager.PlaySfx("explo");
            Destroy(Instantiate(explosion),2);
            isInStepAction = false;
            CubeManager.instance.list.Remove(this);
            Destroy(this.gameObject);
            if (currentState == STATE.CLEARING) CubeManager.instance.SendRestartEvent();
        }




        #endregion

        #region Do Actions

        void DoActionVoid() { }

        void DoActionAppear()
        {
            transform.localScale = new Vector3(CubeManager.instance.ratio, CubeManager.instance.ratio, CubeManager.instance.ratio);
            
        }

        void DoActionMove()
        {
            float lDegreesPerFrame = (m_degrees * CubeManager.instance.ratio);
            transform.RotateAround(m_pivot, m_axis, lDegreesPerFrame - m_substractedAngle);
            m_substractedAngle = lDegreesPerFrame;
            
        }

        void DoActionWait()
        {
            AddMomentum();
            
        }

        void DoActionTranslate()
        {
            transform.position = Vector3.Lerp(m_startPos, m_endPos, CubeManager.instance.ratio);
        }

        void DoActionDisappear()
        {
            transform.localScale = new Vector3(CubeManager.instance.decreasingRatio, CubeManager.instance.decreasingRatio, CubeManager.instance.decreasingRatio);
        }

        #endregion
        #endregion

        #region VARIOUS PRIVATE METHODS

        // Met le Pivot dans la direction désirée
        void SetPivot(DIRECTION pivotPosition)
        {
            m_pivot = transform.position;
            if(pivotPosition == DIRECTION.FRONT) m_pivot += transform.forward * 0.5f;
            else if (pivotPosition == DIRECTION.BACK) m_pivot -= transform.forward * 0.5f;
            m_pivot -= transform.up * 0.5f;
            m_pivot += transform.right * 0.5f;
        }
        
        // Arrange la position, le scale et la rotation du cube à chaque tic
       public void LockPosition()
        {
            m_substractedAngle = 0;
            Quaternion lQuat = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            transform.rotation = lQuat;
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        }

        // Rajoute un effet d'élan au cube à la moitié du tic
        void AddMomentum()
        {
            if (CubeManager.instance.ratio >= 0.5f)
            {
                float lRatio = (CubeManager.instance.ratio - 0.5f) * 2f;
                float lDecreasingRatio = 1 - lRatio;
                float lDegreesPerFrame;
                if (lRatio < 0.5f) lDegreesPerFrame = (-30 * lRatio);
                else lDegreesPerFrame = (-30 * lDecreasingRatio);
                transform.RotateAround(m_pivot, m_axis, lDegreesPerFrame - m_substractedAngle);
                m_substractedAngle = lDegreesPerFrame;

                
            }
        }

        // Tourne le cube vers la droite ou la gauche
        public void Rotate(DIRECTION direction)
        {
            if (direction == DIRECTION.RIGHT)
            {
                transform.rotation *= Quaternion.AngleAxis(90, Vector3.up);
                m_axis = transform.right;
            }
            if (direction == DIRECTION.LEFT)
            {
                transform.rotation *= Quaternion.AngleAxis(-90, Vector3.up);
                m_axis = transform.right;
                SetPivot(DIRECTION.FRONT);
            }

        }

        public bool isFalling()
        {
            // Détection du Fall
            return !Physics.Raycast(transform.position, -transform.up, 0.55f, 1 << LayerMask.NameToLayer("ground"));
          
        }

        public bool isGoneFromLevel()
        {
            return !Physics.Raycast(transform.position, -transform.up, float.PositiveInfinity, 1 << LayerMask.NameToLayer("ground"));
        }


        //Détection de collision
        void OnTriggerEnter(Collider col)
        {
            
            if (col.gameObject.layer == LayerMask.NameToLayer("cube"))
            {
                CubeManager.instance.SendLooseEvent(this);
            }
        }
        #endregion
    }
}

