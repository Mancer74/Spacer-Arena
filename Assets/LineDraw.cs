using UnityEngine;
using System.Collections;

public class LineDraw : MonoBehaviour {

    private LineRenderer line;
    private Vector3 hitPos;
    private Vector3 bouncePos;
    Vector3[] positions = new Vector3[3];

    // Use this for initialization
    void Start () {
        line = GetComponent<LineRenderer>();
        line.SetWidth(.1f, .1f);
        line.SetColors(Color.green, Color.green);
        line.SetVertexCount(3);
	}

	// Update is called once per frame
	void Update () {
        positions[0] = transform.parent.position;
        positions[1] = hitPos;
        positions[2] = bouncePos;
        line.SetPositions(positions);

        Vector3 fwd = transform.forward;
        positions[0] = transform.parent.position + (positions[1]-positions[0])/6.0f;
        line.SetPositions(positions);
        RaycastHit hit;
        //Vector3 offset = new Vector3(0, 0, 0.5f);
        if (Physics.Raycast(transform.parent.transform.position, fwd, out hit, 1000))
        {
            hitPos = hit.point;

            Quaternion rot = Quaternion.AngleAxis(180, hit.normal);
            fwd = rot * fwd;
            fwd = -fwd;

            bouncePos = hit.point + fwd * 10;
            line.SetPosition(0, transform.parent.position);
            line.SetPosition(1, hit.point);
            //Debug.DrawLine(transform.position, hit.point);
        }
    }
}
