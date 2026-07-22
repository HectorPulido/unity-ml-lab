using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

public class LlamaCharacter : MonoBehaviour
{
    // show a prompt in the inspector, with big text area
    [TextArea(3, 10)]
    public string Prompt;
    public float temp = 0.8f;
    public int max_tokens = 50;

    public TMP_Text TextMeshPro;

    async void OnMouseDown()
    {
        TextMeshPro.text = "...";
        var textResponse = await ClientLlama.singletonInstance.SendMessageToServer(Prompt, max_tokens, temp);

        textResponse = Regex.Replace(textResponse, @"[^a-zA-Z0-9áéíóú ]", "");
        TextMeshPro.text = textResponse;
    }
}
