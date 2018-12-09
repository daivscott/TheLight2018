using UnityEngine;
using UnityEngine.Networking;

public class BotSpawner : NetworkBehaviour
{
    [SerializeField] GameObject botPrefab;

    [ServerCallback]
    void Start()
    {
        // create bot
        GameObject obj = Instantiate(botPrefab, transform.position, transform.rotation);
        // modify bot
        obj.GetComponent<NetworkIdentity>().localPlayerAuthority = false;
        obj.AddComponent<Bot>();
        // add bot to network
        NetworkServer.Spawn(obj);
    }
}