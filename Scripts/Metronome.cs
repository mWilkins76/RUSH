using UnityEngine;
using UnityEngine.Events;
using System;

namespace RUSH
{

    /// <summary>
    /// 
    /// </summary>
    public class Metronome : MonoBehaviour
    {
        #region INITIALIZATION

        #region Events
        public UnityEvent onTic;
        public UnityEvent onTac;
        #endregion

        #region Private Variables
        private static Metronome _instance;
        [SerializeField]
        private int m_tempo = 20;
        #endregion

        #region ¨Public Variables
        public static Metronome instance
        {
            get
            {
                return _instance;
            }
        }
        public int Tempo
        {
            get
            {
                return m_tempo;
            }
        }
        #endregion

        #endregion

        #region LIFE CYCLE
        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de Metronome alors que c'est un singleton.");
            }
            _instance = this;


            onTic = new UnityEvent();
            onTac = new UnityEvent();
        }
        


        protected void Update()
        {
            if (CustomTimer.instance.elapsedTime == m_tempo)
            {
                //j'appelle deux events pour attribuer le comportement au cube avant qu'il lance son cycle
                CustomTimer.instance.elapsedTime = 0;
                onTic.Invoke();
                onTac.Invoke();
                

            }
        }

        protected void OnDestroy()
        {
            _instance = null;
        }

        #endregion
    }
}