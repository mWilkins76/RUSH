using UnityEngine;
using System;

namespace RUSH
{

    /// <summary>
    /// 
    /// </summary>
    public class ReflexionManager : MonoBehaviour
    {

        


        public GameObject arrowGo;
        public GameObject conveyorGo;
        public GameObject stopGo;
        public GameObject switchGo;

        private ActionStruct actionSelected;

        private GameObject previousHit;
        private GameObject currentSelection;

        public PoolEvent onActionFixed;
        public PoolEvent onActionRemoved;

        private int currentBtnRef;

        private static ReflexionManager _instance;

        /// <summary>
        /// instance unique de la classe     
        /// </summary>
        public static ReflexionManager instance
        {
            get
            {
                return _instance;
            }
        }

        protected void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de ReflexionManager alors que c'est un singleton.");
            }
            _instance = this;
            onActionFixed = new PoolEvent();
            onActionRemoved = new PoolEvent();
        }

        protected void Start()
        {
            

            HudManager.instance.OnHudButtonClick.AddListener(UpdateSelection);
        }

        protected void Update()
        {
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //Quand la souris touche un ActionObject(layer nommé "ground"), si le joueur clic et que l'ActionObject est bien de state Ground, alors 
            //il sera fixé et prendra le state et l'orientation référencé dans le bouton du Hud
            //Si l'objet est déjà fixé et qu'on clic dessus, on enlève son enfant (la plaque graphique) et on le remet en state Ground avec une orientation de base
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, 1 << LayerMask.NameToLayer("ground")))
            {

                if (Input.GetMouseButtonDown(0)&& hit.transform.gameObject.GetComponent<ActionObject>().currentState != ACTION_STATE.GROUND)
                {
                    if (!hit.transform.gameObject.GetComponent<ActionObject>().isFixed)
                    {
                        SfxManager.manager.PlaySfx("Button");
                        hit.transform.gameObject.GetComponent<ActionObject>().isFixed = true;
                        hit.transform.gameObject.GetComponent<ActionObject>().btnRef = currentBtnRef;
                        onActionFixed.Invoke(currentBtnRef);
                        currentSelection = null;
                    }
                    else
                    {
                        SfxManager.manager.PlaySfx("7");
                        hit.transform.gameObject.GetComponent<ActionObject>().isFixed = false;
                        hit.transform.gameObject.GetComponent<ActionObject>().currentState = ACTION_STATE.GROUND;
                        foreach (Transform child in hit.transform)
                        {
                            Destroy(child.gameObject);
                        }
                        hit.transform.rotation = Quaternion.identity;
                        onActionRemoved.Invoke(hit.transform.gameObject.GetComponent<ActionObject>().btnRef);
                    }
                        
                    
                }
                    

                if (hit.transform.gameObject.GetComponent<ActionObject>().currentState == ACTION_STATE.GROUND && currentSelection != null)
                {
                    DestroyPreviousHit();
                    
                    previousHit = hit.transform.gameObject;
                    hit.transform.gameObject.GetComponent<ActionObject>().currentState = actionSelected.type;
                    currentSelection.transform.position = new Vector3(hit.transform.position.x, hit.transform.position.y + 0.55f, hit.transform.position.z);
                    GameObject actionObject = Instantiate(currentSelection);
                    actionObject.transform.parent = hit.transform;
                    hit.transform.rotation *= Quaternion.AngleAxis(actionSelected.direction, Vector3.up);

                }

            }

            else DestroyPreviousHit();



        }

        protected void OnDestroy()
        {
            _instance = null;
        }

        //Détruit l'objet référencé à l'ancienne position de la souris
        private void DestroyPreviousHit()
        {
            if (previousHit != null && !previousHit.GetComponent<ActionObject>().isFixed)
            {
                previousHit.GetComponent<ActionObject>().currentState = ACTION_STATE.GROUND;
                foreach (Transform child in previousHit.transform)
                {
                    Destroy(child.gameObject);
                }
                previousHit.transform.rotation = Quaternion.identity;
            }
        }


        //Change l'action à poser
        private void UpdateSelection(ActionStruct HudBtn, int BtnRef)
        {
            actionSelected = HudBtn;
            currentBtnRef = BtnRef;
            switch (actionSelected.type)
            {
                case ACTION_STATE.ARROW:
                    currentSelection = arrowGo;
                    break;

                case ACTION_STATE.CONVEYOR:
                    currentSelection = conveyorGo;
                    break;

                case ACTION_STATE.STOP:
                    currentSelection = stopGo;
                    break;

                case ACTION_STATE.SWITCH:
                    currentSelection = switchGo;
                    break;
            }
        }
    }
}