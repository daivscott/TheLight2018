using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {

    // Set the max health
    [SerializeField] float maxHealth = 3f;

    // Health variable with callback on change
    [SyncVar (hook = "OnHealthChanged")] float health;

    // Player reference
    Player player;
    
    void Awake()
    {
        player = GetComponent<Player>();
    }

    // Initialize health to max (controlled only by the server) on respawn
    [ServerCallback]
    void OnEnable()
    {
        health = maxHealth;
    }

    // Initialize health to max (controlled only by the server) at beginning
    [ServerCallback]
    void Start()
    {
        health = maxHealth;
    }

    // 
    [Server]
    public bool TakeDamage()
    {
        bool died = false;
        if (health <= 0)
            return died;

        //decrement the health variable
        health--;

        // set died variable to true if health less than 0
        died = health <= 0;

        // function call
        RpcTakeDamage(died);

        return died;
    }

    // Damage message sent to clients from server
    [ClientRpc]
    void RpcTakeDamage(bool died)
    {
        if(isLocalPlayer)
        {
            PlayerCanvas.canvas.FlashDamageEffect();
        }

        if (died)
            player.Die();
    }

    // health changed callback function
    void OnHealthChanged(float value)
    {
        health = value;
        if(isLocalPlayer)
        {
            // Setting the health amount as percentage in the healthbar
            PlayerCanvas.canvas.SetHealth(value/maxHealth);
        }
    }
}
