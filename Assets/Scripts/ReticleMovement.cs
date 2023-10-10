using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleMovement : MonoBehaviour
{

    public GameObject player;
    public float reticleSpeed = 10f;

    private MeshRenderer mesh;
    private Camera mainCam;
    // Start is called before the first frame update
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        PlayerController.OnHoldingBallChanged += UpdateReticleState;
        mesh.enabled = false;
        mainCam = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        MoveReticle();
    }

    private void UpdateReticleState(bool isHoldingBall)
    {
        Debug.Log("is holding: " + isHoldingBall);
        mesh.enabled = isHoldingBall;
    }

    void MoveReticle()
    {
        //enable if using mouse
        /*  Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        float projection;
        Plane floor = new Plane(Vector3.up, Vector3.zero);

        if (floor.Raycast(ray, out projection))
        {
            Vector3 reticleDir = ray.GetPoint(projection);
            transform.position = reticleDir;
        }
        
        */

        // 2nd analogue stick
        float horizontal = Input.GetAxis("Horizontal2");
        float vertical = Input.GetAxis("Vertical2");

        Vector3 reticleDir = new Vector3();
        Vector3 direction = new Vector3(horizontal, 0f, (-1) * vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Convert the direction from local to world space based on camera orientation
            reticleDir = Quaternion.Euler(0, mainCam.transform.eulerAngles.y, 0) * direction;

        }
        else
        {
            Vector3 differenceVector = (-transform.position + player.transform.position).normalized;
            reticleDir.x = 2 * differenceVector.x;
            reticleDir.y = 0f;
            reticleDir.z = 2 * differenceVector.z;
        }

        reticleDir *= reticleSpeed * Time.fixedDeltaTime;
        transform.position += reticleDir;

    }
}