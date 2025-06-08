using GimmeBattleTransitions.Runtime;
using System.Collections;
using UnityEngine;

namespace GimmeBattleTransitions.Samples
{
    public class BattleIslandTransition : MonoBehaviour
    {
        #region Public Fields

        public float transitionWaitTime = 10.0f;
        public float transitionTime = 2.0f;

        public BattleTransitions battleTransitions;

        #endregion

        #region Private Fields

        private bool transitioning = false;

        #endregion

        private IEnumerator WaitForTransition()
        {
            this.transitioning = true;

            yield return new WaitForSeconds(transitionWaitTime);
            yield return this.StartCoroutine(this.battleTransitions.DoBattleTransition(BattleTransition.DIAMOND, this.transitionTime));

            this.transitioning = false;

            var battleMgr = FindFirstObjectByType<BattleManager>();
            battleMgr.LoadBattleIslandScene();
        }

        private void Update()
        {
            if(!this.transitioning)
            {
                this.StartCoroutine(this.WaitForTransition());
            }
        }

    }
}
