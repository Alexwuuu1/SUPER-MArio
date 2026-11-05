using UnityEngine;

// Small pixel icons generated in code to keep the map independent of scene GUIDs.
public static class WorldMapSprites
{
    private static Texture2D[] icons;
    public static Texture2D Get(int index) {
        if(icons==null){
            string[][] patterns={
                new[]{"................",".......GG.......","......GGGG......",".....GGGGGG.....","....GGGGGGGG....","...GGGGGGGGGG...","..GGGGGGGGGGGG..",".....GGGGGG.....","....GGGGGGGG....","...GGGGGGGGGG...","..GGGGGGGGGGGG..",".GGGGGGGGGGGGGG.","......BB........","......BB........",".....BBBB.......","................"},
                new[]{"................","......SSSS......",".....SSSSSS.....","....SSSSSSSS....","...SSSSSSSSSS...","..SSSSSSSSSSSS..",".SSSSSKKKKSSSSS.","SSSSSKKKKKKSSSSS","SSSSKKKKKKKKSSSS","SSSSKKKKKKKKSSSS","SSSSKKKKKKKKSSSS","SSSSKKKKKKKKSSSS","SSSSKKKKKKKKSSSS","SSSSKKKKKKKKSSSS","SSSSKKKKKKKKSSSS","................"},
                new[]{"................","................",".....WW.........",".....WW.........",".....WW.........","....WWW.........","....WWW.........","...WWWW.........","...WWWW.........","..WWWWW.........","..WWWWW.........",".WWWWWWWW.......","BBBBBBBBBBBBBB..",".BBBBBBBBBBBB...","..BBBBBBBBBB....","~~~~~~~~~~~~~~~~"},
                new[]{"................",".S.S.S....S.S.S.",".SSSSS....SSSSS.",".SSSSSSSSSSSSSS.",".SSSSSSSSSSSSSS.",".SSKSSSSSSSSKSS.",".SSKSSSSSSSSKSS.",".SSSSSSSSSSSSSS.",".SSSSSSSSSSSSSS.",".SSSSSKKKKSSSSS.",".SSSSKKKKKKSSSS.",".SSSSKKKKKKSSSS.",".SSSSKKKKKKSSSS.",".SSSSKKKKKKSSSS.",".SSSSKKKKKKSSSS.","................"}
            };
            icons=new Texture2D[patterns.Length];
            for(int n=0;n<icons.Length;n++){
                var t=new Texture2D(16,16,TextureFormat.RGBA32,false);t.filterMode=FilterMode.Point;
                for(int y=0;y<16;y++)for(int x=0;x<16;x++){
                    char p=patterns[n][y][x];Color c=Color.clear;
                    if(p=='G')c=(x+y)%3==0?new Color(.43f,.76f,.28f):new Color(.1f,.42f,.22f);
                    if(p=='S')c=(x+y)%4==0?new Color(.67f,.63f,.57f):new Color(.37f,.4f,.43f);
                    if(p=='K')c=new Color(.07f,.11f,.15f);
                    if(p=='B')c=new Color(.48f,.27f,.13f);
                    if(p=='W')c=new Color(1,.94f,.71f);
                    if(p=='~')c=new Color(.36f,.8f,.9f);
                    t.SetPixel(x,15-y,c);
                }t.Apply();icons[n]=t;
            }
        }return icons[Mathf.Clamp(index,0,icons.Length-1)];
    }
}
