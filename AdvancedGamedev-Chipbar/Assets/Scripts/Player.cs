using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Public Variables

    public float maxHealth = 100.0f;
    public float movementSpeed = 3.0f;

    public CharacterController charController;

    public Chipbar chipBar;

    public Rect bounds = new Rect();

    #endregion

    #region Private Variables

    private float currentHealth;

    #endregion

    public void AddHealth(float amount)
    {
        float amountPercent = amount / this.maxHealth;

        this.currentHealth += amount;
        this.currentHealth = Mathf.Clamp(this.currentHealth, 0.0f, this.maxHealth);

        this.chipBar.Add(amountPercent);
    }

    public void RemoveHealth(float amount)
    {
        float amountPercent = amount / this.maxHealth;

        this.currentHealth -= amount;
        this.currentHealth = Mathf.Clamp(this.currentHealth, 0.0f, this.maxHealth);

        this.chipBar.Remove(amountPercent);
    }
         
    private void Start()
    {
        this.currentHealth = this.maxHealth;

        float percent = this.currentHealth / this.maxHealth;
        this.chipBar.SetValue(percent);
    }

    private void ClampPosition()
    {
        var pos = this.transform.position;
        var min = this.bounds.min;
        var max = this.bounds.max;

        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.z = Mathf.Clamp(pos.z, min.y, max.y);

        this.transform.position = pos;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * this.movementSpeed;
        float vertical = Input.GetAxis("Vertical") * this.movementSpeed;

        Vector3 movement = new Vector3(horizontal, Physics.gravity.y, vertical) * Time.deltaTime;

        this.charController.Move(movement);

        this.ClampPosition();
    }
}
