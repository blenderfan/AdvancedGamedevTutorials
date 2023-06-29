using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShaderConstants
{

    //Constants
    public static readonly string FISH_SWARM_TRANSFORMS = "_FishSwarmTransforms";
    public static readonly string FISH_SWARM_DATA = "_FishSwarmData";
    public static readonly string FISH_SWARM_TARGET = "_FishSwarmTarget";
    public static readonly string SWARM_LOCAL_TO_WORLD = "_SwarmLocalToWorld";

    public static readonly string TIME = "_Time";
    public static readonly string PREVIOUS_TIME = "_PreviousTime";


    //IDs
    public static readonly int FISH_SWARM_TRANSFORMS_ID = 0;
    public static readonly int FISH_SWARM_DATA_ID = 0;
    public static readonly int FISH_SWARM_TARGET_ID = 0;
    public static readonly int SWARM_LOCAL_TO_WORLD_ID = 0;

    public static readonly int TIME_ID = 0;
    public static readonly int PREVIOUS_TIME_ID = 0;


    static ShaderConstants()
    {
        FISH_SWARM_TRANSFORMS_ID = Shader.PropertyToID(FISH_SWARM_TRANSFORMS);
        FISH_SWARM_DATA_ID = Shader.PropertyToID(FISH_SWARM_DATA);
        FISH_SWARM_TARGET_ID = Shader.PropertyToID(FISH_SWARM_TARGET);
        SWARM_LOCAL_TO_WORLD_ID = Shader.PropertyToID(SWARM_LOCAL_TO_WORLD);

        TIME_ID = Shader.PropertyToID(TIME);
        PREVIOUS_TIME_ID = Shader.PropertyToID(PREVIOUS_TIME);
    }
    
}
