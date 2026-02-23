# 🕹️ Primer Juego 2D (Godot)

Un juego simple 2D hecho con Godot 4.x, basado en el tutorial oficial "[Your First 2D Game](https://docs.godotengine.org/en/4.4/getting_started/first_2d_game/index.html)".
He ampliado el proyecto con características extra para aprender más sobre Godot y C#.

---

## 📂 Estructura del proyecto

- escenas/ → Todas las escenas del juego: Batalla (BatallaHUD, BatallaControlador), Jugador, Enemigo, etc.

- nucleo/ → Archivos de configuración, archivos de constantes, localización, utilidades, etc.

- recursos/ → Imágenes, audio, fuentes, archivos de traduccion, etc.

## ⚡ Características añadidas

### v1.7.1

- Corrección en ButtonPersonalizado para inicializarse sin foco si está desactivado.
- Corrección de código.

### v1.7

- Creado Gestor Notifiación de Logros para mostrar logros por pantalla.
- Modificado Gestor de Logros para lanzar función al desbloquear logros.
- Creado Contenedor Logro Notificación para mostrar el logro.
- Creado indicador de guardado.
- Mostrado indicador de guardado al guardar información del perfil.
- Añadido animación a botones de aumentar tamaño al pasar el ratón o seleccionar.
- Corrección de código.
- Reorganización de código.
- Añadido tiempo de destrucción a Botiquín.
- Eliminados nodos Botones innecesarios.
- Ajustes gráficos.
- Corrección de errores.

### v1.6

- Implementado gestor de atributos del jugador con modificadores vivos.
- Añadidos diferentes efectos a powerups y jugador.
- Implementado gestor de perfiles/partidas: creación, carga y borrado.
- Implementadas vidas para el jugador.
- Creado consumible 'Botiquín' +1 vida.
- Modificada posibilidad de aparecer powerups para no mostrar botiquines si el jugador tiene el máximo de vida.
- Añadido cambio de cultura al thread de C#.
- Añadidas animaciones a elementos de menú.
- Creado Overlay/Cargando y mostrado al cargar el perfil activo al empezar la partida.

### v1.5

- Imeplementación de sistema de Consumibles.
- Creadas monedas recogibles por el jugador.
- Añadido texto flotante al recoger monedas.
- Creado Spawn de PowerUps en batalla.
- Creado Spawn de Monedas en batalla.
- Añadido menú de pausa con las opciones "Renaudar", "Ajustes" y "Terminar Batalla".
- Añadida animación de apagado de televisión al cerrar el juego.
- Creada pantalla y sistema de gestión de estadísticas.
- Creada pantalla y sistema de gestión de logros.
- Añadidas opciones en el menú principal.

### v1.4

- Cambio de look & feel del uso de los botones del menú principal según se use teclado o ratón.
- Añadido control para no spawnear monedas cerca del jugador.
- Añadida animación de apagado al salir del juego.
- Añadidos sonidos a botones y música.
- Añadida animación al morir el personaje con partículas.
- Implementado menú de ajustes.

### v1.3

- Añadido spawn de monedas con animación vinculadas a la puntuación.
- Gestor de audio avanzado con pool de reproductores para sonidos.
- Diferentes niveles de audio para música y efectos.
- Gestión de ajustes persistido en fichero 'ajustes.ini'.

### v1.2

- Sistema de Logger con niveles Trace, Info, Warning y Error.
- Menú principal.
- Fondo con partículas.
- Proyecto actualizado a Godot 4.5.1.
- Efecto Shake al ser golpeado por un enemigo.

### v1.1

- Gestión de la localización e idiomas Español e Inglés.
- Pausa de la partida.

### v1.0

- Movimiento en 8 direcciones con "animación" correspondiente.
- Juego base implementado.

## 📖 Referencias

- Tutorial oficial de Godot 4: [Your First 2D Game](https://docs.godotengine.org/en/4.4/getting_started/first_2d_game/index.html)
- Video tutorial de Alva Majo: [Godot para retrasados [Tutorial]](https://www.youtube.com/watch?v=eQ_HBvtdoiU&t=663s)
- Andrew Vickerman Godot Audio Manager: [Godot Audio Manager](https://github.com/insideout-andrew/godot-audio-manager/tree/main)
- Video tutorial de Rayuse: [Start Menu Keyboard Selection and Shortcuts in Godot](https://www.youtube.com/watch?v=hXXSWhsjp6M)

## Créditos

- Recursos del tutorial de Godot: © 2014-present Juan Linietsky, Ariel Manzur y la comunidad de Godot (CC BY 3.0)
- _digital_click.mp3_ by CreatorsHome ([Digital Click](https://pixabay.com/es/sound-effects/digital-click-357350/))
- _game-over-arcade.mp3_ by freesound_community ([Game Over Arcade](https://pixabay.com/es/sound-effects/game-over-arcade-6435/))
- _retro_song.mp3_ by H-Beats ([Retro game effects](https://pixabay.com/es/sound-effects/retro-game-effects-252988/))
- _retro_coin.mp3_ by Driken5482 ([Retro coin 4](https://pixabay.com/es/sound-effects/retro-coin-4-236671/))
- _retro_wave.mp3_ by van_Wiese ([Retro wave loop 125 BPM](https://pixabay.com/es/sound-effects/retro-wave-loop-125-bpm-8963/))
- _kick.mp3_ by u_9ikddrpcfz ([Kick](https://pixabay.com/es/sound-effects/kick-182227/))
- _spacerangerexpand.ttf_ by Iconian Fonts ([Space Ranger](https://www.dafont.com/es/space-ranger.font))
- _iman.png_ by Giuseppe Ramos ([Magnet cartoon icon](https://www.vecteezy.com/vector-art/10793480-magnet-cartoon-icon))
- _Tronicles-Sirius_Beat.mp3_ by Sirius Beat ([Tronicles](https://www.youtube.com/watch?v=2DNpupwQPJI))
- _tv_shutdown.mp3_ by SoundReality ([TV Shut Down](https://pixabay.com/es/sound-effects/pel%c3%adculas-y-efectos-especiales-tv-shut-down-185446/))
- _cargando.png_ by Freepik ([loading](https://www.flaticon.com/free-icon/loading_3305879))
- _star-coin.png_ by Kason Koo ([moneda-estrella](https://www.flaticon.es/icono-gratis/moneda-estrella_17155297))
- _medkit.png_ by knik1985 ([medkit.png](https://opengameart.org/content/medkit-and-take-effect))
- _disquete.png_ by dinosoftlabs ([disquete](https://www.flaticon.es/icono-gratis/disquete_346091?term=disquete&page=1&position=19&origin=tag&related_id=346091))

## ⚖️ Aviso de uso

Este proyecto se ha realizado **únicamente con fines educativos** y de aprendizaje.  
No pretende comercializar ni redistribuir los contenidos originales de Godot ni de los autores de los recursos utilizados.

El contenido original de este proyecto está bajo el copyright de los autores de los diferentes recursos.
