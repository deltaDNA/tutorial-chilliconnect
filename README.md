## ChilliConnect Tutorial
Tutorial on using Chilliconnect and deltaDNA together
 
deltaDNA and ChilliConnect SDKs and Services work well together to provide a rich set of Analytics, Live-Ops and CRM capabilities.

There are multiple communication paths between  deltaDNA, ChilliConnect, the game client and respective cloud services to facilitate a wide range of use cases.

This tutorial shows examples of some of the following common scenarios. 

* [Common Player Identity](#common-player-identity)
* [Event data collection from client-side deltaDNA SDK] (#ddna-events)
* [Event data collection from ChilliConnect cloud code] (#chilli-events)
4. Server authoritative game configuration from ChilliConnect Cloud
5. Time-critical offers, remote configuration and personalisation with deltaDNA Event Triggered Campaigns at run-time on client.
6. Server authoritative overrides and personalisation with deltaDNA Decision Point campaigns at run-time  called from ChilliConnect cloud code.
7. Remote server authoritative timer driven overrides and personalisation out of game 

Supporting Project files on GitHub
https://github.com/deltaDNA/tutorial-chilliconnect

deltaDNA project
https://www.deltadna.net/demo-account/tutorial-chilliconnect/dev

ChilliConnect project 
https://dashboard.chilliconnect.com/games/642


Playable Demo
Todo - insert link


## Common Player Identity
It makes life a lot easier if deltaDNA and ChilliConnect use the same identity to refer to a player. The deltaDNA SDK can use any identity you provide, so it makes sense to tell the deltaDNA SDK to use the ChilliConnectId to refer to the player. The ChilliConnectId is provided in the response from the ChilliConnect CreatePlayer method. Store it locally and use it to Start the deltaDNA SDK and use it all deltaDNA analysis, segmentation and personalisation.

```csharp 
	DDNA.Instance.StartSDK(chilliConnectId);
``` 


## Event data collection from client-side deltaDNA SDK
