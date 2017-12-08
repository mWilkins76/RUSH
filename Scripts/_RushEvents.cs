using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using UnityEngine;

namespace RUSH
{
        public struct ActionStruct
    {
        public ACTION_STATE type;
        public int pool;
        public int direction;
    }

        [System.Serializable]
        public class ActionEvent : UnityEvent<RaycastHit, ActionObject> { }

        [System.Serializable]
        public class CubeEvent : UnityEvent<Cube> { }

        [System.Serializable]
        public class PositioningEvent : UnityEvent<Vector3, Quaternion, COLOR> { }

        [System.Serializable]
        public class LevelEvent : UnityEvent<LEVEL> { }

        [System.Serializable]
        public class ButtonEvent : UnityEvent<ActionStruct, int> { }
        
        [System.Serializable]
        public class PoolEvent : UnityEvent<int> { }


}
