// An example module that exports deltaDNA geme environment details 
// to be shared across Cloud Code scripts interacting with deltaDNA.

// Add the URL and Environments from your 'Game Details' page on deltaDNA
const collect = "https://collect16104ttrlc.deltadna.net/collect/api";
const engage = "https://engage16104ttrlc.deltadna.net";
const devKey = "00938463175409625971347052615752";
const liveKey = "00938473891762545150089251015752";
const mode = "DEV";

const getCurrentKey = () =>
{
    if (mode=="DEV") {
        return devKey;
    }
    else if (mode == "LIVE"){
        return liveKey;
    }
    else {
        return null;
    }
};

const getCollectUrl = () => {
  return collect + '/'+ getCurrentKey();  
};

const getEngageUrl = () => {
  return engage + '/'+ getCurrentKey();  
};

// Exports
module.exports.collectUrl = collect; 
module.exports.engageUrl = engage; 
module.exports.devEnvironmentKey = devKey; 
module.exports.liveEnvironmentKey = liveKey;
module.exports.mode = mode;

module.exports.getCollectUrl = getCollectUrl ;
module.exports.getEngageUrl = getEngageUrl ;