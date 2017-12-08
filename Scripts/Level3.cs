using UnityEngine;
using System;
using System.Collections.Generic;

namespace RUSH
{

    /// <summary>
    /// CLASSE DE REFERENCE ATTACHE AUX BOUTONS HUD
    /// </summary>
    public class Level3 : LevelHud
    {

        private static Level3 _instance;

        /// <summary>
        /// instance unique de la classe     
        /// </summary>
        public static Level3 instance
        {
            get
            {
                return _instance;
            }
        }

        

        public ActionStruct arrow1;
        public ActionStruct arrow2;
        public ActionStruct arrow3;
        public ActionStruct stop;

        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de Level3 alors que c'est un singleton.");
            }
            _instance = this;

            arrow1.type = ACTION_STATE.ARROW;
            arrow1.pool = 1;
            arrow1.direction = 0;

            arrow2.type = ACTION_STATE.ARROW;
            arrow2.pool = 1;
            arrow2.direction = 90;

            arrow3.type = ACTION_STATE.ARROW;
            arrow3.pool = 1;
            arrow3.direction = 180;

            stop.type = ACTION_STATE.STOP;
            stop.pool = 1;
            stop.direction = 0;
        }

        protected void Start()
        {

        }

        protected void Update()
        {

        }

        protected void OnDestroy()
        {
            _instance = null;
        }
    }
}