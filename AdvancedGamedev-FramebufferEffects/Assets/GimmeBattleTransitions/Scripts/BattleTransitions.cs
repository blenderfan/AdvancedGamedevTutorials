using System.Collections;
using UnityEngine;

namespace GimmeBattleTransitions.Runtime
{

    public class BattleTransitions : MonoBehaviour
    {
        #region Public Fields

        public Camera cameraToCopy;

        public float nearOffset = 0.01f;

        public GameObject screenFXQuad;

        public DiamondParameters diamondDefaultParams;
        public HorizontalParameters horizontalDefaultParams;
        public VerticalParameters verticalDefaultParams;
        public SpiralParameters spiralDefaultParams;

        public BattleTransitionComputeShaders battleTransitionComputeShaders;

        #endregion

        #region Private Fields

        private RenderTexture screenRT = null;

        #endregion

        private Camera GetCamera()
        {
            Camera camera = null;
            if(this.cameraToCopy != null)
            {
                camera = this.cameraToCopy;
            } else
            {
                camera = this.GetComponentInParent<Camera>();
                if(camera == null)
                {
                    camera = Camera.main;
                }
            }


            if (camera == null)
            {
                Debug.LogError("[Battle Transitions]: Unable to find a suitable camera!");
            }

            return camera;
        }

        private RenderTexture CopyScreen()
        {
            Camera camera = this.GetCamera();

            int width = camera.scaledPixelWidth;
            int height = camera.scaledPixelHeight;

            if (this.screenRT != null)
            {
                RenderTexture.ReleaseTemporary(this.screenRT);
                Object.Destroy(this.screenRT);
            }
            this.screenRT = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            this.screenRT.depthStencilFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.D16_UNorm;
            this.screenRT.enableRandomWrite = true;
            this.screenRT.useMipMap = false;
            this.screenRT.autoGenerateMips = false;
            this.screenRT.Create();
            
            camera.targetTexture = this.screenRT;
            camera.Render();
            camera.targetTexture = null;

            var descriptor = this.screenRT.descriptor;
            var copiedTexture = RenderTexture.GetTemporary(descriptor);
            copiedTexture.enableRandomWrite = true;
            copiedTexture.useMipMap = false;
            copiedTexture.autoGenerateMips = false;
            copiedTexture.Create();

            Graphics.Blit(this.screenRT, copiedTexture);

            return copiedTexture;
        }

        private void SetupScreenQuad(RenderTexture texture)
        {
            var camera = this.GetCamera();
            float near = camera.nearClipPlane;

            float planeDist = near + this.nearOffset;
            var quadPos = camera.transform.position + camera.transform.forward * planeDist;

            this.screenFXQuad.transform.position = quadPos;

            this.SetScreenQuadTexture(texture);
            var topRight = camera.ViewportToWorldPoint(new Vector3(1, 1, planeDist));
            var bottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, planeDist));

            float planeWidth = Vector3.Project(topRight - bottomLeft, camera.transform.right).magnitude;
            float planeHeight = Vector3.Project(topRight - bottomLeft, camera.transform.up).magnitude;

