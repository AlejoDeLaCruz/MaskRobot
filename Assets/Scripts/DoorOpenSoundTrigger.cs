using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorOpenSoundTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip clip;                 // clip a reproducir (opcional si tenés AudioSource con clip)
    public bool playOnlyOnce = false;      // si true, suena sólo la primera vez que pasa el jugador

    [Header("Opcional")]
    public float minSpeedToTrigger = 0f;   // si >0 evita reproducir si el jugador está quieto (ej: al pararse encima)

    AudioSource audioSource;
    bool hasPlayed = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Asegurate de que el objeto se llame "Player" o cambiá la comprobación por otra (layer, tag, nombre).
        if (!other.CompareTag("Player")) return;

        if (playOnlyOnce && hasPlayed) return;

        // Si pedimos velocidad mínima, comprobamos la Rigidbody2D del jugador
        if (minSpeedToTrigger > 0f)
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb == null || rb.velocity.magnitude < minSpeedToTrigger) return;
        }

        if (clip != null)
            audioSource.PlayOneShot(clip);
        else if (audioSource.clip != null)
            audioSource.Play();

        hasPlayed = true;
    }
}