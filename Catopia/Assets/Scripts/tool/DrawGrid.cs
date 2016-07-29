using UnityEngine;
using System;

public class DrawGrid : MonoBehaviour
{
    public GameObject prefab;
    void Start()
    {
        DrawLine();
    }

    void DrawLine()
    {
        var content = transform;
        var TILE_SIZE = 60;
        int iCount = 5;
        int jCount = 5;
        var screenPosX = 0;
        var screenPosY = 0;
        for (var i = 0; i < iCount + 1; i += 1)
        {
            var x = -i * TILE_SIZE + screenPosX;
            var y = i * TILE_SIZE / 2 - TILE_SIZE / 2 + screenPosY;
            var x1 = (jCount * TILE_SIZE) - i * TILE_SIZE + screenPosX;
            var y1 = jCount * TILE_SIZE / 2 + i * TILE_SIZE / 2 - TILE_SIZE / 2 + screenPosY;
            // graphics.moveTo(x, y);
            // graphics.lineTo(jCount * Map.TILE_SIZE - i * Map.TILE_SIZE + screenPos.x, jCount * Map.TILE_SIZE / 2 + i * Map.TILE_SIZE / 2 - Map.TILE_SIZE / 2 + screenPos.y);

           var tmp =  CatGameTools.AddChild(content, prefab);
            tmp.transform.position = new Vector3(x, y, 0);
        }



       /* for (var j = 0; j < jCount + 1; j += 1)
        {
            x = j * Map.TILE_SIZE + screenPos.x;
            y = j * Map.TILE_SIZE / 2 - Map.TILE_SIZE / 2 + screenPos.y;
            graphics.moveTo(j * Map.TILE_SIZE + screenPos.x, j * Map.TILE_SIZE / 2 - Map.TILE_SIZE / 2 + screenPos.y);
            graphics.lineTo(-iCount * Map.TILE_SIZE + j * Map.TILE_SIZE + screenPos.x, iCount * Map.TILE_SIZE / 2 + j * Map.TILE_SIZE / 2 - Map.TILE_SIZE / 2 + screenPos.y);
        }
        */
       

    }
}