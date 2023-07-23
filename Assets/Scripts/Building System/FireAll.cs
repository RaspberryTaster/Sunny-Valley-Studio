using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Building_System
{
    public class FireAll : MonoBehaviour
    {
        public LayerMask IgnoreMe;
        private Ray ray;
        private RaycastHit hit;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                NeroFiresEverything();
        }

        public void NeroFiresEverything()
        {
            // Basic example ray
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 1000f, IgnoreMe))
            {
                Debug.Log("Get Rekt " + hit.collider.name);
            }
        }
    }
}