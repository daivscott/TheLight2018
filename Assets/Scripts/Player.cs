
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.UI;


[System.Serializable]
public class ToggleEvent : UnityEvent<bool> {}

public class Player : NetworkBehaviour
{
    [SyncVar (hook = "OnNameChanged")] public string playerName;
    [SyncVar(hook = "OnColorChanged")] public Color playerColor;

    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] float respawnTime = 5f;

    GameObject mainCamera;
    NetworkAnimator anim;

    void Start()
    {
        // Set reference to animator
        anim = GetComponent<NetworkAnimator>();

        mainCamera = Camera.main.gameObject;

        EnablePlayer();
    }

    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        // Send the movement variables to the animator
        anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));
    }

    void DisablePlayer()
    {
        if (isLocalPlayer)
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
        if(isLocalPlayer)
        {
            PlayerCanvas.canvas.WriteGameStatusText("You Died");
            //PlayerCanvas.canvas.PlayDeathAudio();

            anim.SetTrigger("Died");
        }

        DisablePlayer();
        Invoke("Respawn", respawnTime);
    }

    void Respawn()
    {
        if(isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;

            anim.SetTrigger("Restart");
        }

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
}
