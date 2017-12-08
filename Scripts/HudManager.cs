using UnityEngine;
using System;
using System.Collections.Generic;

namespace RUSH
{

    /// <summary>
    /// 
    /// </summary>
    public class HudManager : MonoBehaviour
    {

        public GameObject Level1Hud;
        public GameObject Level2Hud;
        public GameObject Level3Hud;

        private static HudManager _instance;

        private LEVEL currentLevel;

        private int btn1Pool;
        private int btn2Pool;
        private int btn3Pool;
        private int btn4Pool;

        public ButtonEvent OnHudButtonClick;

        /// <summary>
        /// CONCERNE UNIQUEMENT LES BOUTONS D'ACTION  
        /// </summary>
        public static HudManager instance
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
                throw new Exception("Tentative de création d'une autre instance de HudManager alors que c'est un singleton.");
            }
            _instance = this;
            OnHudButtonClick = new ButtonEvent();
        }

        protected void Start()
        {
            ReflexionManager.instance.onActionFixed.AddListener(RemoveFromPool);
            ReflexionManager.instance.onActionRemoved.AddListener(AddToPool);
            MenuManager.instance.onLevelSelected.AddListener(DisplayHud);
            MenuManager.instance.onQuitLevel.AddListener(QuitLevel);
        }



        protected void OnDestroy()
        {
            _instance = null;
        }

        //Associe les bons boutons au bon niveau puis les instancie
        public void DisplayHud(LEVEL level)
        {
            currentLevel = level;
            if (level == LEVEL.EASY)
            {
                
                Level1Hud.SetActive(true);
                btn1Pool = Level1.instance.arrow.pool;
                btn2Pool = Level1.instance.conveyor1.pool;
                btn3Pool = Level1.instance.conveyor2.pool;
                Level1.instance.textBtn1.text = btn1Pool.ToString();
                Level1.instance.textBtn2.text = btn2Pool.ToString();
                Level1.instance.textBtn3.text = btn3Pool.ToString();
            }
                
            else if (level == LEVEL.MEDIUM)
            {
                Level2Hud.SetActive(true);
                btn1Pool = Level2.instance.arrow1.pool;
                btn2Pool = Level2.instance.arrow2.pool;
                btn3Pool = Level2.instance.stop.pool;
                btn4Pool = Level2.instance.switch1.pool;
                Level2.instance.textBtn1.text = btn1Pool.ToString();
                Level2.instance.textBtn2.text = btn2Pool.ToString();
                Level2.instance.textBtn3.text = btn3Pool.ToString();
                Level2.instance.textBtn4.text = btn4Pool.ToString();
            }
                
            else if (level == LEVEL.HARD)
            {
                Level3Hud.SetActive(true);
                btn1Pool = Level3.instance.arrow1.pool;
                btn2Pool = Level3.instance.arrow2.pool;
                btn3Pool = Level3.instance.arrow3.pool;
                btn4Pool = Level3.instance.stop.pool;
                Level3.instance.textBtn1.text = btn1Pool.ToString();
                Level3.instance.textBtn2.text = btn2Pool.ToString();
                Level3.instance.textBtn3.text = btn3Pool.ToString();
                Level3.instance.textBtn4.text = btn4Pool.ToString();
            }
                
        }

        //Clic sur les bouton en commencant par la gauche
        public void Button1()
        {
            if (btn1Pool <= 0)
            {
                SfxManager.manager.PlaySfx("error");
                return;
            }
                
            SfxManager.manager.PlaySfx("1");
            if (currentLevel == LEVEL.EASY) OnHudButtonClick.Invoke(Level1.instance.arrow,1);
            else if (currentLevel == LEVEL.MEDIUM) OnHudButtonClick.Invoke(Level2.instance.arrow1,1);
            else if (currentLevel == LEVEL.HARD) OnHudButtonClick.Invoke(Level3.instance.arrow1,1);
        }

        public void Button2()
        {
            if (btn2Pool <= 0)
            {
                SfxManager.manager.PlaySfx("error");
                return;
            }
            SfxManager.manager.PlaySfx("2");
            if (currentLevel == LEVEL.EASY) OnHudButtonClick.Invoke(Level1.instance.conveyor1,2);
            else if (currentLevel == LEVEL.MEDIUM) OnHudButtonClick.Invoke(Level2.instance.arrow2,2);
            else if (currentLevel == LEVEL.HARD) OnHudButtonClick.Invoke(Level3.instance.arrow2,2);
        }

        public void Button3()
        {
            if (btn3Pool <= 0)
            {
                SfxManager.manager.PlaySfx("error");
                return;
            }
            SfxManager.manager.PlaySfx("3");
            if (currentLevel == LEVEL.EASY) OnHudButtonClick.Invoke(Level1.instance.conveyor2,3);
            else if (currentLevel == LEVEL.MEDIUM) OnHudButtonClick.Invoke(Level2.instance.stop,3);
            else if (currentLevel == LEVEL.HARD) OnHudButtonClick.Invoke(Level3.instance.arrow3,3);
        }

        public void Button4()
        {
            
            if (btn4Pool <= 0)
            {
                SfxManager.manager.PlaySfx("error");
                return;
            }
            SfxManager.manager.PlaySfx("4");
            if (currentLevel == LEVEL.MEDIUM) OnHudButtonClick.Invoke(Level2.instance.switch1,4);
            else if (currentLevel == LEVEL.HARD) OnHudButtonClick.Invoke(Level3.instance.stop,4);
        }


        //Enleve 1 au nombre d'actions de ce type qu'on peut encore poser
        protected void RemoveFromPool(int BtnRef)
        {
            switch (BtnRef)
            {
                case 1:
                    --btn1Pool;
                    break;
                case 2:
                    --btn2Pool;
                    break;
                case 3:
                    --btn3Pool;
                    break;
                case 4:
                    --btn4Pool;
                    break;
            }
            UpdateText();
        }

        //Ajoute 1 au nombre d'actions de ce type qu'on peut encore poser
        protected void AddToPool(int BtnRef)
        {
            switch (BtnRef)
            {
                case 1:
                    ++btn1Pool;
                    break;
                case 2:
                    ++btn2Pool;
                    break;
                case 3:
                    ++btn3Pool;
                    break;
                case 4:
                    ++btn4Pool;
                    break;
            }
            UpdateText();
        }

        //Maj du texte à l'écran
        protected void UpdateText()
        {
            if (currentLevel == LEVEL.EASY)
            {


                Level1.instance.textBtn1.text = btn1Pool.ToString();
                Level1.instance.textBtn2.text = btn2Pool.ToString();
                Level1.instance.textBtn3.text = btn3Pool.ToString();
            }

            else if (currentLevel == LEVEL.MEDIUM)
            {

                Level2.instance.textBtn1.text = btn1Pool.ToString();
                Level2.instance.textBtn2.text = btn2Pool.ToString();
                Level2.instance.textBtn3.text = btn3Pool.ToString();
                Level2.instance.textBtn4.text = btn4Pool.ToString();
            }

            else if (currentLevel == LEVEL.HARD)
            {

                Level3.instance.textBtn1.text = btn1Pool.ToString();
                Level3.instance.textBtn2.text = btn2Pool.ToString();
                Level3.instance.textBtn3.text = btn3Pool.ToString();
                Level3.instance.textBtn4.text = btn4Pool.ToString();
            }
        }

        //Désactive le Hud quand on quitte le niveau 
        protected void QuitLevel()
        {
            if (currentLevel == LEVEL.EASY) Level1Hud.SetActive(false);
            else if (currentLevel == LEVEL.MEDIUM) Level2Hud.SetActive(false);
            else if (currentLevel == LEVEL.HARD) Level3Hud.SetActive(false);
        }
    }
}