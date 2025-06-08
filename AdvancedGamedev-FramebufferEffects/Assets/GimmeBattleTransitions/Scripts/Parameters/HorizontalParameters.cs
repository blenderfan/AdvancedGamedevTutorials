using System;

namespace GimmeBattleTransitions.Runtime
{
    [Serializable]
    public struct HorizontalParameters
    {
        /// <summary>
        /// Amount of color that is being removed per second
        /// </summary>
        public float fade;

        /// <summary>
        /// Time needed to scan from bottom to top
        /// </summary>
        public float scanDuration;

        /// <summary>
        /// Additional sample to the left of the pixel for smoothing and "Waterfall Effect" (in UV Space)
        /// </summary>
        public float leftSmoothOffset;

        /// <summary>
        /// Additional sample to the right of the pixel for smoothing and "Waterfall Effect" (in UV Space)
        /// </summary>
        public float rightSmoothOffset;

        /// <summary>
        /// Sample to the top, from which the main pixel color is sampled (in UV Space)
        /// </summary>
        public float offset;
    }
}
