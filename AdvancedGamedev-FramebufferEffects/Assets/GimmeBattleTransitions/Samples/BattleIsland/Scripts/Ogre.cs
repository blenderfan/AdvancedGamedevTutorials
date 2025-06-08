using System.Collections;
using UnityEngine;

namespace GimmeBattleTransitions.Samples
{
    public class Ogre : MonoBehaviour
    {
        #region Public Fields

        public float waitTimeUntilDeath = 10.0f;
        public float fallTime = 2.0f;
        public float fadeTime = 1.5f;

        #endregion

        #region Private Fields

        private Animator animator = null;

        private bool dying = false;

        private MaterialPropertyBlock mpb;

        private SkinnedMeshRenderer mr;

        #endregion

        void Start()
        {
            this.animator = this.GetComponentInChildren<Animator>();
            this.mpb = new MaterialPropertyBlock();

            this.mr = this.GetComponentInChildren<SkinnedMeshRenderer>();
            this.mr.SetPropertyBlock(this.mpb);
        }

        private IEnumerator Die()
        {
            this.dying = true;

            yield return new WaitForSeconds(this.waitTimeUntilDeath);

            this.animator.SetBool("Die", true);

            yield return new WaitForSeconds(this.fallTime);

            float time = 0.0f;
            float smoothness = this.mr.sharedMaterial.GetFloat("_Smoothness");
            this.mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            while(time < this.fadeTime)
            {
                float percent = time / this.fadeTime;
                float alpha = 1.0f - percent;
                this.mpb.SetFloat("_Alpha", alpha);
                this.mpb.SetFloat("_Smoothness", smoothness * (1.0f - percent));
                this.mr.SetPropertyBlock(this.mpb);

                yield return null;

                time += Time.deltaTime;
            }
        }

        void Update()
        {
            if(!this.dying)
            {
                this.StartCoroutine(this.Die());
            }
        }
    }
}
