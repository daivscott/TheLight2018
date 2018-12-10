using UnityEngine;
using UnityEngine.Networking;


public class PlayerShooting : NetworkBehaviour {

    // Variables and Component references
    [SerializeField] float shotCooldown = 0.3f;
    [SerializeField] int killsToWin = 5;
    [SerializeField] Transform firePosition;
    [SerializeField] ShotEffectsManager shotEffects;
    [SerializeField] float shotLength = 50f;
    [SerializeField] bool wasKillShot;
    [SerializeField] bool killed;

    // kills variable with callback on value change
    [SyncVar(hook = "OnScoreChanged")] int score;
    
    // add player script
    Player player;
    float ellapsedTime;    
    bool canShoot;    

    void Start()
    {
        // reference to player script
        player = GetComponent<Player>();

        shotEffects.Initialize();

        if (isLocalPlayer)
            canShoot = true;

        
    }

    // Initialize kills (controlled only by the server)
    [ServerCallback]
    void OnEnable()
    {
        score = 0;        
    }

    void Update()
    {
        // Validation to check if the player can shoot
        if (!canShoot)
            return;

        ellapsedTime += Time.deltaTime;

        // Check for fire button being pressed and cooldown finishd
        if (Input.GetButtonDown("Fire1") && ellapsedTime > shotCooldown)
        {
            ellapsedTime = 0f;
            CmdFireShot(firePosition.position, firePosition.forward);
        }
    }
    
    // Client command to shoot sent to the server
    [Command]
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        // ray hit location
        RaycastHit hit;

        Ray ray = new Ray(origin, direction);
        // debug shot line
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        // check raycast hit location
        bool result = Physics.Raycast(ray, out hit, shotLength);

        // check if hit location exists
        if(result)
        {
            // check hit location and if enemy has PlayerHealth script 
            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();

            // if enemy had a PlayerHealth script
            if (enemy != null)
            {

                bool wasKillShot = enemy.TakeDamage();           

                // increment kills if the shot is a killshot
                if (wasKillShot && ++score >= killsToWin)
                {
                    player.Won();                 
                }
            }
        }
        // call to process shot effects
        RpcProcessShotEffects(result, hit.point);

    }

    // Set clients to play shot effects from the server
    [ClientRpc]
    void RpcProcessShotEffects(bool playImpact, Vector3 point)
    {
        // play the shot effects
        shotEffects.PlayShotEffects();
        // play impact effects
        if (playImpact)
            shotEffects.PlayImpactEffect(point);
    }

    // score changed call back function
    void OnScoreChanged(int value)
    {
        Debug.Log("value=" + value + "  score=" + score);
        score = value;
        if(isLocalPlayer)
        {
            PlayerCanvas.canvas.SetKills(value);
        }
    }

    public void FireAsBot()
    {
        CmdFireShot(firePosition.position, firePosition.forward);
    }
    
}
