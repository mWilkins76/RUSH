using UnityEngine;

namespace RUSH
{

    /// <summary>
    /// 
    /// </summary>
    /// 
    

    public class CameraClick : MonoBehaviour
    {
        public GameObject sparks;
        private Vector3 mousePos;
        private Vector3 worldPos;

        protected void Start()
        {

        }

        // Envoi de particules à chaque clic dans les menus
        protected void Update()
        {
            if (Input.GetMouseButton(0) && (GameManager.instance.currentState == GAME_STATE.MENU || GameManager.instance.currentState == GAME_STATE.PAUSE))
            {
                mousePos = Input.mousePosition;
                mousePos.z = 1.5f;
                worldPos = GetComponent<Camera>().ScreenToWorldPoint(mousePos);
                Destroy(Instantiate(sparks, worldPos, Quaternion.identity),0.5f);
            }
        }
    }
}