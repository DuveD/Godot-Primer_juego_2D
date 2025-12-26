
using Godot;

namespace Primerjuego2D.nucleo.utilidades;

public static class UtilidadesNodos2D
{
    internal static void AjustarZIndexNodo(Node2D nodo2D, int zIndex)
    {
        nodo2D.ZIndex = zIndex;         // Mayor que el jugador
        nodo2D.ZAsRelative = false;     // Importante
    }
}