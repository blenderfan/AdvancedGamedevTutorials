using UnityEngine;

namespace GimmeBattleTransitions.Runtime
{
    public class ComputeShaderConstants : MonoBehaviour
    {
        //Kernels

        public static readonly string DIAMOND = "Diamond";
        public static readonly string VERTICAL = "Vertical";
        public static readonly string HORIZONTAL = "Horizontal";
        public static readonly string SPIRAL = "Spiral";

        //Keywords
        public static readonly ShaderWord RESULT = new ShaderWord("Result");
        public static readonly ShaderWord COPY = new ShaderWord("Copy");
        public static readonly ShaderWord TIME = new ShaderWord("Time");
        public static readonly ShaderWord OFFSET = new ShaderWord("Offset");

        public static readonly ShaderWord SCANLINE = new ShaderWord("Scanline");
        public static readonly ShaderWord FADE = new ShaderWord("Fade");
        public static readonly ShaderWord NOISE = new ShaderWord("Noise");
        public static readonly ShaderWord SATURATE = new ShaderWord("Saturate");
        public static readonly ShaderWord LEFT_OFFSET = new ShaderWord("LeftOffset");
        public static readonly ShaderWord RIGHT_OFFSET = new ShaderWord("RightOffset");
        public static readonly ShaderWord ANGLE_SPEED = new ShaderWord("AngleSpeed");
        public static readonly ShaderWord CENTER_SPEED = new ShaderWord("CenterSpeed");
    }
}
