using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{

    public enum DiceColor
    {
        BLUE = 0,
        ORANGE = 1,
    }

    #region Public Variables

    public float rollForce = 0.0f;
    public float rollTorque = 10.0f;
    public float diceWaitTime = 1.0f;
    public float rollAngleDeviation = 10.0f;

    public Transform startPosition;

    public Vector3 rollDirection;
    

    #endregion

    #region Private Variables

    private bool isRolling = false;
    private bool didCollide = false;

    private DiceColor currentDiceColor = DiceColor.BLUE;

    private Rigidbody rb = null;

    #endregion

    public bool RollIsFinished() => !this.isRolling;

    public DiceColor GetDiceColor() => this.currentDiceColor;

    private void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
        this.rb.maxAngularVelocity = 100.0f;
    }

    public IEnumerator Roll()
    {
        this.gameObject.SetActive(true);

        this.isRolling = true;
        this.didCollide = false;

        this.rb.MovePosition(this.startPosition.position);
        this.rb.MoveRotation(this.startPosition.rotation);
        this.rb.velocity = Vector3.zero;
        this.rb.angularVelocity = Vector3.zero;
        this.rb.isKinematic = true;

        yield return null;

        this.transform.position = this.startPosition.position;
        this.transform.rotation = this.startPosition.rotation;

        yield return new WaitForSeconds(this.diceWaitTime);

        this.rb.isKinematic = false;

        float deviation = UnityEngine.Random.Range(-this.rollAngleDeviation, this.rollAngleDeviation);
        var direction = Quaternion.AngleAxis(deviation, Vector3.up) * this.rollDirection;

        this.rb.AddForce(direction * this.rollForce, ForceMode.VelocityChange);
        this.rb.AddTorque(Random.onUnitSphere * this.rollTorque, ForceMode.VelocityChange);
    }

    public IEnumerator Vanish()
    {
        float timer = 0.0f;
        while (timer < 2.0f)
        {
            this.rb.AddTorque(Vector3.back, ForceMode.VelocityChange);

            yield return null;

            timer += Time.deltaTime;
        }

        this.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        this.didCollide = true;
    }

    private void CalcualteDiceColorFromCurrentFacing()
    {
        float upDot = Vector3.Dot(this.transform.up, Vector3.up);
        float rightDot = Vector3.Dot(this.transform.right, Vector3.up);
        float forwardDot = Vector3.Dot(this.transform.forward, Vector3.up);

        if (Mathf.Abs(upDot) > Mathf.Abs(rightDot) && Mathf.Abs(upDot) > Mathf.Abs(forwardDot))
        {
            if (upDot > 0.0f)
            {
                this.currentDiceColor = DiceColor.ORANGE;
            }
            else
            {
                this.currentDiceColor = DiceColor.BLUE;
            }
        }
        else if (Mathf.Abs(rightDot) > Mathf.Abs(forwardDot))
        {

            if (rightDot > 0.0f)
            {
                this.currentDiceColor = DiceColor.BLUE;
            }
            else
            {
                this.currentDiceColor = DiceColor.ORANGE;
            }

        }
        else
        {
            if (forwardDot > 0.0f)
            {
                this.currentDiceColor = DiceColor.BLUE;
            }
            else
            {
                this.currentDiceColor = DiceColor.ORANGE;
            }
        }
    }

    void Update()
    {
        if(this.isRolling && this.didCollide)
        {
            if(this.rb.IsSleeping())
            {
                this.isRolling = false;

                this.CalcualteDiceColorFromCurrentFacing();
            }
        }
    }
}
