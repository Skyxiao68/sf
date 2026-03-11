using UnityEngine;
using UnityEngine.InputSystem;

public class camera : MonoBehaviour
{
    private Player_Input inputControl; 
    public float rotateSpeed = 100f; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputControl = new Player_Input();   
        Vector2 inputLook = inputControl.Player.Look.ReadValue<Vector2>(); 
        float mouseX = inputLook.x * rotateSpeed * Time.deltaTime;
        float mouseY = inputLook.y * rotateSpeed * Time.deltaTime;
        transform.Rotate(0, mouseX, 0); 
        transform.Rotate(0, mouseY, 0); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
