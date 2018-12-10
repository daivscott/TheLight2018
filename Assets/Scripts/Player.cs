
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> {}

public class Player : NetworkBehaviour
{
    [SyncVar(hook = "OnNameChanged")] public string playerName;
    [SyncVar(hook = "OnColorChanged")] public Color playerColor;

    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] float respawnTime = 3f;
    [SerializeField] float lobbyReturnTimer = 5f;
    [SerializeField] static bool gameOver = false;

    static List<Player> players = new List<Player>();

    GameObject mainCamera;
    NetworkAnimator anim;

    void Start()
    {
        // temp fix for name canvas or colour not displaying properly on client
        OnNameChanged(playerName);
        OnColorChanged(playerColor);

        // Set reference to animator
        anim = GetComponent<NetworkAnimator>();

        mainCamera = Camera.main.gameObject;

        if (!gameOver)
            EnablePlayer();
    }

    // add player to list
    [ServerCallback]
    void OnEnable()
    {
        gameOver = false;

        if (!players.Contains(this))
        {
            players.Add(this); 
        }
    }

    // remove player from list if they leave
    [ServerCallback]
    void OnDisable()
    {
        if (players.Contains(this))
        {
            players.Remove(this);
        }
    }

    void Update()
    {
        if (gameOver)
        {
            respawnTime = 0;
            //Cursor.lockState = CursorLockMode.None;
            //Cursor.visible = true;

            //CancelInvoke();
            //CancelInvoke("Die");
            //CancelInvoke("Respawn");
            //CancelInvoke("EnablePlayer");
            //Invoke("BackToLobby", lobbyReturnTimer);
        }

        if (!isLocalPlayer)
        {
            return;
        }

        // Send the movement variables to the animator
        anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));
    }

    void DisablePlayer()
    {
        if(isLocalPlayer)
        {
            PlayerCanvas.canvas.HideReticule();
            mainCamera.SetActive(true);
        }

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
            onToggleLocal.Invoke(false);
        else
            onToggleRemote.Invoke(false);
    }

    void EnablePlayer()
    {
        Debug.Log("Enabling Player");
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.Initialize();
            mainCamera.SetActive(false);
        }

        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);       
    }

    public void Die()
    {
        // check if local player or Bot
        if (isLocalPlayer || playerControllerId == -1)
        {
            anim.SetTrigger("Died");
        }

        if(isLocalPlayer)
        {
            PlayerCanvas.canvas.WriteGameStatusText("You Died!");
            //PlayerCanvas.canvas.PlayDeathAudio();
        }

        DisablePlayer();
        Invoke("Respawn", respawnTime);
    }

    void Respawn()
    {
        // check if local player or Bot
        if (isLocalPlayer || playerControllerId == -1)
        {
            anim.SetTrigger("Restart");
        }

        if (isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;
        }

        
        if (!gameOver)
            EnablePlayer();
    }


    void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        // set text
        GetComponentInChildren<Text>(true).text = playerName;
    }

    void OnColorChanged(Color value)
    {
        playerColor = value;
        GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);
    }

    [Server]
    public void Won()
    {
        GameOver();
        PlayerCanvas.canvas.WriteGameStatusText("You Won!");

        Invoke("SendWinMessage", 3f);
                
    }

    [ServerCallback]
    void GameOver()
    {
        gameOver = true;
    }

    void SendWinMessage()
    {
        // inform players of winner
        for (int i = 0; i < players.Count; i++)
        {
            players[i].RpcGameOver(netId, name);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        // go back to the lobby
        Invoke("BackToLobby", lobbyReturnTimer);
    }

    [ClientRpc]
    void RpcGameOver(NetworkInstanceId networkID, string name)
    {
        DisablePlayer();

        gameOver = true;

        if (isLocalPlayer)
        {
            if(netId == networkID)
            {
                PlayerCanvas.canvas.WriteGameStatusText("You Won!");
            }
            else
            {
                PlayerCanvas.canvas.WriteGameStatusText("Game Over!\n\n" + name + "\n\nWon The Game!");
            }
        }
    }

    void BackToLobby()
    {
        FindObjectOfType<NetworkLobbyManager>().SendReturnToLobby();
    }
}
