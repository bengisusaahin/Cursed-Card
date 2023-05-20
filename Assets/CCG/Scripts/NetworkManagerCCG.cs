using UnityEngine;
using Mirror;


[AddComponentMenu("Network Manager CCG")]
public class NetworkManagerCCG : NetworkManager
{
    // Called when Player connects to the server and joins the game
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Transform startPos = GetStartPosition();
        GameObject player = Instantiate(playerPrefab);

        NetworkServer.AddPlayerForConnection(conn, player);
    }
}
