using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GimmeBattleTransitions.Samples
{
    public class OverworldCamera : MonoBehaviour
    {

        #region Public Fields

        public float rotateLerpSpeed = 0.1f;
        public float moveLerpSpeed = 0.1f;
        public float rotateSpeed = 0.1f;
        public float fadeBlackTime = 1.0f;

        public Image fadeBlack;

        public OverworldCharacter character;

        #endregion

        #region Private Fields

        private float currentRotation = 0.0f;

        private Vector3 offset;

        #endregion

        private IEnumerator Fade()
        {
            this.fadeBlack.enabled = true;

            float time = 0.0f;
            while(time < this.fadeBlackTime)
            {
                float alpha = 1.0f - time / this.fadeBlackTime;
                var color = this.fadeBlack.color;
                color.a = alpha;
                this.fadeBlack.color = color;

                yield return null;
                time += Time.deltaTime;
            }

            var lastColor = this.fadeBlack.color;
            lastColor.a = 0.0f;
            this.fadeBlack.color = lastColor;
        }

        private IEnumerator Start()
        {
            this.offset = this.character.transform.position - this.transform.position;

            var battleMgr = FindAnyObjectByType<BattleManager>();
            if (battleMgr.SceneWasChanged()) this.fadeBlack.enabled = true;

            yield return null;

            if (battleMgr.SceneWasChanged())
            {
                if (battleMgr.HasSavedPositions())
                {
                    var savedPos = battleMgr.GetSavedCameraPosition();
                    var savedCharacterPos = battleMgr.GetSavedCharacterPosition();

                    this.offset = savedCharacterPos.GetPosition() - savedPos.GetPosition();

                    this.transform.SetPositionAndRotation(savedPos.GetPosition(), savedPos.rotation);
                }
                this.StartCoroutine(this.Fade());
            }
            else {
                this.fadeBlack.enabled = false;
            }
        }

        public void Update()
        {
            var forward = this.character.transform.position - this.transform.position;
            var targetRotation = Quaternion.LookRotation(forward, Vector3.up);

            float horizontal = Input.GetAxis("Horizontal");
            this.currentRotation += this.rotateSpeed * horizontal * Time.deltaTime;
            var charRotation = Quaternion.AngleAxis(this.currentRotation, Vector3.up);
            var rotatedVec = charRotation * this.offset;

            var targetPos = this.character.transform.position - rotatedVec;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, this.rotateLerpSpeed * Time.deltaTime);
            this.transform.position = Vector3.Lerp(this.transform.position, targetPos, this.moveLerpSpeed * Time.deltaTime);
        }

    }
}
