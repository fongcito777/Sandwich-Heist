using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CollectibleHandler : MonoBehaviour
{
    [SerializeField] private GameObject notificationPanel; // Panel de notificación
    [SerializeField] private TextMeshProUGUI messageText; // Texto del mensaje
    [SerializeField] private Image collectibleImage; // Imagen del coleccionable
    [SerializeField] private AudioClip notificationSound; // Sonido de notificación

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Collectible"))
        {
            // Obtén la información del coleccionable
            Collectible collectible = collision.GetComponent<Collectible>();

            if (collectible != null)
            {
                // Actualiza la UI del panel
                ShowNotification(collectible);

                // Desactiva el coleccionable
                Destroy(collision.gameObject);
            }
        }
    }

    private void ShowNotification(Collectible collectible)
    {
        // Configura el mensaje y la imagen
        messageText.text = collectible.collectibleMessage;
        collectibleImage.sprite = collectible.collectibleSprite;

        // Activa el panel
        notificationPanel.SetActive(true);

        // Reproduce el sonido
        AudioManager.Instance.PlaySFX(notificationSound);

        // Oculta el panel después de unos segundos
        StartCoroutine(HideNotification());
    }

    private IEnumerator HideNotification()
    {
        yield return new WaitForSeconds(2f); // Espera 2 segundos
        notificationPanel.SetActive(false);
    }
}
