using UnityEngine;
using System;
using System.Collections.Generic;

namespace RUSH
{

    /// <summary>
    /// CLASSE DE REFERENCE ATTACHE AUX BOUTONS HUD
    /// </summary>
    public class Level2 : LevelHud
    {

        private static Level2 _instance;

        /// <summary>
        /// instance unique de la classe     
        /// </summary>
        public static Level2 instance
        {
            get
            {
                return _instance;
            }
        }

        public ActionStruct arrow1;
        public ActionStruct arrow2;
        public ActionStruct stop;
        public ActionStruct switch1;

        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de Level2 alors que c'est un singleton.");
            }
            _instance = this;

            arrow1.type = ACTION_STATE.ARROW;
            arrow1.pool = 1;
            arrow1.direction = 90;

            arrow2.type = ACTION_STATE.ARROW;
            arrow2.pool = 1;
            arrow2.direction = -90;

            stop.type = ACTION_STATE.STOP;
            stop.pool = 8;
            stop.direction = 0;

            switch1.type = ACTION_STATE.SWITCH;
            switch1.pool = 2;
            switch1.direction = 0;
        }

        protected void Start()
        {
            
        }



        protected void OnDestroy()
        {
            _instance = null;
        }
    }
}