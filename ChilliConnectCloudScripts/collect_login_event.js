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
            ChilliConnect.Logger.info(body);
           	ChilliConnect.Logger.info("Status Code :" + response.getStatus() );
           
            if (response.getStatus() == 204) {
                ChilliConnect.Logger.info("deltaDNA Login Event Sent to deltaDNA.");
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
catch(e)
{
    ChilliConnect.Logger.error("Error:" + e );
}