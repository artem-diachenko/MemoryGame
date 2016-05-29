using Assets.Classes;
using UnityEngine;
using UnityEngine.UI;

public class InputValidation : MonoBehaviour
{
    // TODO: I would add additional check and message in case of required count is more than avaliable images on server

    private const int DefaultCount = 4;

    public GameObject Manager;
    public GameObject StartButton;

    private void Start ()
    {
        GetComponent<InputField>().placeholder.GetComponent<Text>().text = DefaultCount.ToString();

        Manager.GetComponent<CardManager>().SetCardsCount(DefaultCount / 2);
        GetComponent<InputField>().onEndEdit.AddListener(delegate { Input(GetComponent<InputField>()); });
    }

    private void Input(InputField field)
    {
        int count;

        if (!int.TryParse(field.text, out count))
        {
            SetWarningMessage(field);
            StartButton.GetComponent<Button>().interactable = false;

            Debug.LogWarning(string.Format(Messages.ParsingErrorFormat, field.text));
            return;
        }
        
        if (count % 2 == 0 && count != 0)
        {
            Manager.GetComponent<CardManager>().SetCardsCount(count / 2);
            StartButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            SetWarningMessage(field);
            StartButton.GetComponent<Button>().interactable = false;
        }        
    }

    private void SetWarningMessage(InputField field)
    {
        field.text = string.Empty;
        field.placeholder.GetComponent<Text>().text = Messages.ValidationErrorFormat;
    }
}
