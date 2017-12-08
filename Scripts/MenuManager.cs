using UnityEngine;
using UnityEngine.Events;
using System;

namespace RUSH
{

    /// <summary>
    /// 
    /// </summary>
    public class MenuManager : MonoBehaviour
    {

        private static MenuManager _instance;
        
        public static MenuManager instance
        {
            get
            {
                return _instance;
            }
        }

        public UnityEvent onPlay;
        public UnityEvent onPause;
        public UnityEvent onRestart;
        public LevelEvent onLevelSelected;
        public UnityEvent onQuitLevel;
        public UnityEvent onClearActions;

        public GameObject titleCardPanel;
        public GameObject levelSelectionPanel;
        public GameObject recflectionHudPanel;
        public GameObject solvingHudPanel;
        public GameObject gameOverPanel;
        public GameObject winPanel;
        public GameObject pausePanelSolving;
        public GameObject pausePanelReflection;


        #region LIFE CYCLE
        protected void Awake()
        {
            onPlay = new UnityEvent();
            onPause = new UnityEvent();
            onRestart = new UnityEvent();
            onLevelSelected = new LevelEvent();
            onQuitLevel = new UnityEvent();
            onClearActions = new UnityEvent();

            if (_instance != null)
            {
                throw new Exception("Tentative de création d'une autre instance de MenuManager alors que c'est un singleton.");
            }
            _instance = this;


        }



        protected void OnDestroy()
        {
            _instance = null;
        }
        #endregion


        //Quand on clic sur LAUNCH 
        public void Play()
        {
            SfxManager.manager.PlaySfx("6");
            MusicLoopsManager.manager.PlayMusic(MusicType.solveMusic);
            recflectionHudPanel.SetActive(false);
            solvingHudPanel.SetActive(true);
            onPlay.Invoke();
        }

        //Menu de Pause qui se déclenche pendant la phase Solving
        public void PauseSolving()
        {
            SfxManager.manager.PlaySfx("7");
            onPause.Invoke();
            pausePanelSolving.SetActive(true);
            solvingHudPanel.SetActive(false);
        }

        //Menu de Pause qui se déclenche pendant la phase Reflexion
        public void PauseReflection()
        {
            SfxManager.manager.PlaySfx("7");
            onPause.Invoke();
            pausePanelReflection.SetActive(true);
            recflectionHudPanel.SetActive(false);
        }

        //Quand on clic sur Play dans la TitleCard
        public void GoToLevelSelection()
        {
            SfxManager.manager.PlaySfx("1");
            titleCardPanel.SetActive(false);
            levelSelectionPanel.SetActive(true);
        }

        //Bouton de Selection de Level
        public void EasyLevel()
        {
            SfxManager.manager.PlaySfx("2");
            SetLevel();
            onLevelSelected.Invoke(LEVEL.EASY);
            
        }

        public void MediumLevel()
        {
            SfxManager.manager.PlaySfx("3");
            SetLevel();
            onLevelSelected.Invoke(LEVEL.MEDIUM);
            
        }

        public void HardLevel()
        {
            SfxManager.manager.PlaySfx("4");
            SetLevel();
            onLevelSelected.Invoke(LEVEL.HARD);
            
        }

        //Appellé pour instancier le panel de level
        private void SetLevel()
        {
            MusicLoopsManager.manager.PlayMusic(MusicType.thinkMusic);
            recflectionHudPanel.SetActive(true);
            levelSelectionPanel.SetActive(false);
        }

        //Quand on veut revenir à la phase de reflexion
        public void Restart()
        {
            SfxManager.manager.PlaySfx("1");
            MusicLoopsManager.manager.PlayMusic(MusicType.thinkMusic);
            GameManager.instance.currentState = GAME_STATE.PLAY;
            pausePanelSolving.SetActive(false);
            onRestart.Invoke();
        }


        //Quand on veut recommencer tout le niveau
        public void Retry()
        {
            SfxManager.manager.PlaySfx("5");
            MusicLoopsManager.manager.PlayMusic(MusicType.thinkMusic);
            onClearActions.Invoke();
            GameManager.instance.currentState = GAME_STATE.REFLECTION;
            pausePanelReflection.SetActive(false);
            recflectionHudPanel.SetActive(true);
            HudManager.instance.DisplayHud(GameManager.instance.currentLevel);
        }

        //Quand on veut quitter le niveau
        public void QuitLevel()
        {
            SfxManager.manager.PlaySfx("6");
            MusicLoopsManager.manager.PlayMusic(0);
            onQuitLevel.Invoke();
            gameOverPanel.SetActive(false);
            winPanel.SetActive(false);
            pausePanelSolving.SetActive(false);
            pausePanelReflection.SetActive(false);
            titleCardPanel.SetActive(true);
        }



    }
}