using UnityEngine;

namespace RUSH
{

    /// <summary>
    /// SPRITES DU HUD QUI TOURNE EN FONCTION DU Y DE LA CAMERA
    /// </summary>
    public class RotatingButton : MonoBehaviour
    {
        public GameObject cam;
        float baseAngle;

        protected void Start()
        {
            baseAngle = transform.rotation.z;
        }

        protected void Update()
        {
            transform.rotation = Quaternion.Euler(0, 0,baseAngle +  cam.transform.rotation.eulerAngles.y);
        }
    }
}