using UnityEngine;

namespace GimmeBattleTransitions.Runtime
{
    public class ComputeShaderUtility
    {
        /// <summary>
        /// Given the problem size in width, height and depth, the method returns
        /// the amount of thread groups required in each dimension for the given shader
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="kernelIdx"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static Vector3Int CalculateThreadGroups(ComputeShader shader, int kernelIdx, int width, int height, int depth)
        {
            Vector3Int result = new Vector3Int();
            shader.GetKernelThreadGroupSizes(kernelIdx, out uint threadsX, out uint threadsY, out uint threadsZ);
            result.x = width / (int)threadsX;
            result.y = height / (int)threadsY;
            result.z = depth / (int)threadsZ;

            if (width % threadsX != 0) result.x++;
            if (height % threadsY != 0) result.y++;
            if (depth % threadsZ != 0) result.z++;

            return result;
        }

        /// <summary>
        /// Given the problem size in width and height, the method returns
        /// the amount of thread groups required in each dimension for the given shader
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="kernelIdx"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static Vector3Int CalculateThreadGroups(ComputeShader shader, int kernelIdx, int width, int height)
        {
            Vector3Int result = new Vector3Int();
            shader.GetKernelThreadGroupSizes(kernelIdx, out uint threadsX, out uint threadsY, out uint threadsZ);
            result.x = width / (int)threadsX;
            result.y = height / (int)threadsY;
            result.z = (int)threadsZ;

            if (width % threadsX != 0) result.x++;
            if (height % threadsY != 0) result.y++;

            return result;
        }
    }
}
