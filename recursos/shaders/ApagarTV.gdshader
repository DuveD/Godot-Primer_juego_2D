shader_type canvas_item;

uniform float progress : hint_range(0.0, 1.0);

// Altura (en UV) de la cortina negra.
void fragment() {
    // UV de 0 a 1
    float y = UV.y;

    // Centro vertical (0.5)
    // El negro avanza desde arriba y abajo hacia este punto.
    float border = mix(0.0, 0.5, progress);

    // Si estamos entre border y (1 - border), mostramos la pantalla normal (blanca en este caso)
    // Si estamos fuera, es negro.
    float isInside = step(border, y) * step(y, 1.0 - border);

    // Color base (efecto blanco).
    float bright = 1.0 - progress;

    // Si isInside=1 → se mantiene el color. Si =0 → negro.
    COLOR = vec4(vec3(bright * isInside), 1.0);
}
