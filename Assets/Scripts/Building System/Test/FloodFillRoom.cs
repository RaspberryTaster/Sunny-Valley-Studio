using Assets.Scripts.Building_System;
using Assets.Scripts.Building_System.Test;
using Assets.Scripts.Extensions;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public struct PosDir
{
    public Vector3 pos;
    public Vector3 dir;
    public PosDir(Vector3 pos, Vector3 dir)
    {
        this.pos = pos;
        this.dir = dir;
    }
}

public class PositionOwnershipPair
{
    public Vector3 Position { get; set; }
    public PanelFloorOwnership Ownership { get; set; }

    public PositionOwnershipPair(Vector3 position, PanelFloorOwnership ownership)
    {
        Position = position;
        Ownership = ownership;
    }
}

public class FloodFillRoom : Singleton<FloodFillRoom>
{
    public Tilemap _wallPlacementGrid;
    public Tilemap _groundGrid;
    public Renderer _boundsPlane;
    public Panel _floodPrefab;
    public Node _nodePrefab;
    private Camera _mainCamera;
    public List<Vector3> _wallPositions;
    private Dictionary<Vector3, List<Wall>> _placedWall = new Dictionary<Vector3, List<Wall>>();
    private Dictionary<Vector3, Node> _nodes = new Dictionary<Vector3, Node>();

    //public Dictionary<Vector3, Panel> _allPanels = new();
    public Material _tempFloodMat;

    public PanelGridGenerator _panelGrid;

    private void Awake()
    {
        _panelGrid = GetComponent<PanelGridGenerator>();
    }
    private void Start()
    {
        _mainCamera = Camera.main;
        _panelGrid.GeneratePanelGrid();
    }
    Panel testPanel = null;
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 placementPosition = PlacementUtils.WorldPositionToGridPosition(hit.point, _groundGrid);


