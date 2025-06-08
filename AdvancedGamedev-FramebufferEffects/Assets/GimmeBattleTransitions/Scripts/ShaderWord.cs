using UnityEngine;

namespace GimmeBattleTransitions.Runtime
{
    public struct ShaderWord
    {
        public int id;

        public ShaderWord(string name)
        {
            this.id = Shader.PropertyToID(name);
        }

    }
}
