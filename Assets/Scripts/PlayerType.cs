using UnityEngine;
using TMPro;

public class PlayerType : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] tmpInputFields;

    public bool HasFocusedInputField()
    {
        if (HasFocusedInputField(tmpInputFields))
        {
            return true;
        }

        return false;
    }

    private static bool HasFocusedInputField(TMP_InputField[] fields)
    {
        if (fields == null)
        {
            return false;
        }

        foreach (TMP_InputField field in fields)
        {
            if (field != null && field.isFocused)
            {
                return true;
            }
        }

        return false;
    }
}
