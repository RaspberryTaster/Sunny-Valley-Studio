using NaughtyAttributes;
using UnityEngine;

public class AngleCalculator : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform c;

    void Start()
    {
        //SetMat();
    }

    [Button]
    private void NewMethod()
    {
        // Assuming the coordinates of a1, p, b1, and c1 are defined
        Vector3 a1 = a.position;
        Vector3 p = transform.position;
        Vector3 b1 = b.position;
        Vector3 c1 = c.position;

        // Calculate the angles between p and a1, p and b1, p and c1
        float angleX1 = Mathf.Atan2(a1.z - p.z, a1.x - p.x) * Mathf.Rad2Deg;
        float angleX2 = Mathf.Atan2(b1.z - p.z, b1.x - p.x) * Mathf.Rad2Deg;
        float angleX3 = Mathf.Atan2(c1.z - p.z, c1.x - p.x) * Mathf.Rad2Deg;

        Debug.Log("Position of a1: " + a1);
        Debug.Log("Position of b1: " + b1);
        Debug.Log("Position of c1: " + c1);


        Debug.Log("Angle between p and a1: " + angleX1);
        Debug.Log("Angle between p and b1: " + angleX2);
        Debug.Log("Angle between p and c1: " + angleX3);
    }
}
