using System;

namespace GimmeBattleTransitions.Runtime
{
    [Serializable]
    public struct DiamondParameters 
    {
        /// <summary>
        /// Amount of color that is being removed per second
        /// </summary>
        public float fade;

        /// <summary>
        /// Offset from where pixels are sampled for the diamond effect - bigger values equate to faster smoothing
        /// </summary>
        public float offset;
    }
}
