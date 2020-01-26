//  An example script that handles a request from a deltaDNA Out Of Game campaign
//  parses the input to check it is valid, updates the player record on Chilli Connect accordingly
//  and sends an appropriate response back to deltaDNA 

ChilliConnect.Logger.info("Getting DDNA Environments: ");
var ddna = require("deltadna_endpoints");

// get the required version of the SDK see: https://docs.chilliconnect.com/api/?system=script&sdk=2.15.0
var sdk = ChilliConnect.getSdk("2.15.0");

ChilliConnect.Logger.info("Starting deltaDNA Out Of Game Campaign request");
var response = ChilliConnect.External.Response;

try {

    var coinsBalance;
    var userLevel;
    var player;

    // parse external POST for required parameters
    var userID = ChilliConnect.External.Request.getQueryParam("userID");
    var environmentID = ChilliConnect.External.Request.getQueryParam("environmentID");
    var rewardType = ChilliConnect.External.Request.getQueryParam("rewardType");
    var rewardAmount = Number(ChilliConnect.External.Request.getQueryParam("rewardAmount"));
    var rewardName = ChilliConnect.External.Request.getQueryParam("rewardName");

    // Check the request has a userID and an enviroment key that matches our game and a reward
    if (userID && environmentID && (environmentID == ddna.devEnvironmentKey || environmntID == ddna.liveEnvironmntKey) && rewardType && rewardAmount) {
        ChilliConnect.Logger.info("userID :" + userID + " environmentID : " + environmentID);
        ChilliConnect.Logger.info("Reward :" + rewardAmount + " " + rewardType);

        // Apply reward
        if (rewardType == "COINS") {
            // Increment Player Coin balance
            sdk.PlayerAccounts.asPlayer(userID,
                function () {
                    sdk.Economy.addCurrencyBalance("COINS", rewardAmount);
                    player = sdk.PlayerAccounts.getPlayerDetails();
                    coinsBalance = sdk.Economy.getCurrencyBalance(["COINS"]).Balances[0].Balance;
                    userLevel = sdk.Economy.getCurrencyBalance(["USERLEVEL"]).Balances[0].Balance;
                }
            );


            // Send Feedback event to DDNA to record the fact that the reward was applied
            var ddnaCollectUrl = ddna.getCollectUrl();

            if (ddnaCollectUrl !== null) {
                var parameters = {
                    coinBalance: coinsBalance,
                    userLevel: userLevel,
                    rewardName: rewardName,
                    rewardType: rewardType,
                    rewardAmount: rewardAmount
                };

                var body = {
                    eventName: "outOfGameReward",
                    userID: player.ChilliConnectID,
                    eventParams: parameters
                };

                var request = ChilliConnect.Http.Request.setJson(body);
                var collectResponse = request.post(ddnaCollectUrl);

                if (collectResponse) {
                    ChilliConnect.Logger.info(body);
                    ChilliConnect.Logger.info("Status Code :" + collectResponse.getStatus());

                    if (collectResponse.getStatus() == 204) {
                        ChilliConnect.Logger.info("deltaDNA outOfGameReward Event Sent to deltaDNA.");
                    }
                    else {
                        ChilliConnect.Logger.info("Error Posting event : deltaDNA response code " + response.getStatus());
                    }
                }
                else {
                    ChilliConnect.Logger.info("Error Posting event to deltaDNA.");
                }
            }
        }

        response.SetStatus(200);

    } else {

        // Return error message
        response.setStatus(400);
        response.setBody("Request missing required parameters");
    }

}
catch (e) {
    ChilliConnect.Logger.error("Error:" + e);
    response.setStatus(400);
    response.setBody("Error:" + e);
}

ChilliConnect.Logger.info("Operation Completed.");
return response;