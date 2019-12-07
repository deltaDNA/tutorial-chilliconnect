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


	// Use this for initialization
	void Start () {

        hud = GameObject.FindObjectOfType<HudManager>();          
        UpdatePlayerStatistics();
    }


    // Fetch Currency from ChilliConnect
    public void FetchCurrency(ChilliConnectSdk chilliConnect)
    {
        Debug.Log("Fetching currency");
        
        chilliConnect.Economy.GetCurrencyBalance(new GetCurrencyBalanceRequestDesc(), OnCurrencyBalanceFetched, (request, error) => Debug.LogError(error.ErrorDescription));
    }



    // Handle Currency Response from ChilliConnect
    private void OnCurrencyBalanceFetched(GetCurrencyBalanceRequest request, GetCurrencyBalanceResponse response)
    {
        Debug.Log("Currency fetched: ");
        foreach (var item in response.Balances)
        {   
            switch (item.Key)
            {
                case "COINS":
                    this.SetCoins(item.Balance);
                    break;
            }
        }
    }



    public void SetLevel(int l)
    {
        playerLevel = l;
        hud.SetLevel(playerLevel);
    }

    public void SetHealth(int h)
    {
        playerHealth = h;
        hud.SetHealth(playerHealth);
    }

    public void SetCoins(int c)
    {
        playerCoins = c;
        hud.SetCoins(playerCoins);
    }

    public void SetFoodRemaining(int f)
    {
        foodRemaining = f;
        hud.SetFoodRemaining(foodRemaining);
    }

    public void UpdatePlayerStatistics()
    { 
        hud.SetCoins(playerCoins);
        hud.SetHealth(playerHealth);
        hud.SetLevel(playerLevel);
        hud.SetFoodRemaining(foodRemaining);
    } 
}