            this.screenFXQuad.transform.localScale = new Vector3(planeWidth, planeHeight, 1.0f);

        }

        private void SetScreenQuadTexture(RenderTexture texture)
        {

            var quadRenderer = this.screenFXQuad.GetComponentInChildren<MeshRenderer>();
            var transitionMaterial = quadRenderer.sharedMaterial;

            transitionMaterial.SetTexture("_CameraTexture", texture);
        }

        public IEnumerator VerticalTransition(float duration, bool keepActive = true, VerticalParameters? parameters = null)
        {

            var copy = this.CopyScreen();
            this.SetupScreenQuad(this.screenRT);
            this.screenFXQuad.SetActive(true);

            VerticalParameters verticalParams = parameters ?? this.verticalDefaultParams;

            var shaderInstance = ComputeShader.Instantiate(this.battleTransitionComputeShaders.vertical);
            var shaderKernel = shaderInstance.FindKernel(ComputeShaderConstants.VERTICAL);

            var noiseBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, this.screenRT.height, 4);

            float[] noise = new float[this.screenRT.height];

            for(int i = 0; i < noise.Length; i++)
            {
                float percent = i / (float)noise.Length;
                float value = Mathf.PerlinNoise1D(percent * verticalParams.noisiness) - 0.5f;
                float value_noise = UnityEngine.Random.Range(-verticalParams.noiseScale * 0.25f, verticalParams.noiseScale * 0.25f);
                value *= verticalParams.noiseScale;
                value += value_noise;
                noise[i] = value;
            }

            noiseBuffer.SetData(noise);

            var groups = ComputeShaderUtility.CalculateThreadGroups(shaderInstance, shaderKernel, this.screenRT.width, this.screenRT.height);

            shaderInstance.SetBuffer(shaderKernel, ComputeShaderConstants.NOISE.id, noiseBuffer);

            float timer = 0.0f;
            while(timer < duration)
            {
                float scanline = verticalParams.scanStart + verticalParams.scanSpeed * timer;
                float saturate = verticalParams.saturateIncrease;
                if (timer > verticalParams.saturateDuration) saturate = 0.0f;

                shaderInstance.SetFloat(ComputeShaderConstants.FADE.id, verticalParams.fade * Time.unscaledDeltaTime);
                shaderInstance.SetFloat(ComputeShaderConstants.SCANLINE.id, scanline);
                shaderInstance.SetFloat(ComputeShaderConstants.SATURATE.id, saturate * Time.unscaledDeltaTime);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.COPY.id, copy);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.RESULT.id, this.screenRT);
                shaderInstance.Dispatch(shaderKernel, groups.x, groups.y, groups.z);
                
                Graphics.Blit(this.screenRT, copy);

                yield return null;

                timer += Time.unscaledDeltaTime;
            }

            RenderTexture.ReleaseTemporary(copy);
            Destroy(shaderInstance);
            this.screenFXQuad.SetActive(keepActive);
        }

        public IEnumerator DiamondTransition(float duration, bool keepActive = true, DiamondParameters? parameters = null)
        {
            var copy = this.CopyScreen();
            this.SetupScreenQuad(this.screenRT);
            this.screenFXQuad.SetActive(true);

            var diamondParams = parameters ?? this.diamondDefaultParams;

            var shaderInstance = ComputeShader.Instantiate(this.battleTransitionComputeShaders.diamond);
            var shaderKernel = shaderInstance.FindKernel(ComputeShaderConstants.DIAMOND);

            var groups = ComputeShaderUtility.CalculateThreadGroups(shaderInstance, shaderKernel, this.screenRT.width, this.screenRT.height);

            shaderInstance.SetFloat(ComputeShaderConstants.OFFSET.id, diamondParams.offset);

            float timer = 0.0f;
            while(timer < duration)
            {
                shaderInstance.SetFloat(ComputeShaderConstants.FADE.id, diamondParams.fade * Time.unscaledDeltaTime);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.COPY.id, copy);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.RESULT.id, this.screenRT);
                shaderInstance.Dispatch(shaderKernel, groups.x, groups.y, groups.z);

                Graphics.Blit(this.screenRT, copy);

                yield return null;

                timer += Time.unscaledDeltaTime;
            }

            RenderTexture.ReleaseTemporary(copy);
            Destroy(shaderInstance);
            this.screenFXQuad.SetActive(keepActive);
        }

        public IEnumerator HorizontalTransition(float duration, bool keepActive = true, HorizontalParameters? parameters = null)
        {
            var copy = this.CopyScreen();
            this.SetupScreenQuad(this.screenRT);
            this.screenFXQuad.SetActive(true);

            var horizontalParams = parameters ?? this.horizontalDefaultParams;

            var shaderInstance = ComputeShader.Instantiate(this.battleTransitionComputeShaders.horizontal);
            var shaderKernel = shaderInstance.FindKernel(ComputeShaderConstants.HORIZONTAL);

            var groups = ComputeShaderUtility.CalculateThreadGroups(shaderInstance, shaderKernel, this.screenRT.width, this.screenRT.height);

            shaderInstance.SetFloat(ComputeShaderConstants.LEFT_OFFSET.id, horizontalParams.leftSmoothOffset);
            shaderInstance.SetFloat(ComputeShaderConstants.RIGHT_OFFSET.id, horizontalParams.rightSmoothOffset);
            shaderInstance.SetFloat(ComputeShaderConstants.OFFSET.id, horizontalParams.offset);

            float timer = 0.0f;
            while(timer < duration)
            {
                float percent = Mathf.Clamp01(timer / (duration * horizontalParams.scanDuration));

                shaderInstance.SetFloat(ComputeShaderConstants.FADE.id, horizontalParams.fade * Time.unscaledDeltaTime);
                shaderInstance.SetFloat(ComputeShaderConstants.SCANLINE.id, percent);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.COPY.id, copy);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.RESULT.id, this.screenRT);
                shaderInstance.Dispatch(shaderKernel, groups.x, groups.y, groups.z);

                Graphics.Blit(this.screenRT, copy);

                yield return null;

                timer += Time.unscaledDeltaTime;
            }

            RenderTexture.ReleaseTemporary(copy);
            Destroy(shaderInstance);
            this.screenFXQuad.SetActive(keepActive);
        }

        public IEnumerator SpiralTransition(float duration, bool keepActive = true, SpiralParameters ? parameters = null)
        {
            var copy = this.CopyScreen();
            this.SetupScreenQuad(this.screenRT);
            this.screenFXQuad.SetActive(true);

            var spiralParams = parameters ?? this.spiralDefaultParams;

            var shaderInstance = ComputeShader.Instantiate(this.battleTransitionComputeShaders.spiral);
            var shaderKernel = shaderInstance.FindKernel(ComputeShaderConstants.SPIRAL);

            var groups = ComputeShaderUtility.CalculateThreadGroups(shaderInstance, shaderKernel, this.screenRT.width, this.screenRT.height);


            shaderInstance.SetFloat(ComputeShaderConstants.ANGLE_SPEED.id, spiralParams.angleSpeed);
            shaderInstance.SetFloat(ComputeShaderConstants.CENTER_SPEED.id, spiralParams.centerSpeed);
            shaderInstance.SetFloat(ComputeShaderConstants.OFFSET.id, spiralParams.offset);

            float timer = 0.0f;
            while(timer < duration)
            {
                shaderInstance.SetFloat(ComputeShaderConstants.FADE.id, spiralParams.fade * Time.unscaledDeltaTime);
                shaderInstance.SetFloat(ComputeShaderConstants.TIME.id, timer);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.COPY.id, copy);
                shaderInstance.SetTexture(shaderKernel, ComputeShaderConstants.RESULT.id, this.screenRT);
                shaderInstance.Dispatch(shaderKernel, groups.x, groups.y, groups.z);

                Graphics.Blit(this.screenRT, copy);

                yield return null;

                timer += Time.unscaledDeltaTime;
            }

            RenderTexture.ReleaseTemporary(copy);
            Destroy(shaderInstance);
            this.screenFXQuad.SetActive(keepActive);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transition">Type of transition to play</param>
        /// <param name="duration">Number of seconds the transition should last</param>
        /// <param name="keepActive">If true, the quad the transition is drawn on remains active after the coroutine has finished</param>
        /// <returns></returns>
        public IEnumerator DoBattleTransition(BattleTransition transition, float duration, bool keepActive = true)
        {
            switch(transition)
            {
                case BattleTransition.HORIZONTAL:
                    yield return this.StartCoroutine(this.HorizontalTransition(duration, keepActive));
                    break;
                case BattleTransition.VERTICAL:
                    yield return this.StartCoroutine(this.VerticalTransition(duration, keepActive));
                    break;
                case BattleTransition.SPIRAL:
                    yield return this.StartCoroutine(this.SpiralTransition(duration, keepActive));
                    break;
                case BattleTransition.DIAMOND:
                    yield return this.StartCoroutine(this.DiamondTransition(duration, keepActive));
                    break;
            }
        }
    }

}