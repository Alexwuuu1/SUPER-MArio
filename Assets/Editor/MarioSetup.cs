using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MarioSetup
{
    private static GameObject root;
    private static GameObject Prefab(string name,float x,float y,Transform parent=null) {
        var asset=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/"+name+".prefab");
        if(asset==null)throw new Exception("Missing prefab: "+name);
        var go=(GameObject)PrefabUtility.InstantiatePrefab(asset);go.transform.position=new Vector3(x,y,0);if(parent!=null)go.transform.SetParent(parent);return go;
    }
    private static GameObject Tile(string sprite,float x,float y,float w=1,float h=1,Color? color=null,bool solid=true) {
        var go=new GameObject(sprite);go.transform.position=new Vector3(x,y,0);
        var sr=go.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/"+sprite+".png");
        sr.color=color??Color.white;sr.sortingOrder=1;
        go.transform.localScale=new Vector3(w/sr.sprite.bounds.size.x,h/sr.sprite.bounds.size.y,1);
        if(solid)go.AddComponent<BoxCollider2D>();if(root!=null)go.transform.SetParent(root.transform);return go;
    }
    private static void Floor(string tile,int from,int to,int y=0){for(int x=from;x<=to;x++){Tile(tile,x,y);Tile(tile,x,y-1);}}
    private static void Column(string tile,int x,int top){for(int y=1;y<=top;y++)Tile(tile,x,y);}
    private static void Save(int stage){EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),"Assets/Scenes/1-"+stage+".unity");}
    public static void Terrain() {
        Directory.CreateDirectory("Assets/Scenes");
        for(int stage=2;stage<=4;stage++) {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            root=new GameObject("Escenario");
            var c=new GameObject("Main Camera");c.tag="MainCamera";var cam=c.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=7;
            cam.backgroundColor=stage==2?new Color(.015f,.025f,.06f):stage==3?new Color(.02f,.18f,.5f):new Color(.07f,.025f,.035f);
            c.transform.position=new Vector3(12,6,-10);c.AddComponent<AudioListener>();
            Prefab("Mario",3,2);
            if(stage==2){
                Floor("UndergroundBlock",0,79);Floor("UndergroundBlock",83,119);Floor("UndergroundBlock",124,166);Floor("UndergroundBlock",171,199);
                for(int x=0;x<198;x++)Tile("UndergroundBrick",x,12);
                for(int y=1;y<12;y++)Tile("UndergroundBlock",0,y);
                for(int x=10;x<31;x++)Tile("UndergroundBrick",x,5);
                for(int x=42;x<58;x++)Tile("UndergroundBrick",x,5);
                for(int x=64;x<73;x++)Column("UndergroundBlock",x,(x-64)%4+1);
                for(int x=89;x<99;x++)Tile("UndergroundBrick",x,5);
                for(int x=109;x<116;x++)Tile("UndergroundBrick",x,8);
                for(int x=133;x<142;x++)Column("UndergroundBlock",x,Mathf.Min(x-132,5));
                for(int x=179;x<194;x++)Column("UndergroundBlock",x,Mathf.Min((x-177)/2,6));
                Tile("PipeBottom",197,2,2,3);Tile("PipeTop",197,4,2,1);
            }else if(stage==3){
                Floor("UndergroundBlock",0,159);
                for(int x=0;x<160;x++)Tile("UndergroundBrick",x,12,1,1,new Color(.3f,.65f,1));
                for(int x=22;x<27;x++)Column("UndergroundBlock",x,3);
                for(int x=42;x<48;x++)Column("UndergroundBlock",x,5);
                for(int x=61;x<66;x++)Column("UndergroundBlock",x,2);
                for(int x=84;x<89;x++)Column("UndergroundBlock",x,5);
                for(int x=113;x<119;x++)Column("UndergroundBlock",x,3);
                for(int x=135;x<142;x++)Column("UndergroundBlock",x,5);
                Tile("PipeBottom",156,3,3,5);Tile("PipeTop",156,6,3,1);
            }else{
                Floor("HardBlock",0,17);Floor("HardBlock",29,44);Floor("HardBlock",60,77);Floor("HardBlock",91,102);Floor("HardBlock",117,127);
                for(int x=0;x<=127;x++)Tile("UndergroundBrick",x,12,1,1,new Color(.65f,.4f,.4f));
                for(int x=0;x<127;x+=8){Tile("UndergroundBrick",x,6,1,11,new Color(.25f,.17f,.2f),false);}
                for(int x=10;x<15;x++)Tile("HardBlock",x,3);
                for(int x=33;x<39;x++)Tile("HardBlock",x,4);
                for(int x=66;x<72;x++)Tile("HardBlock",x,4);
                for(int x=95;x<100;x++)Tile("HardBlock",x,3);
            }
            Save(stage);
        }
        AssetDatabase.SaveAssets();
    }
    public static void Gameplay(){
        for(int stage=2;stage<=4;stage++){
            EditorSceneManager.OpenScene("Assets/Scenes/1-"+stage+".unity");root=new GameObject("Jugabilidad");
            if(stage==2){
                foreach(int x in new[]{18,28,47,56,92,102,129,150,160})Prefab(x%2==0?"Goomba":"Koopa",x,1.5f,root.transform);
                for(int x=12;x<30;x+=2)Prefab("Coin",x,7,root.transform);
                for(int x=45;x<57;x+=2)Prefab("Coin",x,7,root.transform);
                foreach(int x in new[]{35,59,102,145})Prefab("MysteryBlock",x,4,root.transform);
                Prefab("MagicMushroom",7,1.5f,root.transform);
            }else if(stage==3){
                GameObject.FindGameObjectWithTag("Player").AddComponent<WaterMotion>();
                foreach(int x in new[]{16,35,54,73,96,123,146}){
                    var fish=Tile("Goomba_Walk1",x,5+(x%4),1.1f,1,new Color(1,.45f,.4f),false);
                    fish.AddComponent<CircleCollider2D>().isTrigger=true;fish.AddComponent<LevelHazard>();var motion=fish.AddComponent<LevelMotion>();motion.travel=new Vector3(4,1.3f,0);motion.speed=.8f+x*.003f;
                }
                for(int x=9;x<151;x+=5)Prefab("Coin",x,3+(x%6),root.transform);
                for(int x=17;x<150;x+=19){for(int y=1;y<4;y++)Tile("Bush1",x,y,1,1,new Color(1,.4f,.5f),false);}
            }else{
                foreach(int x in new[]{18,45,78,103}){
                    int width=x==103?14:x==45?15:12;
                    var lava=Tile("HardBlock",x+width/2f,-.15f,width,1,new Color(1,.2f,.04f),false);lava.name="Lava";lava.AddComponent<BoxCollider2D>().isTrigger=true;lava.AddComponent<LevelHazard>().instant=true;
                }
                foreach(int x in new[]{21,25,49,54,82,87}){var platform=Tile("HardBlock",x,2,3,.6f);var motion=platform.AddComponent<LevelMotion>();motion.travel=new Vector3(0,1.4f,0);motion.speed=1.1f;}
                foreach(int x in new[]{13,36,69,97}){
                    var pivot=new GameObject("Barra de fuego");pivot.transform.position=new Vector3(x,4.6f,0);pivot.AddComponent<LevelMotion>().rotate=true;pivot.GetComponent<LevelMotion>().speed=60;
                    for(int n=1;n<5;n++){var f=Tile("Coin",x+n*.65f,4.6f,.5f,.5f,new Color(1,.3f,.03f),false);f.transform.SetParent(pivot.transform);f.AddComponent<CircleCollider2D>().isTrigger=true;f.AddComponent<LevelHazard>();}
                }
                for(int x=7;x<96;x+=7)Prefab("Coin",x,6,root.transform);
                Prefab("MagicMushroom",5,1.5f,root.transform);
                var bridge=new GameObject("Puente final");for(int x=103;x<118;x++){var t=Tile("Brick",x,1);t.transform.SetParent(bridge.transform);}
                var boss=Tile("Koopa_Walk1",110,3,2.4f,3,new Color(.5f,1,.3f),false);boss.name="Bowser";boss.AddComponent<BoxCollider2D>().isTrigger=true;boss.AddComponent<LevelHazard>();
                var flame=Tile("Coin",0,-30,.9f,.7f,new Color(1,.25f,0),false);flame.name="Proyectil";flame.AddComponent<CircleCollider2D>().isTrigger=true;flame.AddComponent<LevelHazard>();flame.SetActive(false);boss.AddComponent<CastleBoss>().flamePrefab=flame;
                var exit=Tile("Castle",124,3,3,4,Color.white,false);exit.AddComponent<BoxCollider2D>().isTrigger=true;exit.AddComponent<LevelPortal>().finalExit=true;exit.SetActive(false);
                var axe=Tile("HardBlock",119,2,1,1,new Color(1,.85f,.1f),false);axe.name="Mecanismo del puente";axe.AddComponent<BoxCollider2D>().isTrigger=true;var lever=axe.AddComponent<CastleAxe>();lever.bridge=bridge;lever.boss=boss;lever.exit=exit;
            }
            Save(stage);
        }
        AssetDatabase.SaveAssets();
    }
    public static void Presentation(){
        for(int stage=2;stage<=4;stage++){
            EditorSceneManager.OpenScene("Assets/Scenes/1-"+stage+".unity");
            var cam=Camera.main.gameObject;cam.AddComponent<LevelCamera>().end=stage==2?200:stage==3?160:128;
            root=new GameObject("Detalles");
            if(stage==3){for(int x=4;x<155;x+=6){var bubble=Tile("Coin",x,4+(x%6),.16f,.2f,new Color(.65f,.9f,1,.5f),false);bubble.AddComponent<LevelMotion>().travel=new Vector3(0,2,0);}}
            if(stage==4){for(int x=7;x<123;x+=12){Tile("Brick",x,7,.6f,1,new Color(.3f,.2f,.2f),false);var fire=Tile("Coin",x,7.8f,.6f,.9f,new Color(1,.4f,.03f),false);fire.AddComponent<LevelMotion>().travel=new Vector3(0,.14f,0);}}
            Save(stage);
        }
    }
    public static void Integration(){
        for(int stage=2;stage<=4;stage++){
            EditorSceneManager.OpenScene("Assets/Scenes/1-"+stage+".unity");root=new GameObject("Transiciones");
            if(stage<4){var door=Tile("PipeTop",stage==2?197:156,stage==2?6:8,2,2,Color.white,false);door.name="Salida al siguiente nivel";door.AddComponent<BoxCollider2D>().isTrigger=true;door.AddComponent<LevelPortal>();}
            var barrier=new GameObject("Death Barrier");barrier.transform.position=new Vector3(100,-6);barrier.AddComponent<BoxCollider2D>().size=new Vector2(250,2);barrier.GetComponent<BoxCollider2D>().isTrigger=true;barrier.AddComponent<DeathBarrier>();
            Save(stage);
        }
        EditorSceneManager.OpenScene("Assets/Scenes/1-1.unity");
        foreach(var flag in UnityEngine.Object.FindObjectsByType<FlagPole>(FindObjectsSortMode.None)){flag.nextWorld=1;flag.nextStage=2;EditorUtility.SetDirty(flag);}
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        foreach(string name in new[]{"Menu","GameOver","Victory"}){
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);root=new GameObject("Decoracion");
            var c=new GameObject("Main Camera");c.tag="MainCamera";var cam=c.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=7;cam.backgroundColor=name=="Menu"?new Color(.13f,.3f,.55f):new Color(.025f,.03f,.07f);c.transform.position=new Vector3(12,6,-10);c.AddComponent<AudioListener>();
            Floor("GroundBlock",-3,32);Tile("Hill1",5,2,7,4,null,false);Tile("Castle",24,3,5,5,null,false);
            if(name=="Menu"){var manager=new GameObject("Game Manager");manager.AddComponent<GameManager>();manager.AddComponent<MarioScreen>();manager.AddComponent<MarioAudio>();}
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),"Assets/Scenes/"+name+".unity");
        }
        var names=new[]{"Menu","1-1","1-2","1-3","1-4","GameOver","Victory"};
        EditorBuildSettings.scenes=names.Select(n=>new EditorBuildSettingsScene("Assets/Scenes/"+n+".unity",true)).ToArray();
        PlayerSettings.companyName="Alexwuuu1";PlayerSettings.productName="Super Mario - La Forja de Bowser";
        PlayerSettings.defaultScreenWidth=1280;PlayerSettings.defaultScreenHeight=720;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;
        AssetDatabase.SaveAssets();
    }
    public static void Build(){
        var result=BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Where(s=>s.enabled).Select(s=>s.path).ToArray(),"Builds/Windows/SuperMario.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);
        if(result.summary.result!=BuildResult.Succeeded)throw new Exception("Build failed: "+result.summary.result);
        Debug.Log("MARIO_BUILD_OK "+result.summary.totalSize);
    }
}

