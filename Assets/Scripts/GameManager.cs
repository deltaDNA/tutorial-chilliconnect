using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;



public class GameManager : MonoBehaviour {


    public PlayerManager player;
    public GameObject snakePrefab;
    private Snake snake; 
    private GameConsole console; 
    public Text txtStart;
    public Text txtGameOver;
    public Button bttnStart;
    


    public List<int?> foodPerLevel = new List<int?>() { 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48 };
    const int DEFAULT_FOOD_SPAWN = 6;
    public int foodSpawn ;
    public int foodLevelOveride = 0;

    // Start Button Size and Color
    private Color sourceColor;
    private Color targetColor;
    private Vector3 InitialScale;
    private Vector3 FinalScale;
    bool readyToStart = false;
    bool waiting = false;

    private void Start()
    {
        // These are for pulsing the start button size and alpha 
        InitialScale = transform.localScale;
        FinalScale = new Vector3(InitialScale.x + 0.04f,
                                 InitialScale.y + 0.04f,
                                 InitialScale.z);
        sourceColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        targetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);


        
        txtStart.gameObject.SetActive(true);
        bttnStart.gameObject.SetActive(true);
        readyToStart = true;

        console = GameObject.FindObjectOfType<GameConsole>();
        console.UpdateConsole();
    }

    private void Update()
    {
        if (readyToStart)
        {
            // Pulse the start button size and alpha
            bttnStart.image.color = Color.Lerp(sourceColor, targetColor, Mathf.PingPong(Time.time, 1.2f));
            bttnStart.transform.localScale = Vector3.Lerp(InitialScale, FinalScale, Mathf.PingPong(Time.time, 1.2f));

        }
    }



    public void StartLevel(int levelNo)
    {
        // Player starts level
        player.SetLevel(1);
        foodSpawn = GetFoodSpawn(player.playerLevel);

        player.UpdatePlayerStatistics();

        txtGameOver.gameObject.SetActive(false);
        txtStart.gameObject.SetActive(false);
        bttnStart.gameObject.SetActive(false);
        readyToStart = false;

        // Spawn new Snake 
        Vector3 pos = new Vector3(0, 0, -1);
        snake = Instantiate(snakePrefab, pos, Quaternion.identity).GetComponent<Snake>();

        LevelStarted();
        
    }
    public void PlayerDied()
    {
        LevelFailed();

        txtGameOver.gameObject.SetActive(true);
        txtStart.gameObject.SetActive(true);
        bttnStart.gameObject.SetActive(true);
        readyToStart = true; 

    }
    public void LevelUp()
    {
        LevelCompleted();

        player.playerLevel++;
        
        Debug.Log("Level Up - playerLevel " + player.playerLevel);

        player.UpdatePlayerStatistics();

        foodSpawn = GetFoodSpawn(player.playerLevel);
        LevelStarted();
    }

    public void LevelStarted()
    {
    }

    public void LevelCompleted()
    { 
    }

    public void LevelFailed()
    { 
    }

    
    public int GetFoodSpawn(int level)
    {
        int n = DEFAULT_FOOD_SPAWN;

        if (foodLevelOveride > 0)
        {
            n = foodLevelOveride;
        }
        else if (foodPerLevel.Count > player.playerLevel && foodPerLevel[player.playerLevel - 1] != null)
        {
            n = (int)foodPerLevel[player.playerLevel - 1];
        }

        return n;
    }
}
