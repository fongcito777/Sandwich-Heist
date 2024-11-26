using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText; // El texto TMP dentro del botón
    [SerializeField] private Button button; // El botón al que pertenece el texto
    [SerializeField] private float blinkInterval = 0.5f; // Intervalo de parpadeo (en segundos)

    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (buttonText == null || button == null)
        {
            Debug.LogError("ButtonText or Button is not assigned in the Inspector!");
            return;
        }

        // Inicia el parpadeo
        blinkCoroutine = StartCoroutine(BlinkText());

        // Asigna la función para detener el parpadeo al presionar el botón
        button.onClick.AddListener(StopBlinking);
    }

    private IEnumerator BlinkText()
    {
        while (true) // Bucle infinito hasta que se detenga manualmente
        {
            buttonText.enabled = !buttonText.enabled; // Alterna la visibilidad del texto
            yield return new WaitForSeconds(blinkInterval); // Espera el tiempo definido
        }
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine); // Detiene la corrutina
        }
        buttonText.enabled = true; // Asegúrate de que el texto quede visible al final
    }
}
