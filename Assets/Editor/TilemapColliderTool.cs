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
        //CreateEdgeColliders(oneWayRows, parent, scale);

        Vector2 positionOffset = new Vector2(0.25f, 0.0f);    // Y 오프셋 
        Vector2 sizeAdjustment = new Vector2(0.0f, -0.2f); // 크기 미세 조정
        CreateBoxCollidersForOneWayPlatforms(oneWayRows, parent, scale, positionOffset, sizeAdjustment);
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

    private void CreateBoxCollidersForOneWayPlatforms(Dictionary<int, List<Vector3Int>> tileRows, Transform parent, Vector3 scale, Vector2 positionOffset, Vector2 sizeAdjustment)
    {
        foreach (var row in tileRows)
        {
            List<Vector3Int> positions = row.Value;
            positions.Sort((a, b) => a.x.CompareTo(b.x));

            int groupStartX = positions[0].x;
            int currentY = positions[0].y;
            int lastX = groupStartX - 1;

            // 그룹으로 연속된 타일 묶기
            List<Vector3Int> group = new();

            void CreateBoxFromGroup(List<Vector3Int> groupPositions)
            {
                if (groupPositions.Count == 0) return;

                // 시작과 끝 x값 구하기
                int startX = groupPositions[0].x;
                int endX = groupPositions[^1].x;

                // 월드 위치 계산 (타일 중심 기준)
                Vector3 startWorld = parent.GetComponent<Tilemap>().CellToWorld(new Vector3Int(startX, currentY, 0));
                Vector3 endWorld = parent.GetComponent<Tilemap>().CellToWorld(new Vector3Int(endX, currentY, 0));

                // 박스 크기 계산 (가로: 끝 - 시작 + 1 tile 폭, 세로: tile 높이)
                float width = (endX - startX + 1) * scale.x;
                float height = scale.y;

                // GameObject 생성
                GameObject boxObj = new GameObject($"Collider_OneWay_{currentY}_{startX}_{endX}");
                boxObj.transform.parent = parent;

                // 타일맵 월드 좌표 기준, 가로 중앙에 위치시키고 Y는 타일 중앙 + offset 적용
                float posX = (startWorld.x + endWorld.x) / 2f + positionOffset.x;
                float posY = startWorld.y + 0.5f * scale.y + positionOffset.y; // 타일 중심 Y 기준 + offset
                boxObj.transform.position = new Vector3(posX, posY, 0);

                boxObj.tag = "OneWayPlatform";
                boxObj.layer = LayerMask.NameToLayer("Platform");

                BoxCollider2D box = boxObj.AddComponent<BoxCollider2D>();
                box.size = new Vector2(width + sizeAdjustment.x, height + sizeAdjustment.y);
                box.usedByEffector = true;

                PlatformEffector2D effector = boxObj.AddComponent<PlatformEffector2D>();
                effector.useOneWay = true;
                effector.surfaceArc = 160f;
                effector.useSideFriction = false;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                var pos = positions[i];
                if (pos.x != lastX + 1 || pos.y != currentY)
                {
                    CreateBoxFromGroup(group);
                    group.Clear();
                    currentY = pos.y;
                }

                group.Add(pos);
                lastX = pos.x;
            }

            CreateBoxFromGroup(group); // 마지막 그룹 처리
        }
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
