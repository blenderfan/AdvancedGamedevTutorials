using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderConstants 
{
    public static readonly string VORONOI_KERNEL = "Voronoi";

    public static readonly string TABLE_SIZE_X = "TableSizeX";
    public static readonly string TABLE_SIZE_Y = "TableSizeY";
    public static readonly string BOUNDS = "Bounds";
    public static readonly string SITES = "Sites";
    public static readonly string RESULT = "Result";

    public static readonly string COLOR = "_Color";
    public static readonly string MAIN_TEX = "_MainTex";

    public static int TABLE_SIZE_X_ID = 0;
    public static int TABLE_SIZE_Y_ID = 0;
    public static int BOUNDS_ID = 0;
    public static int SITES_ID = 0;
    public static int RESULT_ID = 0;

    public static int COLOR_ID = 0;
    public static int MAIN_TEX_ID = 0;

    static ShaderConstants()
    {
        TABLE_SIZE_X_ID = Shader.PropertyToID(TABLE_SIZE_X);
        TABLE_SIZE_Y_ID = Shader.PropertyToID(TABLE_SIZE_Y);
        BOUNDS_ID = Shader.PropertyToID(BOUNDS);
        SITES_ID = Shader.PropertyToID(SITES);
        RESULT_ID = Shader.PropertyToID(RESULT);

        COLOR_ID = Shader.PropertyToID(COLOR);
        MAIN_TEX_ID = Shader.PropertyToID(MAIN_TEX);
    }

}
