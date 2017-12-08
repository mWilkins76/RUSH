using UnityEngine;
using System;
using UnityEngine.Events;
namespace RUSH
{

    public enum GAME_STATE
    {
        MENU = 0,
        PLAY = 1,
        PAUSE = 2,
        REFLECTION = 3
    }

    public enum LEVEL
    {
        EASY = 0,
        MEDIUM = 1,
        HARD = 2
    }

    public class GameManager : MonoBehaviour
    {

        private static GameManager _instance;

        private bool allSpawnersEmpty = false;
        private bool noMoreCubeInLevel = false;
        public GAME_STATE currentState;

        public GameObject level1Fab;
        public GameObject level2Fab;
        public GameObject level3Fab;

        public GameObject smoke;

        private GameObject instanceLevel;

        public bool hasGameBegun = false;

        public GameObject camera;

        public GameObject ReflectionManagerPointer;

        public LEVEL currentLevel;

        public static GameManager instance
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
                throw new Exception("Tentative de création d'une autre instance de GameManager alors que c'est un singleton.");
            }
            _instance = this;

            currentState = GAME_STATE.MENU;
        }

        protected void Start()
        {
            ActionObjectsManager.instance.onAllSpawnersEmpty.AddListener(SpawnersCheck);
            CubeManager.instance.onNoMoreCube.AddListener(CubeCheck);
            CubeManager.instance.onGameLost.AddListener(Loose);
            CubeManager.instance.onReadyToRestart.AddListener(ReflectionPhase);
            MenuManager.instance.onPlay.AddListener(Play);
            MenuManager.instance.onPause.AddListener(Pause);
            MenuManager.instance.onLevelSelected.AddListener(InstanciateLevel);
            MenuManager.instance.onQuitLevel.AddListener(QuitLevel);

            MusicLoopsManager.manager.PlayMusic(0);
        }

        protected void Update()
        {
            //Action ou non le Script de reflexion s'il est en state Reflection
            if (currentState == GAME_STATE.REFLECTION)
            {
                if (Input.GetKeyDown("space")) MenuManager.instance.Retry();
                ReflectionManagerPointer.transform.gameObject.GetComponent<ReflexionManager>().enabled = true;
            }
                
            else ReflectionManagerPointer.transform.gameObject.GetComponent<ReflexionManager>().enabled = false;

            if (currentState == GAME_STATE.MENU) smoke.SetActive(true);
            else smoke.SetActive(false);
        }

        protected void OnDestroy()
        {
            _instance = null;
        }

        
        protected void Win()
        {
            SfxManager.manager.PlaySfx("win");
            currentState = GAME_STATE.PAUSE;
            MenuManager.instance.solvingHudPanel.SetActive(false);
            MenuManager.instance.winPanel.SetActive(true);
        }

        protected void Loose(Cube cube)
        {
            cube.halo.enabled = true;
            SfxManager.manager.PlaySfx("over");
            currentState = GAME_STATE.PAUSE;
            MenuManager.instance.solvingHudPanel.SetActive(false);
            MenuManager.instance.gameOverPanel.SetActive(true);
        }

        protected void Pause()
        {
            currentState = GAME_STATE.PAUSE;
        }

        protected void Play()
        {
            currentState = GAME_STATE.PLAY;
        }

        // Appellé quand tout les spawner sont vides
        protected void SpawnersCheck()
        {
            allSpawnersEmpty = true;
            if (allSpawnersEmpty && noMoreCubeInLevel) Win();
        }

        // Appellé quand il n'y a plus de cubes dans le niveau;
        protected void CubeCheck()
        {
            noMoreCubeInLevel = true;
            if (allSpawnersEmpty && noMoreCubeInLevel) Win();
        }


        protected void InstanciateLevel(LEVEL selection)
        {
            currentLevel = selection;
            camera.GetComponent<SatelliteCamera>().SetAzimuth();
            camera.GetComponent<SatelliteCamera>().enabled = true;

            currentState = GAME_STATE.REFLECTION;
            
            if (selection == LEVEL.EASY) instanceLevel = Instantiate(level1Fab);
            else if (selection == LEVEL.MEDIUM) instanceLevel = Instantiate(level2Fab);
            else if (selection == LEVEL.HARD) instanceLevel = Instantiate(level3Fab);
        }

        //Déclanche la phase de reflexion
        protected void ReflectionPhase()
        {
            currentState = GAME_STATE.REFLECTION;
            MenuManager.instance.recflectionHudPanel.SetActive(true);
            MenuManager.instance.solvingHudPanel.SetActive(false);
            MenuManager.instance.gameOverPanel.SetActive(false);
        }

        //revient au menu
        protected void QuitLevel()
        {
            currentState = GAME_STATE.MENU;
            noMoreCubeInLevel = false;
            allSpawnersEmpty = false;
            hasGameBegun = false;
            camera.GetComponent<SatelliteCamera>().enabled = false;
            Destroy(instanceLevel);

        }
    }
}