using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GimmeBattleTransitions.Samples
{
    public class BattleCamera : MonoBehaviour
    {
        #region Public Fields

        public float circleRadius = 10.0f;
        public float circleTime = 1.5f;
        public float circleAngle = 30.0f;
        public float circleCameraHeight = 2.0f;
        public float fadeBlackTime = 0.5f;

        public Vector3 lineStart;
        public Vector3 lineEnd;
        public float lineTime = 1.5f;

        public Vector3 staticPosition;
        public Vector3 staticRotation;

        public Image fadeBlack;

        #endregion

        #region Private Fields

        private bool flightStarted = false;
        private bool fadeStarted = false;

        #endregion


        private IEnumerator Fade()
        {
            this.fadeStarted = true;

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

        private IEnumerator BattleFlight()
        {
            this.flightStarted = true;


            float time = 0.0f;

            float startAngle = Random.Range(0.0f, 360.0f);
            float endAngle = (startAngle + this.circleAngle) % 360.0f;

            while(time < this.circleTime)
            {

                float percent = time / this.circleTime;
                float angle = Mathf.LerpAngle(startAngle, endAngle, percent);

                float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
                float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

                Vector3 dir = new Vector3(cos, 0.0f, sin);

                this.transform.forward = dir.normalized;
                this.transform.transform.position = dir * this.circleRadius + Vector3.up * this.circleCameraHeight;

                yield return null;

                time += Time.deltaTime;
            }

            this.transform.rotation = Quaternion.LookRotation(new Vector3(0.5f, -0.5f, 0.0f), Vector3.right);
            time = 0.0f;
            while(time < this.lineTime)
            {
                float percent = time / this.lineTime;
                this.transform.position = Vector3.Lerp(this.lineStart, this.lineEnd, percent);

                yield return null;

                time += Time.deltaTime;
            }

            this.transform.position = this.staticPosition;
            this.transform.eulerAngles = this.staticRotation;
        }


        void Update()
        {
            if(!this.flightStarted)
            {
                this.StartCoroutine(this.BattleFlight());
            }

            if(!this.fadeStarted)
            {
                this.StartCoroutine(this.Fade());
            }
        }
    }
}
