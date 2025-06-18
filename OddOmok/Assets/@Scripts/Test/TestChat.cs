using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestChat : MonoBehaviour
{
    public GameObject inputField;
    public Button Button;
    public TMP_Text originalText;
    void Start()
    {
        Button.onClick.AddListener(() => OnChatInputButtonClick(true));
    }

    void Update()
    {
        
    }

    public void OnChatInputButtonClick(bool isLocal)
    {
        string inputText = inputField.GetComponent<TMP_InputField>().text;

        if (string.IsNullOrWhiteSpace(inputText))
            return;

        GameObject cloneTextGO = Instantiate(originalText.gameObject, originalText.transform.parent);
        TMP_Text cloneText = cloneTextGO.GetComponent<TMP_Text>();
        if (isLocal) cloneText.text = "나) " + inputText;
        else cloneText.text = "적) " + inputText;

        inputField.GetComponent<TMP_InputField>().text = "";
    }
}
