using UnityEngine;

public class AnswerChoiceAudio_Controller : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Button Audio Clips")]
    public AudioClip mouseHover;
    public AudioClip buttonClicked;

    public void buttonHovered()
    {
        audioSource.PlayOneShot(mouseHover);
    }

    public void buttonPressed()
    {
        audioSource.PlayOneShot(buttonClicked);
    }
}
