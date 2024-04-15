using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOver : InteractableObject
{
    [SerializeField] private AudioClip voiceOver;

    private AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public override bool Activate()
    {
        if (Interaction.Instance.CurrentTooltip != this)
        {
            if (Interaction.Instance.CurrentTooltip != null)
            {
                Interaction.Instance.CurrentTooltip.Deactivate();
            }
            if (audioSource != null && voiceOver != null)
            {
                audioSource.PlayOneShot(voiceOver);
            }
            Debug.Log("Play");
            return true;
        }
        Debug.Log("Not Played");
        return false;
    }
    public override bool Deactivate()
    {
        if (Interaction.Instance.CurrentTooltip == this)
        {
            Interaction.Instance.CurrentTooltip = null;
           
            if (audioSource != null && voiceOver != null)
            {
                audioSource.PlayOneShot(voiceOver);
            }
            Debug.Log("Stop");
            return true;
        }
        Debug.Log("Not Stopped");
        return false;
    }
}
