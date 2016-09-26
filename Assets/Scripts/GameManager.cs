using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    [HideInInspector]
    public bool playersTurn = true;


    private int level = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private Text levelText;
    private GameObject levelImage;
    private bool doingSetup;

    // Use this for initialization
    void Awake() {

        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();

    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        Debug.Log("TESING");
        level++;
        InitGame();
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;

    }
    public void GameOver() {
        levelText.text = "After" + level + " days, you starve";
        levelImage.SetActive(true);
        enabled = false;
    }
    void InitGame() {

        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);


        enemies.Clear();
        boardScript.SetupScene(level);
    }

    private void HideLevelImage() {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    // Update is called once per frame
    void Update() {
        if(playersTurn || enemiesMoving || doingSetup) {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script) {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies() {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);

        if(enemies.Count == 0) {
            yield return new WaitForSeconds(turnDelay);
        }

        for(int i = 0; i < enemies.Count; i++) {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
            
        }

        playersTurn = true;
        enemiesMoving = false;
    }
}
