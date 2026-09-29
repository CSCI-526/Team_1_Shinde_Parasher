using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CurvyTrackGenerator : MonoBehaviour
{
    [Header("ROAD SETTINGS")]
    [SerializeField] private float roadWidth = 1.6f;
    [SerializeField] private float edgeLineWidth = 0.10f;

    [Header("ROAD COLORS")]
    [SerializeField] private Color roadColor = new Color(0.12f, 0.13f, 0.15f, 1f);
    [SerializeField] private Color edgeColor = Color.white;

    [Header("TRACK POINTS")]
    [SerializeField] private float pointSpacing = 2.5f;

    private EdgeCollider2D edgeCollider;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private LineRenderer leftEdge;
    private LineRenderer rightEdge;

    /*
     * MAIN TRACK
     *
     * This is deliberately smoother and more user-friendly
     * than the previous version.
     */
    private readonly Vector2[] trackPoints =
    {
        // ------------------------------------------------
        // START
        // ------------------------------------------------

        new Vector2(0f, 0.0f),
        new Vector2(2.5f, 0.0f),
        new Vector2(5f, 0.1f),
        new Vector2(7.5f, 0.3f),

        // ------------------------------------------------
        // FIRST HILL
        // ------------------------------------------------

        new Vector2(10f, 0.8f),
        new Vector2(12.5f, 1.5f),
        new Vector2(15f, 2.1f),
        new Vector2(17.5f, 2.3f),
        new Vector2(20f, 1.9f),
        new Vector2(22.5f, 1.2f),

        // ------------------------------------------------
        // VALLEY
        // ------------------------------------------------

        new Vector2(25f, 0.5f),
        new Vector2(27.5f, 0.2f),
        new Vector2(30f, 0.3f),

        // ------------------------------------------------
        // SECOND HILL
        // ------------------------------------------------

        new Vector2(32.5f, 0.8f),
        new Vector2(35f, 1.6f),
        new Vector2(37.5f, 2.5f),
        new Vector2(40f, 3.0f),
        new Vector2(42.5f, 2.8f),
        new Vector2(45f, 2.1f),
        new Vector2(47.5f, 1.2f),

        // ------------------------------------------------
        // DEEP VALLEY
        // ------------------------------------------------

        new Vector2(50f, 0.5f),
        new Vector2(52.5f, 0.2f),
        new Vector2(55f, 0.4f),

        // ------------------------------------------------
        // BIG HILL
        // ------------------------------------------------

        new Vector2(57.5f, 1.2f),
        new Vector2(60f, 2.2f),
        new Vector2(62.5f, 3.2f),
        new Vector2(65f, 4.0f),
        new Vector2(67.5f, 4.3f),
        new Vector2(70f, 4.0f),
        new Vector2(72.5f, 3.2f),
        new Vector2(75f, 2.1f),

        // ------------------------------------------------
        // ROLLER SECTION
        // ------------------------------------------------

        new Vector2(77.5f, 1.0f),
        new Vector2(80f, 1.4f),
        new Vector2(82.5f, 2.4f),
        new Vector2(85f, 3.0f),
        new Vector2(87.5f, 2.4f),
        new Vector2(90f, 1.4f),

        // ------------------------------------------------
        // FINAL CLIMB
        // ------------------------------------------------

        new Vector2(92.5f, 1.0f),
        new Vector2(95f, 1.7f),
        new Vector2(97.5f, 2.7f),
        new Vector2(100f, 3.8f),
        new Vector2(102.5f, 4.7f),
        new Vector2(105f, 5.0f),

        // ------------------------------------------------
        // FINAL DOWNHILL
        // ------------------------------------------------

        new Vector2(107.5f, 4.5f),
        new Vector2(110f, 3.6f),
        new Vector2(112.5f, 2.4f),
        new Vector2(115f, 1.5f),

        // ------------------------------------------------
        // FINISH
        // ------------------------------------------------

        new Vector2(117.5f, 1.0f),
        new Vector2(120f, 1.0f)
    };

    private void Awake()
    {
        GenerateTrack();
    }

    private void GenerateTrack()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        GenerateRoadMesh();
        GenerateCollision();
        GenerateRoadEdges();

        Debug.Log(
            "Weight Rush track generated with " +
            trackPoints.Length +
            " points."
        );
    }

    // ====================================================
    // COLLISION
    // ====================================================

    private void GenerateCollision()
    {
        Vector2[] collisionPoints =
            new Vector2[trackPoints.Length];

        for (int i = 0; i < trackPoints.Length; i++)
        {
            Vector2 normal = GetUpwardNormal(i);

            /*
             * IMPORTANT:
             *
             * The collider is placed at the TOP of the road.
             *
             * This prevents the car from being half inside
             * the visible road.
             */
            collisionPoints[i] =
                trackPoints[i] +
                normal * (roadWidth * 0.5f);
        }

        edgeCollider.points = collisionPoints;
    }

    // ====================================================
    // ROAD MESH
    // ====================================================

    private void GenerateRoadMesh()
    {
        Mesh mesh = new Mesh();

        mesh.name = "Weight Rush Road";

        Vector3[] vertices =
            new Vector3[trackPoints.Length * 2];

        Vector2[] uv =
            new Vector2[trackPoints.Length * 2];

        int[] triangles =
            new int[(trackPoints.Length - 1) * 6];

        for (int i = 0; i < trackPoints.Length; i++)
        {
            Vector2 normal =
                GetUpwardNormal(i);

            Vector2 top =
                trackPoints[i] +
                normal * (roadWidth * 0.5f);

            Vector2 bottom =
                trackPoints[i] -
                normal * (roadWidth * 0.5f);

            vertices[i * 2] =
                new Vector3(
                    top.x,
                    top.y,
                    0f
                );

            vertices[i * 2 + 1] =
                new Vector3(
                    bottom.x,
                    bottom.y,
                    0f
                );

            float u =
                i / (float)(trackPoints.Length - 1);

            uv[i * 2] =
                new Vector2(u, 1f);

            uv[i * 2 + 1] =
                new Vector2(u, 0f);
        }

        int triangleIndex = 0;

        for (int i = 0; i < trackPoints.Length - 1; i++)
        {
            int topLeft = i * 2;
            int bottomLeft = i * 2 + 1;

            int topRight = (i + 1) * 2;
            int bottomRight = (i + 1) * 2 + 1;

            triangles[triangleIndex++] = topLeft;
            triangles[triangleIndex++] = bottomLeft;
            triangles[triangleIndex++] = topRight;

            triangles[triangleIndex++] = topRight;
            triangles[triangleIndex++] = bottomLeft;
            triangles[triangleIndex++] = bottomRight;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        CreateRoadMaterial();
    }

    // ====================================================
    // ROAD MATERIAL
    // ====================================================

    private void CreateRoadMaterial()
    {
        Shader shader =
            Shader.Find(
                "Universal Render Pipeline/Unlit"
            );

        if (shader == null)
        {
            shader =
                Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            Debug.LogWarning(
                "Could not find a suitable road shader."
            );

            return;
        }

        Material material =
            new Material(shader);

        material.name =
            "Weight Rush Road Material";

        material.color =
            roadColor;

        meshRenderer.material =
            material;

        meshRenderer.sortingOrder = 5;
    }

    // ====================================================
    // ROAD EDGE LINES
    // ====================================================

    private void GenerateRoadEdges()
    {
        leftEdge =
            CreateEdgeLine(
                "Left Road Edge"
            );

        rightEdge =
            CreateEdgeLine(
                "Right Road Edge"
            );

        Vector3[] leftPoints =
            new Vector3[trackPoints.Length];

        Vector3[] rightPoints =
            new Vector3[trackPoints.Length];

        for (int i = 0; i < trackPoints.Length; i++)
        {
            Vector2 normal =
                GetUpwardNormal(i);

            Vector2 top =
                trackPoints[i] +
                normal * (roadWidth * 0.5f);

            /*
             * Slightly above the road so the
             * white line is clearly visible.
             */
            Vector2 left =
                top +
                normal * 0.03f;

            Vector2 right =
                top +
                normal * 0.03f;

            leftPoints[i] =
                new Vector3(
                    left.x,
                    left.y,
                    -0.05f
                );

            rightPoints[i] =
                new Vector3(
                    right.x,
                    right.y,
                    -0.05f
                );
        }

        /*
         * Both lines use the same road top edge.
         *
         * The visual road itself provides the
         * main road surface.
         */
        leftEdge.positionCount =
            trackPoints.Length;

        rightEdge.positionCount =
            trackPoints.Length;

        leftEdge.SetPositions(
            leftPoints
        );

        rightEdge.SetPositions(
            rightPoints
        );
    }

    // ====================================================
    // CREATE LINE
    // ====================================================

    private LineRenderer CreateEdgeLine(
        string objectName
    )
    {
        GameObject lineObject =
            new GameObject(objectName);

        lineObject.transform.SetParent(
            transform
        );

        lineObject.transform.localPosition =
            Vector3.zero;

        LineRenderer line =
            lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = false;

        line.startWidth =
            edgeLineWidth;

        line.endWidth =
            edgeLineWidth;

        line.numCornerVertices = 8;
        line.numCapVertices = 8;

        line.startColor =
            edgeColor;

        line.endColor =
            edgeColor;

        line.sortingOrder = 10;

        Shader shader =
            Shader.Find("Sprites/Default");

        if (shader != null)
        {
            Material material =
                new Material(shader);

            material.color =
                edgeColor;

            line.material =
                material;
        }

        return line;
    }

    // ====================================================
    // UPWARD NORMAL
    // ====================================================

    private Vector2 GetUpwardNormal(int index)
    {
        Vector2 tangent;

        if (index == 0)
        {
            tangent =
                trackPoints[1] -
                trackPoints[0];
        }
        else if (index ==
                 trackPoints.Length - 1)
        {
            tangent =
                trackPoints[index] -
                trackPoints[index - 1];
        }
        else
        {
            tangent =
                trackPoints[index + 1] -
                trackPoints[index - 1];
        }

        tangent.Normalize();

        /*
         * Perpendicular vector.
         */
        Vector2 normal =
            new Vector2(
                -tangent.y,
                tangent.x
            );

        /*
         * Always choose the upward-facing normal.
         */
        if (normal.y < 0f)
        {
            normal = -normal;
        }

        return normal.normalized;
    }
}