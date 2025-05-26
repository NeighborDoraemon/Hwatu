using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TilemapColliderTool : EditorWindow
{
    private Tilemap targetTilemap;

    [MenuItem("Tools/Generate Tilemap Colliders")]
    public static void ShowWindow()
    {
        GetWindow<TilemapColliderTool>("Tilemap Collider Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select Tilemap to Generate Colliders", EditorStyles.boldLabel);

        targetTilemap = (Tilemap)EditorGUILayout.ObjectField("Target Tilemap", targetTilemap, typeof(Tilemap), true);

        if (GUILayout.Button("Generate Colliders"))
        {
            if (targetTilemap != null)
            {
                GenerateColliders();
            }
            else
            {
                Debug.LogError("Tilemap이 선택되지 않았습니다.");
            }
        }
    }

    private void GenerateColliders()
    {
        Transform parent = targetTilemap.transform;
        Vector3 scale = parent.lossyScale;

        foreach (Transform child in parent)
        {
            if (child.name.StartsWith("Collider_"))
            {
                DestroyImmediate(child.gameObject);
            }
        }

        BoundsInt bounds = targetTilemap.cellBounds;
        TileBase[] allTiles = targetTilemap.GetTilesBlock(bounds);

        Dictionary<int, List<Vector3Int>> oneWayRows = new();
        Dictionary<int, List<Vector3Int>> normalRows = new();
        List<Vector3Int> wallTiles = new();

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int pos = new(x, y, 0);
                TileBase tile = targetTilemap.GetTile(pos);
                if (tile == null) continue;

                string tileName = tile.name;
                if (tileName.Contains("내부")) continue;

                if (tileName.Contains("가운데") || tileName.Contains("천장") || tileName.Contains("하단"))
                {
                    wallTiles.Add(pos);
                }
                else if (tileName.Contains("공중"))
                {
                    if (!oneWayRows.ContainsKey(y)) oneWayRows[y] = new();
                    oneWayRows[y].Add(pos);
                }
                else if (tileName.Contains("상단"))
                {
                    if (!normalRows.ContainsKey(y)) normalRows[y] = new();
                    normalRows[y].Add(pos);
                }
                else if (tileName.Contains("끝") || tileName.Contains("모서리"))
                {
                    if (!oneWayRows.ContainsKey(y)) oneWayRows[y] = new();
                    oneWayRows[y].Add(pos);
                }
            }
        }

        CreateBoxColliders(normalRows, parent, "Platform", "Platform", scale, false, true);
        CreateBoxColliders(GroupByX(wallTiles), parent, "Walls", "Walls", scale, true, true);
        CreateEdgeColliders(oneWayRows, parent, scale);
    }

    private void CreateBoxColliders(Dictionary<int, List<Vector3Int>> tileRows, Transform parent, string tag, string layer, Vector3 scale, bool vertical = false, bool useEffector = false)
    {
        foreach (var row in tileRows)
        {
            List<Vector3Int> positions = row.Value;
            positions.Sort((a, b) => vertical ? a.y.CompareTo(b.y) : a.x.CompareTo(b.x));

            List<Vector3Int> currentGroup = new();
            Vector3Int? last = null;

            foreach (var pos in positions)
            {
                if (last.HasValue && (vertical ? pos.y != last.Value.y + 1 || pos.x != last.Value.x : pos.x != last.Value.x + 1 || pos.y != last.Value.y))
                {
                    CreateBox(currentGroup, parent, tag, layer, scale, vertical, useEffector);
                    currentGroup.Clear();
                }
                currentGroup.Add(pos);
                last = pos;
            }
            if (currentGroup.Count > 0)
            {
                CreateBox(currentGroup, parent, tag, layer, scale, vertical, useEffector);
            }
        }
    }

    private void CreateBox(List<Vector3Int> positions, Transform parent, string tag, string layer, Vector3 scale, bool vertical, bool useEffector)
    {
        Tilemap tilemap = parent.GetComponent<Tilemap>();

        Vector3 min = tilemap.CellToWorld(positions[0]);
        Vector3 max = tilemap.CellToWorld(positions[positions.Count - 1]);

        float width = (vertical ? 1f : positions.Count) * scale.x;
        float height = (vertical ? positions.Count : 0.8f) * scale.y;

        Vector3 center = min + new Vector3(
            ((vertical ? 0 : positions.Count - 1) * 0.5f + 0.5f) * scale.x,
            ((vertical ? positions.Count - 1 : 0) * 0.5f + (vertical ? 0.5f : 0.4f)) * scale.y,
            0);

        GameObject colObj = new($"Collider_{tag}_{positions[0].x}_{positions[0].y}");
        colObj.transform.parent = parent;
        colObj.transform.position = center;
        colObj.tag = tag;
        colObj.layer = LayerMask.NameToLayer(layer);

        BoxCollider2D box = colObj.AddComponent<BoxCollider2D>();
        box.size = new Vector2(width, height);

        if (useEffector)
        {
            box.usedByEffector = true;
            PlatformEffector2D effector = colObj.AddComponent<PlatformEffector2D>();
            effector.surfaceArc = (tag == "Platform") ? 360f : 355f;
            effector.rotationalOffset = (tag == "Platform") ? 0f : 180f;
            effector.useSideFriction = false;
        }
    }

    private void CreateEdgeColliders(Dictionary<int, List<Vector3Int>> tileRows, Transform parent, Vector3 scale)
    {
        foreach (var row in tileRows)
        {
            List<Vector3Int> positions = row.Value;
            positions.Sort((a, b) => a.x.CompareTo(b.x));

            List<Vector3> points = new();
            int groupStartX = positions[0].x;
            int currentY = positions[0].y;
            int lastX = groupStartX - 1;

            for (int i = 0; i < positions.Count; i++)
            {
                var pos = positions[i];
                if (pos.x != lastX + 1 || pos.y != currentY)
                {
                    if (points.Count > 0)
                    {
                        CreateEdge(points, parent, currentY, scale);
                    }
                    points.Clear();
                }

                Vector3 world = parent.GetComponent<Tilemap>().CellToWorld(pos) + new Vector3(0.5f * scale.x, (0.5f + 0.3f) * scale.y, 0); // Y offset
                points.Add(world);
                lastX = pos.x;
            }

            if (points.Count > 0)
            {
                CreateEdge(points, parent, currentY, scale);
            }
        }
    }

    private void CreateEdge(List<Vector3> points, Transform parent, int yLevel, Vector3 scale)
    {
        if (points.Count < 2) return;

        points[0] += new Vector3(-0.5f * scale.x, 0, 0);
        points[^1] += new Vector3(0.5f * scale.x, 0, 0);

        GameObject edgeObj = new($"Collider_OneWay_{yLevel}");
        edgeObj.transform.parent = parent;
        edgeObj.tag = "OneWayPlatform";
        edgeObj.layer = LayerMask.NameToLayer("Platform");

        EdgeCollider2D edge = edgeObj.AddComponent<EdgeCollider2D>();
        Vector2[] edgePoints = points.ConvertAll(p => (Vector2)edgeObj.transform.InverseTransformPoint(p)).ToArray();
        edge.points = edgePoints;
        edge.usedByEffector = true;

        PlatformEffector2D effector = edgeObj.AddComponent<PlatformEffector2D>();
        effector.useOneWay = true;
        effector.surfaceArc = 160f;
        effector.useSideFriction = false;
    }

    private Dictionary<int, List<Vector3Int>> GroupByX(List<Vector3Int> positions)
    {
        Dictionary<int, List<Vector3Int>> result = new();
        foreach (var pos in positions)
        {
            if (!result.ContainsKey(pos.x)) result[pos.x] = new();
            result[pos.x].Add(pos);
        }
        return result;
    }
}
