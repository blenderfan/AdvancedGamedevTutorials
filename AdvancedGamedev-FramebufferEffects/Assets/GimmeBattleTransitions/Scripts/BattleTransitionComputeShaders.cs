using UnityEngine;

namespace GimmeBattleTransitions.Runtime
{
    [CreateAssetMenu(fileName = "BattleTransitionShaders", menuName = "Gimme/BattleTransitions/Battle Transition Shaders")]
    public class BattleTransitionComputeShaders : ScriptableObject
    {
        public ComputeShader diamond;
        public ComputeShader vertical;
        public ComputeShader horizontal;
        public ComputeShader spiral;

    }
}
