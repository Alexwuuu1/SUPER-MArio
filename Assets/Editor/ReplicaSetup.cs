using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ReplicaSetup
{
    [Serializable] private class Layouts {public string[] underground;public string[] water;}
    private static Transform group;
    private static GameObject Tile(string sprite,float x,float y,float w=1,float h=1,Color? tint=null,bool solid=true){
        var go=new GameObject(sprite);go.transform.SetParent(group);go.transform.position=new Vector3(x,y);
        var sr=go.AddComponent<SpriteRenderer>();sr.sprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/"+sprite+".png");sr.color=tint??Color.white;sr.sortingOrder=1;
        go.transform.localScale=new Vector3(w/sr.sprite.bounds.size.x,h/sr.sprite.bounds.size.y,1);if(solid)go.AddComponent<BoxCollider2D>();return go;
    }
    private static GameObject Prefab(string name,float x,float y){var go=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/"+name+".prefab"));go.transform.position=new Vector3(x,y);go.transform.SetParent(group);return go;}
    private static BlockHit Brick(float x,float y){var go=Prefab("UndergroundBrick",x,y);var hit=go.GetComponent<BlockHit>();if(hit==null)hit=go.AddComponent<BlockHit>();hit.emptyBlock=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UndergroundBlock.png");return hit;}
    private static void Fresh(int stage){
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);group=new GameObject("Recreacion del mapa original").transform;
        var c=new GameObject("Main Camera");c.tag="MainCamera";var cam=c.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=7;cam.backgroundColor=stage==2?Color.black:new Color(32/255f,56/255f,236/255f);c.transform.position=new Vector3(12.5f,6,-10);c.AddComponent<AudioListener>();c.AddComponent<LevelCamera>().end=192;
        Prefab("Mario",3,stage==2?8:5);if(stage==3)UnityEngine.Object.FindFirstObjectByType<Player>().gameObject.AddComponent<WaterMotion>();
    }
    private static void Grid(string[] rows,bool water){
        for(int r=0;r<rows.Length;r++)for(int x=0;x<rows[r].Length;x++){
            int y=11-r;char t=rows[r][x];
            if(!water&&y==11&&x>=6&&x<=137)t='B';
            if(!water&&x==10&&y==4)t='Q';
            if(!water&&t=='C'&&((x==29&&y==5)||(x==46&&y==6)||(x==69&&y==5)||(x==73&&y==5)||(x==149&&y==5)||(x==146&&y==1)))t='.';
            switch(t){
                case 'G':case 'H':Tile("UndergroundBlock",x,y);break;
                case 'B':Brick(x,y);break;
                case 'S':Tile("UndergroundBlock",x,y,1,1,new Color(.25f,1,.2f));break;
                case 'Q':var block=Prefab("MysteryBlock",x,y).GetComponent<BlockHit>();block.item=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/"+(x==10?"MagicMushroom":"BlockCoin")+".prefab");block.maxHits=1;break;
                case 'C':Prefab("Coin",x,y);break;
            }
        }
    }
    private static void Pipe(float x,float top,bool plant){
        for(int y=1;y<top;y++)Tile("PipeBottom",x,y,2,1);
        Tile("PipeTop",x,top,2,1);
        if(plant){var p=Tile("Bush1",x,top-.3f,1.2f,1.4f,null,false);p.name="Planta pirana";p.AddComponent<BoxCollider2D>().isTrigger=true;p.AddComponent<LevelHazard>();p.AddComponent<RetroCreature>().kind=RetroCreature.Kind.Piranha;}
    }
    private static void Exit(float x,float y){var go=new GameObject("Salida al siguiente nivel");go.transform.SetParent(group);go.transform.position=new Vector3(x,y);go.AddComponent<BoxCollider2D>().size=new Vector2(1.5f,2);go.GetComponent<BoxCollider2D>().isTrigger=true;go.AddComponent<LevelPortal>();}
    private static void Barrier(){var go=new GameObject("Limite inferior");go.transform.position=new Vector3(96,-19);var hit=go.AddComponent<BoxCollider2D>();hit.size=new Vector2(230,2);hit.isTrigger=true;go.AddComponent<DeathBarrier>();}
    public static void Underground(){
        var data=JsonUtility.FromJson<Layouts>(File.ReadAllText("Assets/Editor/ReferenceLayouts.json"));Fresh(2);Grid(data.underground,false);
        foreach(float x in new[]{16.5f,17.5f,29.5f,62,64,73,74,99,100,101,113,135,136,137})Prefab("Goomba",x,1.1f);
        foreach(float x in new[]{44,46,59,146})Prefab("Koopa",x,1.3f);
        Prefab("Koopa",89,12.3f);
        var multi=Brick(29,4);multi.item=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BlockCoin.prefab");multi.maxHits=10;
        foreach(var blockPosition in new[]{new Vector2(46,5),new Vector2(69,4),new Vector2(149,5)}){
            foreach(var sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))if(sr.transform.position==new Vector3(blockPosition.x,blockPosition.y))UnityEngine.Object.DestroyImmediate(sr.gameObject);
            var b=Brick(blockPosition.x,blockPosition.y);b.item=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/"+(blockPosition.x==46?"Starman":"MagicMushroom")+".prefab");b.maxHits=1;
        }
        Prefab("1upMushroom",89,12.6f);
        Pipe(103.5f,3,false);Pipe(109.5f,4,true);Pipe(115.5f,2,true);
        for(int n=0;n<3;n++){
            var lift=Tile("HardBlock",140.5f,1+n*4,3,.4f,new Color(1,.72f,.4f));lift.name="Elevador descendente";lift.AddComponent<LevelLift>().descending=true;
            var up=Tile("HardBlock",155.5f,1+n*4,3,.4f,new Color(1,.72f,.4f));up.name="Elevador ascendente";up.AddComponent<LevelLift>();
        }
        for(int y=4;y<=11;y++)Tile("PipeBottom",168.5f,y,2,1);
        var mouth=Tile("PipeTop",166,4,2,1);mouth.transform.rotation=Quaternion.Euler(0,0,90);Exit(165.7f,4);
        for(int x=178;x<=186;x+=4){Pipe(x+.5f,2,false);Exit(x+.5f,3.4f);}
        // Sala opcional: 27 monedas y retorno a la tercera tuberia.
        for(int x=100;x<=116;x++){Tile("UndergroundBlock",x,-14);Tile("UndergroundBlock",x,-15);Tile("UndergroundBrick",x,-3);}
        for(int y=-13;y<=-4;y++){Tile("UndergroundBrick",100,y);Tile("UndergroundBrick",116,y);}
        for(int x=103;x<=111;x++)for(int y=-12;y<=-8;y+=2)Prefab("Coin",x,y);
        for(int x=103;x<=111;x++)Tile("UndergroundBrick",x,-9);
        var destination=new GameObject("Entrada sala de monedas");destination.transform.position=new Vector3(101.5f,-12);
        var entry=Tile("PipeTop",103.5f,3.7f,1.6f,.5f,null,false);entry.name="Bajar a sala de monedas [S]";entry.AddComponent<BoxCollider2D>().isTrigger=true;entry.AddComponent<Pipe>().connection=destination.transform;
        var returning=new GameObject("Retorno sala de monedas");returning.transform.position=new Vector3(115.5f,3.2f);
        Tile("PipeBottom",114.5f,-12,2,3);Tile("PipeTop",114.5f,-10,2,1);
        var leave=new GameObject("Regresar al recorrido [S]");leave.transform.position=new Vector3(114.5f,-9);leave.AddComponent<BoxCollider2D>().isTrigger=true;var pipe=leave.AddComponent<Pipe>();pipe.connection=returning.transform;pipe.exitDirection=Vector3.up;
        Barrier();EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),"Assets/Scenes/1-2.unity");Debug.Log("REPLICA_UNDERGROUND_OK");
    }
    public static void Water(){
        var data=JsonUtility.FromJson<Layouts>(File.ReadAllText("Assets/Editor/ReferenceLayouts.json"));Fresh(3);Grid(data.water,true);
        for(int x=0;x<192;x++)Tile("UndergroundBrick",x,11.8f,1,.22f,new Color(.65f,.85f,1),false);
        foreach(var coral in new[]{new Vector2(11,3),new Vector2(33,5),new Vector2(42,6),new Vector2(50,4),new Vector2(89,3),new Vector2(102,6),new Vector2(120,4),new Vector2(147,3),new Vector2(149,3),new Vector2(173,4)}){
            for(int y=1;y<=coral.y;y++){var go=Tile("Bush1",coral.x,y,1,1,null,false);go.AddComponent<RetroCreature>().kind=RetroCreature.Kind.Coral;}
        }
        foreach(var p in new[]{new Vector2(22,1.5f),new Vector2(46,3.5f),new Vector2(55,2.5f),new Vector2(83,8.5f),new Vector2(94,5.5f),new Vector2(105,1.5f)}){
            var squid=Tile("Fish",p.x,p.y,1.1f,1.3f,null,false);squid.name="Blooper";squid.AddComponent<CircleCollider2D>().isTrigger=true;squid.AddComponent<LevelHazard>();squid.AddComponent<RetroCreature>().kind=RetroCreature.Kind.Blooper;
        }
        foreach(var p in new[]{new Vector2(76,5),new Vector2(79,8),new Vector2(82,4),new Vector2(95,8),new Vector2(98,11),new Vector2(101,3),new Vector2(117,10),new Vector2(128,3),new Vector2(137,5),new Vector2(145,2),new Vector2(150,9),new Vector2(157,6),new Vector2(164,10),new Vector2(167,2),new Vector2(175,4),new Vector2(183,8),new Vector2(186,6)}){
            var fish=Tile("Fish",p.x,p.y,1,1,null,false);fish.name="Cheep Cheep";fish.AddComponent<CircleCollider2D>().isTrigger=true;fish.AddComponent<LevelHazard>();var motion=fish.AddComponent<LevelMotion>();motion.travel=new Vector3(3.4f,.5f,0);motion.speed=.6f+p.x*.002f;
        }
        for(int x=4;x<190;x+=7){var bubble=Tile("Coin",x,6,.15f,.18f,new Color(.6f,.9f,1,.6f),false);bubble.AddComponent<LevelMotion>().travel=new Vector3(0,3,0);}
        var mouth=Tile("PipeTop",189,6,2,1);mouth.transform.rotation=Quaternion.Euler(0,0,90);Tile("PipeBottom",191,6,2,2);Exit(187.7f,6);
        Barrier();GameObject.Find("Limite inferior").transform.position=new Vector3(96,-5);EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),"Assets/Scenes/1-3.unity");Debug.Log("REPLICA_WATER_OK");
    }
    public static void CastlePolish(){
        EditorSceneManager.OpenScene("Assets/Scenes/1-4.unity");group=new GameObject("Detalles de la forja").transform;
        foreach(var sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))if(sr.name=="Lava")sr.gameObject.AddComponent<LavaSurface>();
        for(int x=3;x<126;x+=8){
            for(int y=1;y<=11;y++){var stone=Tile("Brick",x,y,1,1,new Color(.3f,.24f,.26f),false);stone.GetComponent<SpriteRenderer>().sortingOrder=-2;}
            Tile("HardBlock",x,1,2,1,new Color(.45f,.35f,.35f),false).GetComponent<SpriteRenderer>().sortingOrder=-2;
        }
        for(int x=8;x<125;x+=18){Tile("Flag",x,8.5f,1.5f,3,new Color(.85f,.18f,.1f),false).GetComponent<SpriteRenderer>().sortingOrder=-1;}
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),"Assets/Scenes/1-4.unity");
    }
    public static void MapArt(){
        EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");var screen=UnityEngine.Object.FindFirstObjectByType<MarioScreen>();
        screen.mapPlayer=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/Mario_Small_Idle.png");screen.mapCastle=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/Castle.png");screen.mapPipe=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/PipeTop.png");
        EditorUtility.SetDirty(screen);EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }
    public static void All(){Underground();Water();CastlePolish();MapArt();}
}
