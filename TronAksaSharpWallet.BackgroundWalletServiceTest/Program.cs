using TronAksaSharp.Enums;
using TronAksaSharp.Wallet;


var client = new TronClient("API_KEY", TronNetwork.NileTestNet);

var watchAddress = "TYkrpvjHfVcuqRei7pDbSiU67CriYHNrvm"; 
var watchPrivateKey = "b89e495325fe92898d8c6a7f7b18bac99e0c69af447a53bf37092eb0f98d29c7";
var forwardAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N";
var network = TronNetwork.NileTestNet;
var minReserve = 1;
var checkIntervalSeconds = 10;


await client.StartForwardingAsync(watchAddress,watchPrivateKey,forwardAddress,minReserve,checkIntervalSeconds);

