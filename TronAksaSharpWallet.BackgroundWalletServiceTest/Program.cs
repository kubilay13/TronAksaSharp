using TronAksaSharp.Enums;
using TronAksaSharp.Models;
using TronAksaSharp.Services;
using TronAksaSharp.Wallet;




var config = new WalletForwardConfig
{
    WatchAddress = "TYkrpvjHfVcuqRei7pDbSiU67CriYHNrvm",
    WatchPrivateKey = "b89e495325fe92898d8c6a7f7b18bac99e0c69af447a53bf37092eb0f98d29c7",
    ForwardAddress = "TEWJWLwFL3dbMjXtj2smNfto9sXdWquF4N",
    Network = TronNetwork.NileTestNet
};

var tronClient = new TronClient("APIKEY", config.Network);

var service = new WalletForwardService(tronClient, config);

var cts = new CancellationTokenSource();

Console.WriteLine("Wallet forward service started...");
await service.StartAsync(cts.Token);
    
