using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

namespace RUSH
{

    
    
    public class Spawner : ActionObject
    {
        #region INITIALIZATION

        #region Private variables
        [SerializeField]
        private bool[] m_spawnRate;
        private int m_ticCounter = 0;


        public GameObject redParticle;
        public GameObject blueParticle;
        public GameObject yellowParticle;
        public GameObject pinkParticle;
        private GameObject directionParticle;
        
        #endregion

        #endregion

        #region LIFE CYCLE

        new void Start()
        {
            base.Start();
            ActionObjectsManager.instance.spawnerList.Add(this);
            switch (color)
            {
                case COLOR.RED:
                    directionParticle = redParticle;
                    break;
                case COLOR.BLUE:
                    directionParticle = blueParticle;
                    break;
                case COLOR.YELLOW:
                    directionParticle = yellowParticle;
                    break;
                case COLOR.PINK:
                    directionParticle = pinkParticle;
                    break;
                default:
                    directionParticle = redParticle;
                    break;
            }
            directionParticle.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            directionParticle.transform.rotation = transform.rotation;
            Instantiate(directionParticle,transform);
        }
        #endregion

        #region VARIOUS PUBLIC METHODS
        public void SpawningProcess()
        {
            if (cubeCounter == ActionObjectsManager.CUBE_NUMBER)
            {
                ActionObjectsManager.instance.emptySpawnerCounter++;
                return;
            }

            if (currentState == ACTION_STATE.RESTART) return;
               
            if (m_ticCounter >= m_spawnRate.Length) m_ticCounter = 0;
            if (m_spawnRate[m_ticCounter++])
            {
                cubeCounter++;
                ActionObjectsManager.instance.SendSpawnSignal(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation, color);
                
            }
        }
        #endregion
    }
}

