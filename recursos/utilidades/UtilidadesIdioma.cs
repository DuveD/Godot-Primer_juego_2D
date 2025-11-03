using System.Collections.Generic;
using Godot;
using Microsoft.VisualBasic;

public class UtilidadesIdioma
{
    /// <summary>
    /// Enumerado con los idiomas disponibles.
    /// </summary>
    public enum Idioma
    {
        Castellano,
        Ingles
    }

    /// <summary>
    /// Establece el idioma de la aplicación.
    /// </summary>
    public static void SetIdioma(Idioma idioma)
    {
        string locale;

        switch (idioma)
        {
            default:
            case Idioma.Castellano:
                locale = "es";
                break;
            case Idioma.Ingles:
                locale = "en";
                break;
        }

        TranslationServer.SetLocale(locale);
    }

    /// <summary>
    /// Establece el idioma a castellano.
    /// </summary>
    public static void SetIdiomaCastellano()
    {
        SetIdioma(Idioma.Castellano);
    }

    /// <summary>
    /// Establece el idioma a inglés.
    /// </summary>
    public static void SetIdiomaIngles()
    {
        SetIdioma(Idioma.Ingles);
    }
}