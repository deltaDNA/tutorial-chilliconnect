using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using ChilliConnect;
using DeltaDNA; 


public class GameManager : MonoBehaviour {


    public ChilliConnectSdk chilliConnect;
    public string chilliConnectId = null;
    private string chilliConnectSecret = null;



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
        // Start ChilliConnect SDK, Login, then start deltaDNA SDK
        StartSDKs();

        // These are for pulsing the start button size and alpha 
        InitialScale = transform.localScale;
        FinalScale = new Vector3(InitialScale.x + 0.04f,
                                 InitialScale.y + 0.04f,
                                 InitialScale.z);
        sourceColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        targetColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        // Show the Start button       
        txtStart.gameObject.SetActive(true);
        bttnStart.gameObject.SetActive(true);
        readyToStart = true;

        // A simple debug console in game
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



    private void StartSDKs()
    {
        // Start the ChilliConnect SDK
        ChilliConnectInit();

        // Login to ChilliConnect and start deltaDNA SDK
        LogIn(chilliConnectId, chilliConnectSecret);
    }




    // Start the ChilliConnect SDK, creating a player if one is not stored on the client.
    private void ChilliConnectInit()
    {
        // InitialiseChilliConnect SDK with our Game Token
        chilliConnect = new ChilliConnectSdk("6PvaW0XKPZF3wUTOavDPwcLQUho9DQdS",true);

        // Create a Player and store the ChilliConnectId if we don't already have one
        if (!PlayerPrefs.HasKey("ChilliConnectId") || !PlayerPrefs.HasKey("ChilliConnectSecret"))
        {
            var createPlayerRequest = new CreatePlayerRequestDesc();

            // Create Player Account
            chilliConnect.PlayerAccounts.CreatePlayer(createPlayerRequest,
                (CreatePlayerRequest request, CreatePlayerResponse response) =>
                {
                    Debug.Log("Create Player successfull : " + response.ChilliConnectId);

                    PlayerPrefs.SetString("ChilliConnectId", response.ChilliConnectId);
                    PlayerPrefs.SetString("ChilliConnectSecret", response.ChilliConnectSecret);

                    chilliConnectId = response.ChilliConnectId;
                    chilliConnectSecret = response.ChilliConnectSecret;                    
                },
                (CreatePlayerRequest request, CreatePlayerError error) =>
                {
                    Debug.Log("An error occurred Creating Player : " + error.ErrorDescription);
                }
            );
        }
        else
        {
            chilliConnectId = PlayerPrefs.GetString("ChilliConnectId");
            chilliConnectSecret = PlayerPrefs.GetString("ChilliConnectSecret");            
        }

    }


    // Login to ChilliConnect, then start deltaDNA SDK
    private void LogIn(string chilliConnectId, string chilliConnectSecret)
    {
        var loginRequest = new LogInUsingChilliConnectRequestDesc(chilliConnectId, chilliConnectSecret);
        chilliConnect.PlayerAccounts.LogInUsingChilliConnect(loginRequest,
            (LogInUsingChilliConnectRequest request, LogInUsingChilliConnectResponse response) =>
            {
                Debug.Log("Login using ChilliConnect OK");

                // Start the deltaDNA SDK using the chilliConnectIs as the deltaDNA userID
                DeltaDNAInit(chilliConnectId);
            }, 
            (LogInUsingChilliConnectRequest request, LogInUsingChilliConnectError error) =>
            {
                Debug.Log("An error occurred during ChilliConnect Player Login : " + error.ErrorDescription + "\n Data : " + error.ErrorData);
                Debug.Log("Quitting");
                Application.Quit();
            }
        );
    }

   // Setup a few things and start the deltaDNA SDK
    private void DeltaDNAInit(string chilliConnectId)
    {
        // Configure some things
        DDNA.Instance.SetLoggingLevel(DeltaDNA.Logger.Level.DEBUG);
        DDNA.Instance.ClientVersion = Application.version;

        // Event Triggered Campaigns configuration settings
        DDNA.Instance.Settings.MultipleActionsForEventTriggerEnabled = true;
        DDNA.Instance.NotifyOnSessionConfigured(true);
        DDNA.Instance.OnSessionConfigured += (bool cachedConfig) => GetGameConfig(cachedConfig);

        // Register Handlers for Event Triggered Campaign responses
        DDNA.Instance.Settings.DefaultGameParameterHandler = new GameParametersHandler(gameParameters =>
        {
            MyGameParameterHandler(gameParameters);
        });
        DDNA.Instance.Settings.DefaultImageMessageHandler = new ImageMessageHandler(DDNA.Instance, imageMessage =>
        {
            MyImageMessageHandler(imageMessage);
        });


        // Start the SDK with the chilliConnectId to ensure deltaDNA.userID and ChilliConnectId are the same.
        DDNA.Instance.StartSDK(chilliConnectId);
    }




    private void GetGameConfig(bool cachedConfig)
    {
        Debug.Log("Received deltaDNA configuration");
    }




    private void MyGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        Debug.Log("Received deltaDNA gameParameters from event triggered campaign" + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));
    }




    private void MyImageMessageHandler(ImageMessage imageMessage)
    {
        // Add Handler for Image Message 'dismiss' action
        imageMessage.OnDismiss += (ImageMessage.EventArgs obj) =>
        {
            Debug.Log("Image Message dismissed by " + obj.ID);
            // NB We won't process any game parameters if the player dimisses the Image Message
        };

        // Add Handler for Image Message 'action' action
        imageMessage.OnAction += (ImageMessage.EventArgs obj) =>
        {
            Debug.Log("Image Message actioned by " + obj.ID + " with command " + obj.ActionValue);

            // Process any parameters received with the Image Message
            if(imageMessage.Parameters != null)
            {
                MyGameParameterHandler(imageMessage.Parameters);
            }
        };

        // The image message is cached, it will show instantly
        imageMessage.Show();
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
