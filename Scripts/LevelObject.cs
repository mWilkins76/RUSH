using UnityEngine;

namespace RUSH
{
    public enum COLOR
    {
        NONE = 0,
        BLUE = 1,
        RED = 2,
        PINK = 3,
        YELLOW = 4,
        GROUND = 5
    }

    /// <summary>
    /// CLASSE DE BASE DES ACTIONOBJECTS ET DES CUBES
    /// </summary>
    public class LevelObject : MonoBehaviour
    {

        #region INITIALIZATION

        #region Colors 
        public COLOR color;
        public Material blue;
        public Material pink;
        public Material red;
        public Material yellow;
        #endregion

        protected int m_cubeCounter = 0;

        public int cubeCounter
        {
            get { return m_cubeCounter; }

            set { m_cubeCounter = value; }
        }

        #endregion

        protected void SetColor()
        {
            if (color == COLOR.BLUE) GetComponent<Renderer>().material = blue;
            if (color == COLOR.PINK) GetComponent<Renderer>().material = pink;
            if (color == COLOR.RED) GetComponent<Renderer>().material = red;
            if (color == COLOR.YELLOW) GetComponent<Renderer>().material = yellow;
        }
    }
}