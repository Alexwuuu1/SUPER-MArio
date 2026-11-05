# Verificacion de Interfaz

Base: `f193a38`; dependencia Controles: `06d73fe` incorporada en la rama feature.
Unity 6000.3.16f1. Pruebas automatizadas en Play Mode mediante batchmode.

## Paso 01

- Revision: mapa extraido a MarioWorldMap y menu reducido a dos opciones coherentes con su navegacion.
- PASS: MarioChecks.Run, 19 comprobaciones de menu, mapa, desbloqueos, cuatro niveles, musica, moneda, temporizador, vidas, selector, victoria y Game Over.
- No se realizaron pruebas manuales de flechas/Enter y clics. Quedan pendientes para la revision humana.

## Paso 02

- Aplicado por separado y revisado: pausa persistente, continuar, reiniciar, ajustes y retorno al menu; bloqueo de entrada acuatico durante la pausa.
- Pruebas de Unity pendientes por indicacion del usuario; las realizara el responsable de integracion.
