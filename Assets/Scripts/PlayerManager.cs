using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChilliConnect;

//using DeltaDNA;

public class PlayerManager : MonoBehaviour {

    public int playerLevel = 1 ;
    public int playerHealth = 100 ;
    public int playerCoins = 100 ;

    public int foodRemaining = 0; 
    public HudManager hud;

    public enum State { DEAD, ALIVE };
    public State state = State.DEAD; 

    private ChilliConnectSdk chilliConnect; 

	// Use this for initialization
	void Start () {

        hud = GameObject.FindObjectOfType<HudManager>();          
        UpdatePlayerStatistics();
    }


    // Fetch Currency from ChilliConnect
    public void FetchCurrency(ChilliConnectSdk chilliConnect)
    {
        this.chilliConnect = chilliConnect; 
        Debug.Log("Fetching currency");
        
        chilliConnect.Economy.GetCurrencyBalance(new GetCurrencyBalanceRequestDesc()
            , (request, response) =>
            {
                // Handle Currency Response from ChilliConnect
                Debug.Log("Currency fetched: ");
                foreach (var item in response.Balances)
                {
                    switch (item.Key)
                    {
                        case "COINS":
                            playerCoins = item.Balance;
                            break;
                        case "USERLEVEL":
                            playerLevel = item.Balance;
                            break;
                    }
                }
                UpdatePlayerStatistics();
            }
            , (request, error) => Debug.LogError(error.ErrorDescription));
    }



    public void SetLevel(int l)
    {       
        chilliConnect.Economy.SetCurrencyBalance(new SetCurrencyBalanceRequestDesc("USERLEVEL",l)
            , (request,response) => Debug.Log("Set UserLevel " + l) 
            , (request, error) => Debug.LogError(error.ErrorDescription)
        );

        playerLevel = l;
        UpdatePlayerStatistics();
    }

    public void SetHealth(int h)
    {
        // TODO Save to cloud
        playerHealth = h;
        UpdatePlayerStatistics();

    }

    public void SpendCoins(int c)
    {
        SetCoins(playerCoins - c); 
    }

    public void receiveCoins(int c)
    {
        SetCoins(playerCoins + c);
    }

    private void SetCoins(int c)
    {
        chilliConnect.Economy.SetCurrencyBalance(new SetCurrencyBalanceRequestDesc("COINS", c)
            , (request, response) => Debug.Log("Set UserCoins " + c)
            , (request, error) => Debug.LogError(error.ErrorDescription)
        );
        playerCoins = c;
        UpdatePlayerStatistics();

    }

    public void SetFoodRemaining(int f)
    {
        foodRemaining = f;
        UpdatePlayerStatistics();
    }

    public void UpdatePlayerStatistics()
    { 
        hud.SetCoins(playerCoins);
        hud.SetHealth(playerHealth);
        hud.SetLevel(playerLevel);
        hud.SetFoodRemaining(foodRemaining);
    } 

    public void Kill()
    {
        this.state = State.DEAD;
    }
    public void NewPlayer()
    {
        this.state = State.ALIVE;
    }
}