using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class should be attached to the world-space canvases used for the interactable tooltips.
/// </summary>
public class DigitalTooltip : InteractableObject
{
    [Tooltip("This is the sprite that will be displayed when the tooltip is inactive.")]
    [SerializeField] private Sprite icon;
    [Tooltip("This is the background sprite that will be displayed when the tooltip is active.")]
    [SerializeField] private Sprite background;
    [Tooltip("This is the audio clip that will play when notes are opened/closed.")]
    [SerializeField] private AudioClip interactClip;
    [SerializeField] private AudioClip voiceOver;
    [SerializeField] private Image voiceIcon;

    // These lines declare private fields to hold references to components
    private Image imageRenderer; //should be a child of this object
    private GameObject textObject; //should be a child of the image renderer object
    private AudioSource audioSource; // used to play audio clips 
    private Coroutine voiceOverCoroutine; // manages the coroutine for voice-over

    //Awake is executed before the Start method
    private void Awake()
    {
        //Create necessary component references
        if (TryGetComponent(out audioSource) == false)
        {
            Debug.LogWarning($"{name} needs an Audio Source attached to it!");
        }
        imageRenderer = GetComponentInChildren<Image>();
        if (imageRenderer == null)
        {
            Debug.LogWarning($"{name} should have a UI Image as a child!");
        }
        else
        {
            textObject = imageRenderer.GetComponentInChildren<Text>().gameObject;
            if (textObject == null)
            {
                Debug.LogWarning($"{imageRenderer.name} should have a UI Text as a child!");
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Set icon sprite and disable text
        if (imageRenderer != null)
        {
            imageRenderer.sprite = icon;
        }
        if (textObject != null)
        {
            textObject.SetActive(false);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        // Rotates the object so that it faces the same direction as the main camera.
        // Canvas's are inverted by default, so by facing the same direction the camera is facing it should appear correct to the player.
        transform.forward = Camera.main.transform.forward;
    }

    /// <summary>
    /// Disables the current tooltip (if there is one) then sets the background sprite and activates the tooltip text.
    /// </summary>
    /// <returns></returns>
    public override bool Activate()
    {
        if (Interaction.Instance.CurrentTooltip != this)
        {
            // Deactivate any currently active tooltip
            if (Interaction.Instance.CurrentTooltip != null)
            {
                Interaction.Instance.CurrentTooltip.Deactivate();
            }
            // Set this tooltip as the current one
            Interaction.Instance.CurrentTooltip = this;
            // Set background sprit and activate text
            if (imageRenderer != null)
            {
                imageRenderer.sprite = background;
            }
            if (textObject != null)
            {
                textObject.SetActive(true);
            }
            // If there is an audio file attached to the object, play the interaction sound and start the voice-over coroutine
            // (this makes sure the voiceover and interaction sound don't overlap each other)
            if (audioSource != null && interactClip != null)
            {
                audioSource.PlayOneShot(interactClip);
                // Task.Delay(1000).ContinueWith(t => VoiceOverActivate(true));

                // StopCoroutine is used so that the voiceover doesn't keep triggering when you spam click the interactable object
                // (it basically stops the voiceover from overlapping with itself)
                if (voiceOverCoroutine != null)
                    StopCoroutine(voiceOverCoroutine);
                // This is what starts the voiceover coroutine 
                voiceOverCoroutine = StartCoroutine(VoiceOverActivate());
            }
            
            return true;
        }
        return false;
    }

    /// <summary>
    /// If this tooltip is the current tooltip, sets the icon sprite then disables the tooltip text.
    /// </summary>
    /// <returns></returns>
    public override bool Deactivate()
    {
        if (Interaction.Instance.CurrentTooltip == this)
        {      
            // Clear the current tooltip
            Interaction.Instance.CurrentTooltip = null;
            if (imageRenderer != null)
            {
                // Set icon sprite and deactivate the textbox/text 
                imageRenderer.sprite = icon;
            }
            if (textObject != null)
            {
                textObject.SetActive(false);
            }
            // Stops the voiceover coroutine and plays the interaction sound
            if (audioSource != null && interactClip != null)
            {
                if (voiceOverCoroutine != null)
                    StopCoroutine(voiceOverCoroutine);
                voiceOverCoroutine = StartCoroutine(VoiceOverActivate(false));
                audioSource.PlayOneShot(interactClip);
            }
            return true;
        }
        return false;
    }
    // This coroutine manages the activation and deactivation of the voiceover audio
    // When activated, it waits for a delay and then plays the voiceover clip
    // When deactivated, it stops the audio
    IEnumerator VoiceOverActivate(bool isActive = true)
    {
        if (isActive)
        {
            // Creates a delay before the voiceover starts
            yield return new WaitForSeconds(1.8f);
            voiceIcon.enabled = true;
            // Turns the audio volume up to 2
            audioSource.volume = 2;
            audioSource.PlayOneShot(voiceOver);
            yield return new WaitForSeconds(voiceOver.length);
            voiceIcon.enabled = false;
        }
        else 
        {
            voiceIcon.enabled = false;
            // Turns the audio volume back down to 1
            audioSource.volume = 0.107f;
            audioSource.Stop();
        }
    }
}
