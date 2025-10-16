using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class RutFormatter : MonoBehaviour
{
    const string Accepted = "0123456789";

    [SerializeField] private TMP_InputField m_input;
    
    public void OnTextChanged(string text)
    {
        if(text.Length > 13)
        {
            m_input.text = text[..^1];
            return;
        }

        text = text.ToLower();
        int len = text.Length;

        List<char> list = new();
        for(int i = 0; i < len; i++)
        {
            bool last = i == len - 1;
            bool isK = text[i] == 'k';

            if (last && isK)
                continue;

            if (!Accepted.Contains(text[i]))
                list.Add(text[i]);
        }

        foreach (char c in list)
            text = text.Replace(c.ToString(), "");

        text = Formatter(text);
        m_input.text = text;

        if (text.Length >= 2)
            m_input.caretPosition = text.Length;
    }

    private string Formatter(string text)
    {
        if (text.Length < 2)
            return text;

        char last = text[^1];
        int rut = int.Parse(text[..^1]);
        string parsedRut = rut.ToString("N0", new CultureInfo("es-CL"));
        return $"{parsedRut}-{last}";
    }
}
