// An example script that makes a deltaDNA Engage Decision Point Campaign request
// from ChilliConnect Cloud Code 

ChilliConnect.Logger.info("Getting DDNA Environments: ");
var ddna = require("deltadna_endpoints");

ChilliConnect.Logger.info("Starting deltaDNA Engage DecisionPoint Campaign request : " + ChilliConnect.Request.decisionPoint);

var sdk = ChilliConnect.getSdk("2.15.0");
var player = sdk.PlayerAccounts.getPlayerDetails();

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
catch( e)
{
    ChilliConnect.Logger.error("Error:" + e );
}

ChilliConnect.Logger.info("deltaDNA Engage operation complete.");

return {};