using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayerCollisionChecks
{
    private static double ready;
    static PlayerCollisionChecks() { EditorApplication.playModeStateChanged += Changed; }
    public static void Run() {
        SessionState.SetBool("PlayerCollisionChecks", true);
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
        EditorApplication.EnterPlaymode();
    }
    private static void Changed(PlayModeStateChange state) {
        if(state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool("PlayerCollisionChecks", false)) return;
        ready = EditorApplication.timeSinceStartup + 2;
        EditorApplication.update += Test;
    }
    private static GameObject Box(Transform parent, Vector2 position, Vector2 size) {
        var go = new GameObject("Regression terrain"); go.transform.SetParent(parent); go.transform.position = position;
        go.AddComponent<BoxCollider2D>().size = size; return go;
    }
    private static void Require(bool condition, string message) { if(!condition) throw new Exception(message); Debug.Log("COLLISION_OK: " + message); }
    private static void Case(bool big, int direction, bool ceiling = false) {
        var fixture = new GameObject("Collision regression");
        Box(fixture.transform, new Vector2(520, 0), new Vector2(24, 1));
        if(ceiling) Box(fixture.transform, new Vector2(520, 4), new Vector2(5, 1));
        else {
            Box(fixture.transform, new Vector2(519, 1.5f), new Vector2(1, 2));
            Box(fixture.transform, new Vector2(521, 2), new Vector2(1, 3));
        }
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Mario.prefab");
        var mario = UnityEngine.Object.Instantiate(prefab, new Vector3(520, 1.03f), Quaternion.identity);
        var player = mario.GetComponent<Player>();
        if(big) player.Grow();
        var motor = mario.GetComponent<PlayerMovement>(); var body = mario.GetComponent<Rigidbody2D>();
        var input = typeof(PlayerMovement).GetMethod("ProcessInput", BindingFlags.Instance | BindingFlags.NonPublic);
        var fixedUpdate = typeof(PlayerMovement).GetMethod("FixedUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
        Physics2D.SyncTransforms();
        Action<float, bool, bool> step = (axis, pressed, held) => {
            input.Invoke(motor, new object[] {axis, pressed, held, Time.fixedDeltaTime});
            fixedUpdate.Invoke(motor, null); Physics2D.Simulate(Time.fixedDeltaTime);
        };
        for(int i=0; i<15; i++) step(0, false, false);
        Require(motor.grounded, "suelo detectado con Mario " + (big ? "grande" : "pequeno"));
        float maxY = body.position.y;
        for(int i=0; i<50; i++) {
            step(ceiling ? 0 : direction, i == 0, i < 25);
            maxY = Mathf.Max(maxY, body.position.y);
        }
        if(ceiling) Require(maxY <= 2.06f, "el techo real detiene la cabeza de Mario grande");
        else {
            Require(maxY > 4, "la pared lateral no cancela el salto");
            Require(direction > 0 ? body.position.x > 521.5f : body.position.x < 518.5f,
                "salir del hueco hacia " + (direction > 0 ? "la derecha" : "la izquierda") + " con Mario " + (big ? "grande" : "pequeno"));
        }
        UnityEngine.Object.DestroyImmediate(mario); UnityEngine.Object.DestroyImmediate(fixture);
    }
    private static void Test() {
        if(EditorApplication.timeSinceStartup < ready) return;
        EditorApplication.update -= Test;
        var oldMode = Physics2D.simulationMode;
        try {
            Physics2D.simulationMode = SimulationMode2D.Script;
            Case(false, 1); Case(false, -1); Case(true, 1); Case(true, -1); Case(true, 0, true);
            File.WriteAllText("../mario-colisiones-verificacion.txt", "PASS - saltos a ambos lados, Mario pequeno y grande, y techo real");
            Debug.Log("PLAYER_COLLISION_CHECKS_OK");
        } catch(Exception e) {
            File.WriteAllText("../mario-colisiones-verificacion.txt", "FAILED " + e);
            Debug.LogError(e); EditorApplication.Exit(1); return;
        } finally { Physics2D.simulationMode = oldMode; SessionState.SetBool("PlayerCollisionChecks", false); }
        EditorApplication.Exit(0);
    }
}
