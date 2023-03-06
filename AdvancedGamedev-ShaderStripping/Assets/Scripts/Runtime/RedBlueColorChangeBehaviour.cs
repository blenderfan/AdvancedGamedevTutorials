using System.Collections;
using UnityEngine;

namespace ShaderStripping
{
    public class RedBlueColorChangeBehaviour : MonoBehaviour
    {

        #region Public Variables

        public Material redBlueMaterial;

        public Vector2 timeInterval;

        #endregion

        #region Private Variables

        private bool colorChangeCoroutineInProgress = false;

        private static readonly string BLUE_VARIANT = "BLUE";
        private static readonly string RED_VARIANT = "RED";

        #endregion

        private void ActivateRedColor()
        {
            this.redBlueMaterial.DisableKeyword(BLUE_VARIANT);
            this.redBlueMaterial.EnableKeyword(RED_VARIANT);
        }

        private void ActivateBlueColor()
        {
            this.redBlueMaterial.EnableKeyword(BLUE_VARIANT);
            this.redBlueMaterial.DisableKeyword(RED_VARIANT);
        }


        private IEnumerator ChangeColor(float waitTime)
        {
            this.colorChangeCoroutineInProgress = true;

            yield return new WaitForSeconds(waitTime);

            if(this.redBlueMaterial.IsKeywordEnabled(BLUE_VARIANT))
            {
                this.ActivateRedColor();
            } else
            {
                this.ActivateBlueColor();
            }

            this.colorChangeCoroutineInProgress = false;
        }

        private void Update()
        {
            if(!this.colorChangeCoroutineInProgress)
            {
                float time = UnityEngine.Random.Range(this.timeInterval.x, this.timeInterval.y);

                this.StartCoroutine(this.ChangeColor(time));
            }
        }

        private void OnDisable()
        {
            this.ActivateRedColor();
        }
    }
}
