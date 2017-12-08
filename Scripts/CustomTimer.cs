using UnityEngine;
using System;

namespace RUSH
{

    /// <summary>
    /// 
    /// </summary>
    public class CustomTimer : MonoBehaviour
    {
        #region INITIALIZATION

        #region Private Variables
        private static CustomTimer _instance;
        private int m_deltaTime = 1;
        private float m_elapsedTime;

        #endregion

        #region ¨Public Variables
        
        public static CustomTimer instance
        {
            get
            {
                return _instance;
            }
        }
        public float elapsedTime
        {
            get { return m_elapsedTime; }

            set { m_elapsedTime = value; }
        }
        #endregion

        #endregion

        #region LIFE CYCLE
        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de CustomTimer alors que c'est un singleton.");
            }
            _instance = this;
        }


        protected void Update()
        {
           if(GameManager.instance.currentState == GAME_STATE.PLAY) m_elapsedTime += m_deltaTime;    
        }

        protected void OnDestroy()
        {
            _instance = null;
        }
        #endregion

        
    }
}





