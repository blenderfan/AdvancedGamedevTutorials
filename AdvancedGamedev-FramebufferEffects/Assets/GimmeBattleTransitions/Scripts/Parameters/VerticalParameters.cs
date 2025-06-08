using System;

namespace GimmeBattleTransitions.Runtime
{
    [Serializable]
    public struct VerticalParameters
    {
        /// <summary>
        /// Amount of color that is being removed each frame
        /// </summary>
        public float fade;

        /// <summary>
        /// Start (in seconds) when the vertical scan should be on screen
        /// </summary>
        public float scanStart;
        /// <summary>
        /// Speed of the vertical scan (in UV per second)
        /// </summary>
        public float scanSpeed;

        /// <summary>
        /// Noisiness of the color fade
        /// </summary>
        public float noisiness;

        /// <summary>
        /// Scale of the noise used in the color fade
        /// </summary>
        public float noiseScale;

        /// <summary>
        /// Saturation increase per second at the beginning
        /// </summary>
        public float saturateIncrease;

        /// <summary>
        /// Time of the saturation increase in seconds
        /// </summary>
        public float saturateDuration;

    }
}
