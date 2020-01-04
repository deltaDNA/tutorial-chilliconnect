# ChilliConnect Tutorial
 
deltaDNA and ChilliConnect work well together to provide a rich set of Analytics, Live-Ops and CRM capabilities.

There are multiple communication paths between deltaDNA, ChilliConnect, the game client and respective cloud services to facilitate a wide range of use cases.

This tutorial contains examples of the following common scenarios. 

* [Common Player Identity](#common-player-identity)
* [Client Based Event Collection](#client-based-event-collection)
* [Cloud Based Event Collection](#cloud-based-event-collection)
* [Cloud Based Game configuration](#cloud-based-game-configuration)
* [Client Based CRM](#client-based-crm)
* [Cloud Based CRM](#cloud-based-crm)
* [Out Of Game CRM](#out-of-game-crm)

Supporting project files
* Project source code on [GitHub](https://github.com/deltaDNA/tutorial-chilliconnect)
* [deltaDNA](https://www.deltadna.net/demo-account/tutorial-chilliconnect/dev) event schema, reporting, segementation and campaigns. *Please email support@deltaDNA.com if you require access to this game account*
* [Playable Demo](https://connect.unity.com/p/deltadna-chilliconnect-tutorial) on Unity Community Hub



# Common Player Identity
It makes life a lot easier if deltaDNA and ChilliConnect use the same identity to refer to a player. 

The deltaDNA SDK can use any identity you provide, so it makes sense to tell it to use the ChilliConnectId to refer to the player. The ChilliConnectId is provided in the response from the ChilliConnect CreatePlayer method. Store it locally then use it on all subsequent occasions to Start the deltaDNA SDK. It will then be used for all deltaDNA analysis, segmentation and personalisation as well as appearing in the ChilliConnect UI for player management.

```csharp 
	DDNA.Instance.StartSDK(chilliConnectId);
``` 


# Client based event collection
Most of the gameplay events that you generate will originate from the game client, as the player does things you want to record. Examples of client side events would include the player levelling up, completing a mission etc..

These events are recorded on the game client, cached locally and uploaded to deltaDNA automatically by the SDK at regular 1 minute intervals. 

This example records a **missionStarted** event to reveal the details of a mission the player has just started.
```csharp
        // Record DDNA MissionStarted event
        DDNA.Instance.RecordEvent(new GameEvent("missionStarted")
            .AddParam("missionName", "Mission " + currentLevel.ToString("D3"))
            .AddParam("missionID", currentLevel.ToString("D3"))
            .AddParam("userLevel", player.playerLevel)
            .AddParam("isTutorial", false)
            .AddParam("coinBalance", player.playerCoins)
            .AddParam("food", levels[currentLevel - 1].food)
            .AddParam("poison", levels[currentLevel - 1].poison)
            .AddParam("missionCost", levels[currentLevel - 1].cost)
            .AddParam("missionReward", levels[currentLevel - 1].reward)
            .AddParam("timelimit", levels[currentLevel - 1].timelimit))
            .Run();
    }
```


# Cloud Based Event Collection
There are occasions when you may want to submit an event to deltaDNA from ChilliConnect Cloud code. This will happen if there is something that server knows about the player that the client doesn't know or cannot be trusted with. In this scenatio, the [deltaDNA REST API](https://docs.deltadna.com/advanced-integration/rest-api/) can be used to send events to deltaDNA.

In this example a **chilliConnectLogin** event is sent to deltaDNA from ChilliConnect Cloud Code when the player logs in to ChilliConnect. This is achieved by creating a ChilliConnect Cloud Code **Event Script** and setting it to trigger when the ChilliConnect **logInUsingChilliConnect v2.0** API method is called. 

![loginEvent](Images/ChilliConnectEventScript.jpg)

The [login event script](ChilliConnectCloudScripts/collect_login_event.js): 
* Retrieves the deltaDNA URL and Environment details from a [common cloud code module](ChilliConnectCloudScripts/deltadna_endpoints.js) 
* Constructs a JSON event and parameters
* Posts the event to deltaDNA
* Handles the response and logs any errors.

```javascript
// An example script that sends a "chilliConnectLogin" event to deltaDNA
// from ChillConnect Cloud Code, when the player logs in.

var ddna = require("deltadna_endpoints");
ChilliConnect.Logger.info("Getting DDNA Environments: ");

var sdk = ChilliConnect.getSdk("2.15.0");
var player = sdk.PlayerAccounts.getPlayerDetails();
var coinsBalance = sdk.Economy.getCurrencyBalance(["COINS"]).Balances[0].Balance;
var userLevel = sdk.Economy.getCurrencyBalance(["USERLEVEL"]).Balances[0].Balance;

try {
    var ddnaCollectUrl = ddna.getCollectUrl();
    
    if (ddnaCollectUrl !== null) {
        var parameters = {
            coinBalance : coinsBalance,
            userLevel : userLevel
        };
        
        var body = {
            eventName: "chilliConnectLogin",
            userID: player.ChilliConnectID,
            eventParams : parameters
        };
        
        var request = ChilliConnect.Http.Request.setJson(body);
        var response = request.post(ddnaCollectUrl);    
        
        if(response) {
	 .
	 .
	 .	
```
For convenience, the deltaDNA URL and environment keys for this game are loaded from a **module** created in Chilli Connect Cloud Code

The [Endpoints module](ChilliConnectCloudScripts/deltadna_endpoints.js): 
* defines the deltaDNA Collect & Engage URL endpoints
* defines the deltaDNA Environment Keys
* defines the current Environment to use (DEV or LIVE)
* Provides a helper function to get the complete URL to POST Collect events or Engage campaign requests to. 

*The endpoint and environment key values for your game can found on your game details page in deltaDNA*

![EndpointsModule](Images/ChilliConnectModule.jpg)
```javascript
// Add the URL and Environments from your 'Game Details' page on deltaDNA
const collect = "https://collect16104ttrlc.deltadna.net/collect/api";
const engage = "https://engage16104ttrlc.deltadna.net";
const devKey = "00938463175409625971347052615752";
const liveKey = "00938473891762545150089251015752";
const mode = "DEV";
.
.
.
// Exports
module.exports.collectUrl = collect; 
module.exports.engageUrl = engage; 
module.exports.devEnvironmentKey = devKey; 
module.exports.liveEnvironmentKey = liveKey;
module.exports.mode = mode;

module.exports.getCollectUrl = getCollectUrl ;
module.exports.getEngageUrl = getEngageUrl ;
```

# Cloud Based Game Configuration
It is common practice for the configuration of game variables, currencies, store, inventory and levels to be loaded from a remote CDN so they can be easily updated without requiring a game update.

In this example, the player's coin blance and highest level reached are implemented as server authoratative currencies and loaded at the start of the game from the ChilliConnect Catalog as **Currencies**.
```C#
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
```
![Coins](Images/ChilliConnectCurrency.jpg)

The same approach is used to configure the game's levels. They are loaded as a JSON object decsribing the cost, reward and difficulty settings for each level. The JSON level configiuration is implemented as a **Metadata** Catalog Item in ChilliConnect and downloaded at the start of each game session.

```C#
    Debug.Log("Fetching metadata to configure game levels");
    chilliConnect.Catalog.GetMetadataDefinitions(new GetMetadataDefinitionsRequestDesc()
        , OnMetaDataFetched
        , (request, error) => Debug.LogError(error.ErrorDescription)
    );
```
```C#
    // Handle Game Configuration repsonse from ChilliConnect
    private void OnMetaDataFetched(GetMetadataDefinitionsRequest request, GetMetadataDefinitionsResponse response)
    {
        Debug.Log("Metadata fetched: ");
        levels = new List<Level>();

        foreach (MetadataDefinition metadataItem in response.Items)
        {
            var levelList = metadataItem.CustomData.AsDictionary().GetList("levels");

            foreach (var level in levelList)
            {
                Level l = new Level();

                if (level.AsDictionary().ContainsKey("food"))
                    l.food = level.AsDictionary().GetInt("food");

                if (level.AsDictionary().ContainsKey("poison"))
                    l.poison = level.AsDictionary().GetInt("poison");

                if (level.AsDictionary().ContainsKey("cost"))
                    l.cost = level.AsDictionary().GetInt("cost");

                if (level.AsDictionary().ContainsKey("reward"))
                    l.reward = level.AsDictionary().GetInt("reward");

                if (level.AsDictionary().ContainsKey("timelimit"))
                    l.timelimit = level.AsDictionary().GetInt("timelimit");

                levels.Add(l);
            }
        }
        Debug.Log("Levels Loaded " + levels.Count);
        .
        .
        .
```

![LevelConfig](Images/ChilliConnectLevels.jpg)


# Client Based CRM
There will be occasions when you want to change an aspect of the game configuration for a subset of players, without necessarily wanting to impact all players. 

Perhaps you want to change the difficulty of a particular level for new players, present a special offer to verteran players or promote a limited time event.

You may even want to A/B test different content and configurations to determine the optimal settings for different player segments. 

In this example our analysis has identified that the 3rd level is too difficult for new players, so we have setup a deltaDNA Event triggered campaign to reduce the cost and the number of items of food the player has to eat and increased the level reward if they fail the 3rd level. 

The campaign sends the following **Game Parameter Action** to the player.
![GameParameterAction](Images/Mission3Action.jpg)
targeting **New Players**
![EventTriggeredCampaignSegment](Images/EventSegment.jpg)
and is triggered if a **missionFailed** event with the **missionID** parameter equal to **003** is recorded.
![EventTrigger](Images/EventTrigger.jpg)
The following code records all mission fails when the player dies.
```C#
    public void PlayerDied()
    {
        player.Kill();
        missionSummary.Show(PlayerManager.State.DEAD);

        // Record DDNA MissionFailed event
        DDNA.Instance.RecordEvent(new GameEvent("missionFailed")
            .AddParam("missionName", "Mission " + currentLevel.ToString("D3"))
            .AddParam("missionID", currentLevel.ToString("D3"))
            .AddParam("userLevel", player.playerLevel)
            .AddParam("isTutorial", false)
            .AddParam("coinBalance", player.playerCoins)
            .AddParam("foodRemaining", player.foodRemaining)
            .AddParam("food", levels[currentLevel - 1].food)
            .AddParam("poison", levels[currentLevel - 1].poison)
            .AddParam("missionCost", levels[currentLevel - 1].cost)
            .AddParam("missionReward", levels[currentLevel - 1].reward)
            .AddParam("timelimit", levels[currentLevel - 1].timelimit))
            .Run();
        .
        .
        .
```
A common callback method has been specified to handle **Game Parameter** responses from **Event Triggered Campaigns**
```C#
        // Event Triggered Campaigns configuration settings
        DDNA.Instance.Settings.MultipleActionsForEventTriggerEnabled = true;
        DDNA.Instance.NotifyOnSessionConfigured(true);
        DDNA.Instance.OnSessionConfigured += (bool cachedConfig) => GetDDNAGameConfig(cachedConfig);

        // Register Handlers for Event Triggered Campaign responses
        DDNA.Instance.Settings.DefaultGameParameterHandler = new GameParametersHandler(gameParameters =>
        {
            MyGameParameterHandler(gameParameters);
        });
        DDNA.Instance.Settings.DefaultImageMessageHandler = new ImageMessageHandler(DDNA.Instance, imageMessage =>
        {
            MyImageMessageHandler(imageMessage);
        });
```
It can handle all the parameters we may want to remotely override.
```C#        
    private void MyGameParameterHandler(Dictionary<string, object> gameParameters)
    {
        Debug.Log("Received deltaDNA gameParameters from event triggered campaign" + DeltaDNA.MiniJSON.Json.Serialize(gameParameters));

        foreach (string key in gameParameters.Keys)
        {
            // Coin Balalnce Modifier
            if (key == "coins")
            {
                int c = System.Convert.ToInt32(gameParameters[key]);
                RewardReceived("coins", "Event Triggered Campaign reward", c);
            }


            // Level configuration modifiers
            if (key == "food" || key == "poison" || key == "missionCost" || key == "missionReward" || key == "timelimit")
            {
                int v = System.Convert.ToInt32(gameParameters[key]);
                Debug.Log(string.Format("Mission {0} {1} configuration changed to {2}", currentLevel, key, v));
                missionModified(currentLevel, key, v);

                switch (key)
                {
                    case "food":
                        levels[currentLevel - 1].food = v;
                        break;
                    case "poison":
                        levels[currentLevel - 1].poison = v;
                        break;
                    case "missionCost":
                        levels[currentLevel - 1].cost = v;
                        break;
                    case "missionReward":
                        levels[currentLevel - 1].reward = v;
                        break;
                    case "timelimit":
                        levels[currentLevel - 1].timelimit = v;
                        break;
                }
            }
        }
    }
```
We even record a **missionModified** event so we can easily analyse the impact of any changes to a level.
```C#
    public void missionModified(int missionID, string modifierType, int modifierAmount)
    {
        DDNA.Instance.RecordEvent(new GameEvent("missionModified")
            .AddParam("missionID", missionID.ToString("D3"))
            .AddParam("userLevel", player.playerLevel)
            .AddParam("coinBalance", player.playerCoins)
            .AddParam("food", levels[missionID-1].food)
            .AddParam("poison", levels[missionID-1].poison)
            .AddParam("missionCost", levels[missionID-1].cost)
            .AddParam("missionReward", levels[missionID-1].reward)
            .AddParam("timelimit", levels[missionID-1].timelimit))
            .Run();
    }
```
# Cloud Based CRM
There may be occasions when you want to modify the game or personalize the player experience remotely in a server autoritative manner. You can do this by making deltaDNA Engage Campaign requests from your ChilliConnect Cloud Code and using the response to update some aspect of the game.

A typical scenario might involve a Cloud Code Event Script communicating with the deltaDNA decision point campaign API to retrieve Ad placement info, IAP promo information. 

However, we will show a more complex example to demonstrate additional commumication steps. 
* The game client will call a ChilliConnect Cloud Code script.
* The Cloud Code script will make a request to the deltaDNA Engage Decision Point API.
* The Cloud Code script will pass the response back to the game client.
* The game client will parse and act on the response.

This scenario passes Ad Placemement info back to the game client so we can remotely control whether the player sees **IAP Promo** or **Rewarded Ad** placements, depending on the player segmentation conditions we remotely define in decision point campaigns.

e.g. A **Game Parameter Action** that would show the player Rewarded Ads worth 10 Coins on each failed level up to 10 times per session.
![RewardedAd GameParameter Action](Images/RewardedAd.jpg)

And a set of Decision Point Campaigns to promote different placements based on player segment.
![Decision Point Camapigns](Images/DecisionPointCampaigns.jpg) 

The following code in the game client runs the remote Cloud Code script and deals with the response.
```C#
    private void RemoteCampaign(string decisionPoint, string parameters)
    {

        var scriptParams = new Dictionary<string, SdkCore.MultiTypeValue>();
        scriptParams.Add("decisionPoint", decisionPoint);
        scriptParams.Add("locale", "en_GB");
        scriptParams.Add("platform", DDNA.Instance.Platform);
        if (!string.IsNullOrEmpty(parameters)) scriptParams.Add("parameters", parameters);

        var runScriptRequest = new RunScriptRequestDesc("ENGAGE_DECISION_POINT_CAMPAIGN");
        runScriptRequest.Params = scriptParams;

        Debug.Log("Running Engage Campaign Script for decisionPoint : " + decisionPoint);
        chilliConnect.CloudCode.RunScript(runScriptRequest
            , (request, response) => {

                var engageResponse = response.Output.AsDictionary();

                if (engageResponse.ContainsKey("parameters"))
                {
                    var p = engageResponse["parameters"].AsDictionary();
                    foreach (var i in p)
                    {
                        //Debug.Log("Response Parameter : " + i.Key + " Value : " + i.Value);

                        if (i.Key == "placementType")
                            placementManager.type = i.Value.AsString();

                        if (i.Key == "placementPosition")
                            placementManager.position = i.Value.AsString();

                        if (i.Key == "placementFrequency")
                            placementManager.frequency = i.Value.AsInt();

                        if (i.Key == "placementSessionCap")
                            placementManager.limit = i.Value.AsInt();

                        if (i.Key == "placementType")
                            placementManager.type = i.Value.AsString();

                        if (i.Key == "placementPromoID")
                            placementManager.promoID = i.Value.AsInt();
                    }
                }
            }
            , (request, error) => Debug.LogError(error.ErrorDescription));
    }
```

The Cloud Code script is implemented as am **API** script that requires 3 input parameters.
![API Script](Images/ChilliConnectAPIScript.jpg)

Like the event recording script it uses a common module to get the deltaDNA **Engage API URL** and **Environment Key**. It then build an Engage reuquest, POSTs it and passes the response back to the game client.
```Javascript
try {
    var ddnaEngageUrl =  ddna.getEngageUrl();
    
    if (ChilliConnect.Request.decisionPoint && ChilliConnect.Request.platform && ddnaEngageUrl) {
        
        var parameters = {};
        
        var body = {
            decisionPoint: ChilliConnect.Request.decisionPoint,
            platform : ChilliConnect.Request.platform,
            userID: player.ChilliConnectID,
            version: "4",
            parameters : parameters
        };
        
        ChilliConnect.Logger.info(ddnaEngageUrl);

        var request = ChilliConnect.Http.Request.setJson(body);
        var response = request.post(ddnaEngageUrl);    
                  
        if(response) {
            ChilliConnect.Logger.info(response.getBody());
            return(response.getJSON());
        }
    }            
}
````

# Out Of Game CRM
deltaDNA Out Of Game campaigns are used for sending Push Notifications, Emails and Webhook requests to the player. But they are also ideal for remotely gifting currency or inventory items to a player. 

Out Of Game campaigns support powerful player segmentation, timing and localisation capabilities and can be used to automate CRM campaigns. 

In this example we will remotely gift 25 coins to new players at 6pm in their local time-zone and send them a push notification informing them of the gift and encouraging them back to the game.











