using GimmeBattleTransitions.Runtime;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace GimmeBattleTransitions.Samples
{
    public class OverworldCharacter : MonoBehaviour
    {

        #region Public Fields

        public float movementSpeed = 2.0f;
        public float waterLevel = 44.0f;

        public float encounterWalkTime = 5.0f;
        public float encounterVariation = 2.5f;

        public float transitionDuration = 2.0f;

        #endregion

        public Image blackScreen;

        #region Private Fields

        private Animator animator;

        private bool frozen = false;

        private CharacterController controller;

        private float nextEncounterTime = 0.0f;
        private float encounterTimer = 0.0f;

        private int transitionType = 1;

        private OverworldCamera cam;

        private Vector3 velocity;

        #endregion

        IEnumerator Start()
        {
            this.controller = this.gameObject.GetComponentInChildren<CharacterController>();
            this.cam = GameObject.FindAnyObjectByType<OverworldCamera>();

            this.nextEncounterTime = Random.Range(this.encounterWalkTime - this.encounterVariation, this.encounterWalkTime + this.encounterVariation);
            this.animator = this.GetComponentInChildren<Animator>();

            yield return null;

            var battleMgr = FindAnyObjectByType<BattleManager>();
            if(battleMgr.SceneWasChanged() && battleMgr.HasSavedPositions())
            {
                var savedPos = battleMgr.GetSavedCharacterPosition();
                this.transform.SetPositionAndRotation(savedPos.GetPosition(), savedPos.rotation);
            }
        }

        private IEnumerator DoScreenTransition()
        {

            var battleMgr = FindAnyObjectByType<BattleManager>();
            var battleTransitions = FindAnyObjectByType<BattleTransitions>();

            var transition = (BattleTransition)this.transitionType;
            if(battleMgr.SceneWasChanged())
            {
                transition = (BattleTransition)battleMgr.GetLastTransition();
                transition++;
            }
            if (transition == BattleTransition.NONE || transition > BattleTransition.SPIRAL) transition = BattleTransition.HORIZONTAL;

            yield return battleTransitions.DoBattleTransition(transition, this.transitionDuration);

            this.blackScreen.enabled = true;

            var overworldCam = FindAnyObjectByType<OverworldCamera>();
            battleMgr.LoadBattleScene(this.transform.localToWorldMatrix, overworldCam.transform.localToWorldMatrix, (int)transition);
        }

        void Update()
        {
            if (this.frozen) return;

            if(this.controller.isGrounded && this.velocity.y < 0.0f)
            {
                this.velocity.y = 0.0f;
            }

            float sideways = Input.GetAxis("Horizontal");
            float forward = Input.GetAxis("Vertical");

            var camRight = this.cam.transform.right;
            var camFwd = this.cam.transform.forward;

            var flatRight = new Vector3(camRight.x, 0.0f, camRight.z);
            var flatFwd = new Vector3(camFwd.x, 0.0f, camFwd.z);


            Vector3 movement = forward * flatFwd + flatRight * sideways;
            movement *= this.movementSpeed;

            var ray = new Ray()
            {
                direction = Vector3.down,
                origin = this.transform.position + movement
            };

            if(Physics.Raycast(ray, out var hitInfo, 50.0f)) {

                if (hitInfo.point.y <= this.waterLevel && hitInfo.distance < 1.2f)
                {
                    movement = Vector3.zero;
                }
            }

            this.controller.Move(movement * Time.deltaTime);

            this.velocity += Physics.gravity * Time.deltaTime;
            this.controller.Move(this.velocity * Time.deltaTime);

            var flatMovement = new Vector3(movement.x, 0.0f, movement.z);
            if (flatMovement.sqrMagnitude > 0.1f)
            {
                this.transform.forward = flatMovement.normalized;

                this.encounterTimer += Time.deltaTime;

                if (this.encounterTimer > this.nextEncounterTime)
                {
                    this.nextEncounterTime = Random.Range(this.encounterWalkTime - this.encounterVariation, this.encounterWalkTime + this.encounterVariation);
                    this.encounterTimer = 0.0f;

                    this.frozen = true;

                    this.StartCoroutine(this.DoScreenTransition());
                }
            }

            if(this.animator != null)
            {
                this.animator.SetBool("Run", flatMovement.sqrMagnitude > 0.1f);
            }


        }
    }
}
