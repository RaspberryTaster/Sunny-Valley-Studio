using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Flags]
public enum PanelFloorOwnership
{
    A = 1,
    B = 2,
    C = 4,
    D = 8,
    ALL = A | B | C | D
}

namespace Assets.Scripts.Building_System
{
    public class Panel : MonoBehaviour
    {
        Renderer rend;

        public PanelFloorOwnership A;
        public Material tempMaterial;
        private void Awake()
        {
            rend = GetComponent<Renderer>();
        }

        [Button]
        public void SetTest()
        {
            SetMaterials(A,tempMaterial);
        }
        public void SetMaterials(PanelFloorOwnership ownership, Material mat)
        {
            if ((ownership & PanelFloorOwnership.A) != 0)
            {
                SetMatA(mat);
            }

            if ((ownership & PanelFloorOwnership.B) != 0)
            {
                SetMatB(mat);
            }

            if ((ownership & PanelFloorOwnership.C) != 0)
            {
                SetMatC(mat);
            }

            if ((ownership & PanelFloorOwnership.D) != 0)
            {
                SetMatD(mat);
            }
        }

        private void SetMatC(Material mat)
        {
            var mats = new Material[] { mat, rend.materials[1], rend.materials[2], rend.materials[3] };
            rend.materials = mats;
        }

        private void SetMatA(Material mat)
        {
            var mats = new Material[] { rend.materials[0], mat, rend.materials[2], rend.materials[3] };
            rend.materials = mats;
        }

        private void SetMatD(Material mat)
        {
            var mats = new Material[] { rend.materials[0], rend.materials[1], mat, rend.materials[3] };
            rend.materials = mats;
        }

        private void SetMatB(Material mat)
        {
            var mats = new Material[] { rend.materials[0], rend.materials[1], rend.materials[2], mat };
            rend.materials = mats;
        }
    }
}
