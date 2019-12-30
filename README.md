## ChilliConnect Tutorial
 
deltaDNA and ChilliConnect work well together to provide a rich set of Analytics, Live-Ops and CRM capabilities.

There are multiple communication paths between deltaDNA, ChilliConnect, the game client and respective cloud services to facilitate a wide range of use cases.

This tutorial contains examples of the following common scenarios. 

* [Common Player Identity](#common-player-identity)
* [Client Based Event Collection](#client-based-event-collection)
* [Cloud Based Event Collection](#cloud-based-event-collection)
4. Server authoritative game configuration from ChilliConnect Cloud
5. Time-critical offers, remote configuration and personalisation with deltaDNA Event Triggered Campaigns at run-time on client.
6. Server authoritative overrides and personalisation with deltaDNA Decision Point campaigns at run-time  called from ChilliConnect cloud code.
7. Remote server authoritative timer driven overrides and personalisation out of game 

Supporting project files on GitHub
https://github.com/deltaDNA/tutorial-chilliconnect

deltaDNA project
https://www.deltadna.net/demo-account/tutorial-chilliconnect/dev
*Please email support@deltaDNA.com if you require access to this game account*

ChilliConnect project 
https://dashboard.chilliconnect.com/games/642


Playable Demo
Todo - insert link


## Common Player Identity
It makes life a lot easier if deltaDNA and ChilliConnect use the same identity to refer to a player. 

The deltaDNA SDK can use any identity you provide, so it makes sense to tell it to use the ChilliConnectId to refer to the player. The ChilliConnectId is provided in the response from the ChilliConnect CreatePlayer method. Store it locally then use it on all subsequent occasions to Start the deltaDNA SDK. It will then be used for all deltaDNA analysis, segmentation and personalisation as well as appearing in the ChilliConnect UI for player management.

```csharp 
	DDNA.Instance.StartSDK(chilliConnectId);
``` 


## Client based event collection
Most of the gameplay events that you generate will originate from the game client, as the player does things you want to record and use in reporting, analysis, segmentation and personalisation. Examples of client side events would include the player levelling up, completing a mission etc..

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


## Cloud Based Event Collection
There are occasions when you may want to submit an event to deltaDNA from ChilliConnect Cloud code. This will happen if there is something that server knows about the player that the client doesn't know or cannot be trusted with. In this scenatio, the [deltaDNA REST API](https://docs.deltadna.com/advanced-integration/rest-api/) can be used to send events to deltaDNA.

In this example a **chilliConnectLogin** event is sent to deltaDNA from ChilliConnect Cloud Code when the player logs in to ChilliConnect. This is achieved by creating a ChilliConnect **Event Script** and setting it to trigger when the ChilliConnect **LogInUsingChilliConnect** API method is called. 

The [login event script](ChilliConnectCloudScripts/collect_login_event.js): 
* Retrieves the deltaDNA URL and Environment details from a [common cloud code module](ChilliConnectCloudScripts/deltadna_endpoints.js) 
* Constructs a JSON event and parameters
* Posts the event to deltaDNA
* Handles the response and logs any errors.

```javascript
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
