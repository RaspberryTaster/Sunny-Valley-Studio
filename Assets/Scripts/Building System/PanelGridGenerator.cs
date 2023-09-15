using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;
using Assets.Scripts.Building_System;

public class PanelGridGenerator : MonoBehaviour
{
    [SerializeField] private Panel panelPrefab;
    [SerializeField]private Vector2Int _gridSize = new Vector2Int(5, 5);
    public float _panelSpacing = 1.0f;
    public Vector3 WorldBottomLeft => CalculateWorldBottomLeft();


    private Panel[,] _panelGrid; // 2D array to store panels.
    private List<Panel> _panelList = new List<Panel>(); // List to store panels.

        private Transform _panelParent; // Parent for the panels.
    public Transform PanelParent
    {
        get
        {
            // If _panelParent is null, try to find it among the children
            if (_panelParent == null)
            {
                _panelParent = transform.Find("Panel Parent");
            }
            return _panelParent;
        }
        set
        {
            _panelParent = value;
        }
    }

    private Vector3 CalculateWorldBottomLeft()
    {
        Vector3 centerOffset = new Vector3(
            (_gridSize.x - 1) * _panelSpacing * 0.5f,
            0,
            (_gridSize.y - 1) * _panelSpacing * 0.5f
        );

        return transform.position - centerOffset;
    }

    [Button("Generate Panel Grid")]
    public void GeneratePanelGrid()
    {
        GeneratePanelGrid(_gridSize);
    }

    public void GeneratePanelGrid(Vector2Int size)
    {
        _gridSize = size;
        ClearExistingPanels();

        // Create a parent for the panels under this GameObject
        if (PanelParent == null)
        {
            PanelParent = new GameObject("Panel Parent").transform;
            PanelParent.parent = transform;
        }

        _panelGrid = new Panel[_gridSize.x, _gridSize.y]; // Initialize the 2D array.

        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                Vector3 panelPosition = WorldBottomLeft + new Vector3(x * _panelSpacing, 0, y * _panelSpacing);
                Panel panel = Instantiate(panelPrefab, panelPosition, Quaternion.identity);
                panel.transform.parent = PanelParent; // Parent the panel to the _panelParent.

                // Store the panel in the 2D array.
                _panelGrid[x, y] = panel;

                // Add the panel to the list.
                _panelList.Add(panel);

                // Set the panel's name based on grid coordinates.
                panel.name = "Panel [" + x + ", " + y + "]";

                // Optionally, you can add custom logic here to set materials or properties for each panel.
            }
        }
    }

    public Panel PanelFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 gridSize= new Vector2(_gridSize.x , _gridSize.y);   
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.z + gridSize.y / 2) / gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSize.x - 1) * percentX);
        int y = Mathf.RoundToInt((gridSize.y - 1) * percentY);
        return _panelGrid[x, y];
    }


    [Button("Clear Existing Panels")]
    public void ClearExistingPanels()
    {
        if (PanelParent != null)
        {
            int childCount = PanelParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                DestroyImmediate(PanelParent.GetChild(0).gameObject);
            }

            PanelParent = null;
            _panelList.Clear(); // Clear the panel list.
        }
    }
}
