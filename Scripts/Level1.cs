using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace RUSH
{

    /// <summary>
    /// CLASSE DE REFERENCE ATTACHE AUX BOUTONS HUD
    /// </summary>
    public class Level1 : LevelHud
    {

        private static Level1 _instance;

        /// <summary>
        /// instance unique de la classe     
        /// </summary>
        public static Level1 instance
        {
            get
            {
                return _instance;
            }
        }


        public ActionStruct arrow;
        public ActionStruct conveyor1;
        public ActionStruct conveyor2;
        

        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de Level1 alors que c'est un singleton.");
            }
            _instance = this;

            
            arrow.type = ACTION_STATE.ARROW;
            arrow.pool = 1;
            arrow.direction = 90;
            

            conveyor1.type = ACTION_STATE.CONVEYOR;
            conveyor1.pool = 6;
            conveyor1.direction = 0;

            conveyor2.type = ACTION_STATE.CONVEYOR;
            conveyor2.pool = 6;
            conveyor2.direction = 90;
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