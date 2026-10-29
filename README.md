# Super Mario - La Forja de Bowser

Proyecto de Unity **6000.3.16f1**, basado en el repositorio compartido para la actividad.

## Probar en Unity

1. Abrir el proyecto desde Unity Hub.
2. Abrir `Assets/Scenes/Menu.unity` y pulsar **Play**.
3. Elegir **1 jugador** para recorrer el mapa con desbloqueos, o **Selector de niveles** para probar directamente cualquier escenario con tres vidas y los contadores en cero.
4. Detener Play antes de cambiar de escena en el editor. Cada nivel tambien puede abrirse individualmente.

**Controles:** A/D o flechas para moverse y cambiar de dirección también en el aire; espacio para saltar o impulsarse al nadar; S para entrar en las tuberías de la sala de monedas; M para activar o silenciar la música. El volumen se puede ajustar con el deslizador. Ratón para los botones del menú, mapa y selector.

## Mejoras

- Menú principal interactivo y selector de los cuatro niveles.
- Mapa con caminos y desbloqueo progresivo entre escenarios.
- Música diferente por ambiente y efectos de salto, moneda, pisotón, powerup, daño y muerte.
- HUD de puntaje, monedas, tiempo y vidas; monedas y enemigos aportan puntaje.
- Escenas de Game Over y victoria, con reinicio de partida.
- Subterráneo basado en la distribución de World 1-2: columnas, bloques de objetos y monedas, tres tuberías, sala opcional de 27 monedas, ascensores en ambos sentidos y salida hacia el agua.
- Acuático basado en World 2-2: recorrido de 192 bloques, huecos, plataformas, pasos estrechos, coral, Cheep Cheeps y Bloopers, nado y salida hacia el castillo.
- **La Forja de Bowser:** escenario propio con tres zonas de lava, plataformas móviles, barras de fuego, antorchas y puente final. Al llegar al mecanismo dorado se elimina el puente y se habilita la salida.
- Cámara con seguimiento y límites en los tres escenarios añadidos.

## Estado de revisión

La distribución del subterráneo y del acuático se reconstruyó tomando como referencia los mapas de World 1-2 y World 2-2. Son recreaciones aproximadas del recorrido original, con físicas y comportamiento de enemigos adaptados a este proyecto de Unity. El castillo es una propuesta original.

Referencias de distribución: [World 1-2](https://www.nesmaps.com/maps/SuperMarioBrothers/SuperMarioBrosWorld1-2Map.html) y [World 2-2](https://www.nesmaps.com/maps/SuperMarioBrothers/SuperMarioBrosWorld2-2Map.html), mapas de Rick N. Bruns / NESMaps.

El ejecutable se generará después de revisar la jugabilidad.

## Proyecto base y créditos

Fork de [Jesusbautistah/SUPER-MArio](https://github.com/Jesusbautistah/SUPER-MArio), basado en el tutorial de Zigurous. Se conserva a continuación la información del proyecto original. Mario y sus elementos pertenecen a Nintendo; proyecto educativo.

# Super Mario Bros (2D)

> Super Mario Bros is a platformer game developed and published by Nintendo for the NES game console. Players control Mario as they traverse the Mushroom Kingdom to rescue Princess Peach from Bowser. They traverse side-scrolling stages while avoiding hazards such as enemies and pits with the aid of power-ups such as the Super Mushroom, Fire Flower, and Starman.

- **Topics**: Physics, Level Design, Game Feel
- **Version**: Unity 2021.3 (LTS)
- [**Download**](https://github.com/zigurous/unity-super-mario-tutorial/archive/refs/heads/main.zip)
- [**Video Playlist**](https://youtube.com/playlist?list=PLqlFiJjSZ2x1mrMpSQgYdRm8PyWRTg6He)
