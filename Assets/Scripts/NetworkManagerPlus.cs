using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkManagerPlus : NetworkManager
{
    [SerializeField]
    List<Transform> spawnPositions = new List<Transform>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = (GameObject)Instantiate(playerPrefab, GetSpawnPosition(numPlayers), Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);

        player.GetComponent<PlayerLogic>().SetTeam((numPlayers - 1) % 2 == 0 ? Team.Blue : Team.Red);

        Debug.Log("Player spawned with Index: " + (numPlayers - 1));
    }

    Vector3 GetSpawnPosition(int spawnIndex)
    {
        return spawnPositions[spawnIndex].position;
    }
}
