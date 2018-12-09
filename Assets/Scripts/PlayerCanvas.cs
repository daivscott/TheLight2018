using UnityEngine;
using UnityEngine.UI;

public class PlayerCanvas : MonoBehaviour 
{
    public static PlayerCanvas canvas;

    // Component references
    [Header("Component References")]
    [SerializeField] Image reticule;
    [SerializeField] UIFader damageImage;
    [SerializeField] Text gameStatusText;
    //[SerializeField] Text healthValue;
    [SerializeField] Text killsValue;
    [SerializeField] Text logText;
    [SerializeField] AudioSource deathAudio;
    [SerializeField] RectTransform healthBarFill;

    //Ensure there is only one PlayerCanvas
    void Awake()
    {
        if(canvas == null)
            canvas = this;
        else if(canvas != this)
            Destroy(gameObject);
    }

    //Find all of our resources
    void Reset()
    {
        reticule = GameObject.Find("Reticule").GetComponent<Image> ();
        damageImage = GameObject.Find("DamagedFlash").GetComponent<UIFader> ();
        gameStatusText = GameObject.Find("GameStatusText").GetComponent<Text> ();
        //healthValue = GameObject.Find("HealthValue").GetComponent<Text> ();
        killsValue = GameObject.Find("KillsValue").GetComponent<Text> ();
        logText = GameObject.Find("LogText").GetComponent<Text> ();
        deathAudio = GameObject.Find("DeathAudio").GetComponent<AudioSource> ();
    }

    // Initialize display
    public void Initialize()
    {
        reticule.enabled = true;
        gameStatusText.text = "";
    }

    // Hide the reticule function
    public void HideReticule()
    {
        reticule.enabled = false;
    }

    // Flash the damage effect function
    public void FlashDamageEffect()
    {
        damageImage.Flash();
    }

    // Play audio on death function
    public void PlayDeathAudio()
    {
        if (!deathAudio.isPlaying)
            deathAudio.Play();
    }


    // Set GUI kills value
    public void SetKills(int amount)
    {
        killsValue.text = amount.ToString();
    }

    // Set GUI health amount
    public void SetHealth(float amount)
    {
        // Set healthbar amount
        healthBarFill.localScale = new Vector3(amount, 1f, 1f);
        //healthValue.text = amount.ToString ();
    }

    // Display text to GUI element gameStatusText
    public void WriteGameStatusText(string text)
    {
        gameStatusText.text = text;
    }

    // Display text to GUI element logText function
    public void WriteLogText(string text, float duration)
    {
        CancelInvoke ();
        logText.text = text;
        Invoke ("ClearLogText", duration);
    }

    // Clear log text function
    void ClearLogText()
    {
        logText.text = "";
    }
}