                Debug.Log("Flood");
                // Check if placement position is within bounds
                if (IsWithinBoundsXZ(placementPosition))
                {
                    BeginRoomGeneration(placementPosition);
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //Vector3 placementPosition = Position(hit.point);
                testPanel = _panelGrid.PanelFromWorldPoint(hit.point);
                Debug.Log(testPanel.name);
            }

        }

    }


    [Button]
    public void SetPanelGrid()
    {
        Bounds bounds = _boundsPlane.bounds;

        _panelGrid.GeneratePanelGrid(new Vector2Int((int)bounds.size.x , (int)bounds.size.z));
    }

    public Node GetNodeAtPos(Vector3 position)
    {
        //if

        var p = PlacementUtils.WorldPositionToGridPosition(position, _groundGrid);
        if (_nodes.ContainsKey(p))
        {
            return _nodes[p];
        }
        else
        {

            var n = Instantiate(_nodePrefab);
            n.SetPos(p);
            _nodes.Add(p, n);
            return n;
        }

    }
    public void AddWall(Wall w)
    {
        if (w == null) return;
        Vector3 key = Position(w.transform.position);
        Vector3 key1 = Position(w.endPoint.position);
        _wallPositions.Add(key);
        _wallPositions.Add(key1);

        // Add the wall to the _placedWall dictionary
        if (!_placedWall.ContainsKey(key))
        {
            _placedWall[key] = new List<Wall>();
        }
        _placedWall[key].Add(w);

        if (!_placedWall.ContainsKey(key1))
        {
            _placedWall[key1] = new List<Wall>();
        }
        _placedWall[key1].Add(w);
    }

    public void RemoveWall(Wall w)
    {
        if (w == null) return;
        Vector3 key = Position(w.transform.position);
        Vector3 key1 = Position(w.endPoint.position);
        _wallPositions.Remove(key);
        _wallPositions.Remove(key1);

        // Remove the wall from the _placedWall dictionary
        if (_placedWall.ContainsKey(key))
        {
            _placedWall[key].Remove(w);
        }

        if (_placedWall.ContainsKey(key1))
        {
            _placedWall[key1].Remove(w);
        }
    }

    public Vector3 Position(Vector3 position)
    {
        Vector3Int gridPosition = _wallPlacementGrid.WorldToCell(position);
        return _wallPlacementGrid.GetCellCenterWorld(gridPosition);
    }

    private bool IsWithinBoundsXZ(Vector3 position)
    {
        Bounds bounds = _boundsPlane.bounds;

        bool withinXBounds = position.x >= bounds.min.x && position.x <= bounds.max.x;
        bool withinZBounds = position.z >= bounds.min.z && position.z <= bounds.max.z;

        return withinXBounds && withinZBounds;
    }

    public void BeginRoomGeneration(Vector3 position)
    {

        //if (!IsWithinBoundsXZ(position)) return;
        GenerateRoom(FloodFill(position));
    }


    private void GenerateRoom(List<Vector3> filledPositions)
    {

        if (filledPositions.Count == 0)
        {
            Debug.LogWarning("No filled positions available.");
        }
        else
        {
            List<Vector3> nodePositions = new List<Vector3>();
            List<Wall> encounteredWalls = new List<Wall>();


            foreach (var position in filledPositions)
            {
                Node node = GetNodeAtPos(position);
                if (node != null)
                {
                    nodePositions.Add(position);
                }

                if (_placedWall.ContainsKey(position))
                {
                    foreach (var wall in _placedWall[position])
                    {
                        if (!encounteredWalls.Contains(wall))
                        {
                            encounteredWalls.Add(wall);
                        }
                    }
                }
            }

            _mat = RandomMaterialColor();
            List<PositionOwnershipPair> positionOwnershipPairs = _positionsToColor.Select(position => new PositionOwnershipPair(position, PanelFloorOwnership.ALL)).ToList().Concat(_fragmentedPositionsToColor.ToList()).ToList();
            Debug.Log(_fragmentedPositionsToColor.Count);
            //ChangeMat(_positionsToColor, _mat);
            //ChangeMat(_fragmentedPositionsToColor, _mat);


            Room newRoom = new Room(nodePositions, encounteredWalls, positionOwnershipPairs);
            newRoom.SetRoomFloorMat(_mat);
            // Now you have the room data in the newRoom object, you can do further processing or use it as needed.
        }

    }

    Material _mat;
    Queue<Vector3> _positionsToColor = new Queue<Vector3>();
    Queue<PositionOwnershipPair> _fragmentedPositionsToColor = new Queue<PositionOwnershipPair>();
    private List<Vector3> FloodFill(Vector3 initialPosition)
    {
        HashSet<Vector3> visitedPositions = new HashSet<Vector3>();
        Queue<PosDir> positionsToCheck = new Queue<PosDir>();
        List<Vector3> filledPositions = new List<Vector3>(); // Store filled positions

        _positionsToColor.Clear();
        _fragmentedPositionsToColor.Clear();
        //_mat = RandomMaterialColor();
        positionsToCheck.Enqueue(new PosDir(initialPosition, Vector3.zero));
        while (positionsToCheck.Count > 0) // Continue until all positions are processed
        {

            PosDir p = positionsToCheck.Dequeue();
            Vector3 position = p.pos;
            Node curNode = GetNodeAtPos(position);
            var wallDirections = curNode.wallDirections;
            if (!IsWithinBoundsXZ(position))
            {
                Debug.LogWarning("Lead to outdoors.");
                return new List<Vector3>();
                //continue;
            }

            if (visitedPositions.Contains(position))
            {
                continue;
            }

            visitedPositions.Add(position);

            Panel samplePanel = _panelGrid.PanelFromWorldPoint(position);
            if (filledPositions.Contains(position))
            {
                _positionsToColor.Enqueue(position);
                filledPositions.Add(position);
                //SetMat(position, curNode, _mat);
                //continue;
            }
            else if (samplePanel != null)
            {
                _positionsToColor.Enqueue(position);
                filledPositions.Add(position);
            }
            else if (_placedWall.ContainsKey(position))
            {
                _positionsToColor.Enqueue(position);
                filledPositions.Add(position);
                //SetMat(position, curNode,_mat);
                // Handle walls 
                continue;
            }
            /*
            else
            {
                //var x = Instantiate(_floodPrefab, PlacementUtils.WorldPositionToGridPosition(position+new Vector3(0,0000001f,0), _groundGrid), Quaternion.identity);
                //_allPanels.Add(position, x);
                filledPositions.Add(position); // Add filled position to the list
                _positionsToColor.Enqueue(position);
                //SetMat(position, curNode,_mat);
            }
            */


            if ((curNode.wallDirections & WallDirection.Diagonal_Alpha) != 0)
            {
                //bool approachingTop = p.dir == Vector3.left || p.dir == Vector3.forward;
                //thjat means you only option is to go 
                //that means you cant go any further down or right
                //can only go the opposite  of for or left

                if (p.dir == Vector3.forward)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    //Means A and D on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right));
                }
                else if (p.dir == Vector3.left)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    //Means A and D on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back));
                }
                else if (p.dir == Vector3.right)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position , PanelFloorOwnership.C));
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means B and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward));
                }
                else
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.C));
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means B and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left));
                }
            }
            else if ((curNode.wallDirections & WallDirection.Diagonal_Beta) != 0)
            {
                if (p.dir == Vector3.forward)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    //Means A and B on panel belong to the room
                    //D And C now must become empty
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left));
                }
                else if (p.dir == Vector3.right)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    //Means A and B on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back));
                }
                else if (p.dir == Vector3.left)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.C));
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means D and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward));
                }
                else
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.C));
                   // _allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    //_allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means D and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right));
                }
            }
            else
            {
                if ((wallDirections & WallDirection.North) == 0 && p.dir != Vector3.forward)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back)); // Avoid going back
                }

                if ((wallDirections & WallDirection.South) == 0 && p.dir != Vector3.back)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward)); // Avoid going forward
                }

                if ((wallDirections & WallDirection.West) == 0 && p.dir != Vector3.left)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right)); // Avoid going right
                }

                if ((wallDirections & WallDirection.East) == 0 && p.dir != Vector3.right)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left)); // Avoid going left
                }


            }

            //if wall directions include Wall.diagonal_alpha 
        }

        return filledPositions; // Return the list of filled positions
    }
    /*
     private List<Vector3> FloodFill(Vector3 initialPosition)
    {
        HashSet<Vector3> visitedPositions = new HashSet<Vector3>();
        Queue<PosDir> positionsToCheck = new Queue<PosDir>();
        List<Vector3> filledPositions = new List<Vector3>(); // Store filled positions

        _positionsToColor.Clear();
        _fragmentedPositionsToColor.Clear();
        _mat = RandomMaterialColor();
        positionsToCheck.Enqueue(new PosDir(initialPosition, Vector3.zero));
        while (positionsToCheck.Count > 0) // Continue until all positions are processed
        {

            PosDir p = positionsToCheck.Dequeue();
            Vector3 position = p.pos;
            Node curNode = Instance.GetNodeAtPos(position);
            var wallDirections = curNode.wallDirections;
            if (!IsWithinBoundsXZ(position))
            {
                Debug.LogWarning("Lead to outdoors.");
                return new List<Vector3>();
                //continue;
            }

            if (visitedPositions.Contains(position))
            {
                continue;
            }

            visitedPositions.Add(position);

            if (filledPositions.Contains(position) || _allPanels.Keys.Contains(position))
            {
                _positionsToColor.Enqueue(position);
                SetMat(position, curNode, _mat);
                //continue;
            }
            else if (_placedWall.Keys.Contains(position))
            {
                _positionsToColor.Enqueue(position);
                SetMat(position, curNode,_mat);
                // Handle walls 
                continue;
            }
            else
            {
                var x = Instantiate(_floodPrefab, PlacementUtils.WorldPositionToGridPosition(position+new Vector3(0,0000001f,0), _groundGrid), Quaternion.identity);
                _allPanels.Add(position, x);
                filledPositions.Add(position); // Add filled position to the list
                _positionsToColor.Enqueue(position);
                SetMat(position, curNode,_mat);
            }



            if ((curNode.wallDirections & WallDirection.Diagonal_Alpha) != 0)
            {
                //bool approachingTop = p.dir == Vector3.left || p.dir == Vector3.forward;
                //thjat means you only option is to go 
                //that means you cant go any further down or right
                //can only go the opposite  of for or left

                if (p.dir == Vector3.forward)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    //Means A and D on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right));
                }
                else if (p.dir == Vector3.left)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    //Means A and D on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back));
                }
                else if (p.dir == Vector3.right)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position , PanelFloorOwnership.C));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means B and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward));
                }
                else
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.C));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means B and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left));
                }
            }
            else if ((curNode.wallDirections & WallDirection.Diagonal_Beta) != 0)
            {
                if (p.dir == Vector3.forward)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    //Means A and B on panel belong to the room
                    //D And C now must become empty
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left));
                }
                else if (p.dir == Vector3.right)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.A));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.B));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.A, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.B, _mat);
                    //Means A and B on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back));
                }
                else if (p.dir == Vector3.left)
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.C));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means D and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward));
                }
                else
                {
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.D));
                    _fragmentedPositionsToColor.Enqueue(new PositionOwnershipPair(position, PanelFloorOwnership.C));
                    _allPanels[position].SetMaterials(PanelFloorOwnership.D, _mat);
                    _allPanels[position].SetMaterials(PanelFloorOwnership.C, _mat);
                    //Means D and C on panel belong to the room
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right));
                }
            }
            else
            {
                if ((wallDirections & WallDirection.North) == 0 && p.dir != Vector3.forward)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.forward, Vector3.back)); // Avoid going back
                }

                if ((wallDirections & WallDirection.South) == 0 && p.dir != Vector3.back)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.back, Vector3.forward)); // Avoid going forward
                }

                if ((wallDirections & WallDirection.West) == 0 && p.dir != Vector3.left)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.left, Vector3.right)); // Avoid going right
                }

                if ((wallDirections & WallDirection.East) == 0 && p.dir != Vector3.right)
                {
                    positionsToCheck.Enqueue(new PosDir(position + Vector3.right, Vector3.left)); // Avoid going left
                }


            }

            //if wall directions include Wall.diagonal_alpha 
        }

        return filledPositions; // Return the list of filled positions
    }
     */
    public void ChangeMat(Queue<Vector3> positionsToColor, Material mat)
    {
        Debug.Log("Applying Color");
        while (positionsToColor.Count > 0)
        {
            Vector3 position = positionsToColor.Dequeue();
            Node curNode = Instance.GetNodeAtPos(position);

            // Apply materials to the positions in the queue
            SetMat(position, curNode, mat);

            // Handle walls or other logic specific to coloring
            // ...

            // Add the logic for enqueuing adjacent positions to _positionsToColor if needed
            // ...
        }
    }
    public void ChangeMat(Queue<PositionOwnershipPair> positionsToColor, Material mat)
    {
        Debug.Log("Applying Color");
        while (positionsToColor.Count > 0)
        {
            PositionOwnershipPair pair = positionsToColor.Dequeue();
            Vector3 position = pair.Position;
            PanelFloorOwnership ownership = pair.Ownership;

            //Node curNode = Instance.GetNodeAtPos(position);

            // Apply materials using the ownership information
            // For example:

            Panel samplePanel = _panelGrid.PanelFromWorldPoint(position);
            samplePanel.SetMaterials(ownership, mat);
            //_allPanels[position].SetMaterials(ownership, mat);

            // Handle other logic as needed...
        }
    }


    public void ChangeMat(List<PositionOwnershipPair> p, Material mat)
    {
        Queue<PositionOwnershipPair> positionsToColor = new Queue<PositionOwnershipPair>(p);
        Debug.Log("Applying Color");
        while (positionsToColor.Count > 0)
        {
            PositionOwnershipPair pair = positionsToColor.Dequeue();
            Vector3 position = pair.Position;
            PanelFloorOwnership ownership = pair.Ownership;

            //Node curNode = Instance.GetNodeAtPos(position);

            // Apply materials using the ownership information
            // For example:
            Panel samplePanel = _panelGrid.PanelFromWorldPoint(position);
            samplePanel.SetMaterials(ownership, mat);
            //_allPanels[position].SetMaterials(ownership, mat);

            // Handle other logic as needed...
        }
    }

    private void SetMat(Vector3 position, Node curNode,Material mat)
    {
        if ((curNode.wallDirections & WallDirection.Diagonal_Alpha) == 0 && (curNode.wallDirections & WallDirection.Diagonal_Beta) == 0)
        {
            Panel samplePanel = _panelGrid.PanelFromWorldPoint(position);
            samplePanel.SetMaterials(PanelFloorOwnership.ALL, mat);
        }
    }

    public Material RandomMaterialColor()
    {
        Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        return CreateTempMaterial(randomColor);
    }
    private Material CreateTempMaterial(Color color)
    {
        Material newMaterial = new Material(_tempFloodMat);
        newMaterial.color = color;
        return newMaterial;
    }

    private void OnDrawGizmos()
    {
        if(testPanel == null) { return; }
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(testPanel.transform.position, 1);
    }
}
