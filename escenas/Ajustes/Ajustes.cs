using Godot;
using System;

public partial class Control : Godot.Control
{
    private void OnPulsarIngles()
    {
        TranslationServer.SetLocale("en");
    }

    private void OnPulsarCastellano()
    {
        TranslationServer.SetLocale("es");
    }
}
