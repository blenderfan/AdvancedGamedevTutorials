using System;

namespace GimmeBattleTransitions.Runtime
{
    [Serializable]
    public struct SpiralParameters 
    {
        /// <summary>
        /// Amount of color that is being removed each frame
        /// </summary>
        public float fade;

        /// <summary>
        /// Speed of turning of the spiral
        /// </summary>
        public float angleSpeed;

        /// <summary>
        /// Speed at which pixels are drawn to the center of the screen
        /// </summary>
        public float centerSpeed;

        /// <summary>
        /// Offset (in time) from which pixels are sampled
        /// </summary>
        public float offset;
    }
}
