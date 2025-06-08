using System.Collections;
using UnityEngine;

namespace GimmeBattleTransitions.Samples
{
    public class BattleCharacter : MonoBehaviour
    {
        #region Public Fields

        public float waitTime = 6.0f;
        public float slashHeightTransitionTime = 3.0f;
        public float slashHeightWaitTime = 0.75f;
        public float slashTime = 0.5f;
        public float slashWaitTime = 2.0f;

        public Vector3 slashHeightPosition;
        public Vector3 slashPosition;


        #endregion

        #region Private Fields

        private Animator animator;

        private bool slashing = false;

        #endregion

        void Start()
        {
            this.animator = this.GetComponentInChildren<Animator>();
        }

        private IEnumerator SlashRoutine()
        {

            this.slashing = true;
            this.animator.SetBool("Fighting", true);

            yield return new WaitForSeconds(this.waitTime);

            this.animator.SetBool("SlashStart", true);

            var startPos = this.transform.position;
            float time = 0.0f;
            while(time < this.slashHeightTransitionTime)
            {
                float percent = time / this.slashHeightTransitionTime;
                float targetY = this.slashHeightPosition.y;
                float currentY = Mathf.Sqrt(percent) * targetY;

                Vector3 linearPos = Vector3.Lerp(startPos, this.slashHeightPosition, percent);
                linearPos.y = currentY;

                this.transform.position = linearPos;

                yield return null;

                time += Time.deltaTime;
            }

            this.transform.position = this.slashHeightPosition;
            
            yield return new WaitForSeconds(this.slashHeightWaitTime);

            this.animator.SetBool("SlashEnd", true);

            startPos = this.slashHeightPosition;
            time = 0.0f;
            while(time < this.slashTime)
            {
                float percent = time / this.slashTime;

                this.transform.position = Vector3.Lerp(this.slashHeightPosition, this.slashPosition, percent);

                yield return null;

                time += Time.deltaTime;
            }

            this.transform.position = this.slashPosition;

            yield return new WaitForSeconds(this.slashWaitTime);
        }

        void Update()
        {
            if(!this.slashing)
            {
                this.StartCoroutine(this.SlashRoutine());
            }

        }
    }
}
