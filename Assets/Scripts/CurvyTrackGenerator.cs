using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(EdgeCollider2D))]
[RequireComponent(typeof(LineRenderer))]
public class CurvyTrackGenerator : MonoBehaviour
{
    [Header("ROAD SETTINGS")]
    [SerializeField] private float roadWidth = 3.5f;
    [SerializeField] private float edgeLineWidth = 0.10f;

    [Header("ROAD COLORS")]
    [SerializeField] private Color roadColor =
        new Color(0.08f, 0.08f, 0.08f, 1f);

    [Header("TRACK SETTINGS")]
    [SerializeField] private float pointSpacing = 3f;

    [Header("TRACK POINTS")]
    [SerializeField] private Vector2[] trackPoints =
    {
        // =====================================================
        // START
        // =====================================================

        new Vector2(0f, 0f),
        new Vector2(3f, 0.05f),
        new Vector2(6f, 0.10f),
        new Vector2(9f, 0.05f),

        // =====================================================
        // SMALL CLIMB
        // =====================================================

        new Vector2(12f, 0.20f),
        new Vector2(15f, 0.55f),
        new Vector2(18f, 0.90f),
        new Vector2(21f, 1.05f),

        // =====================================================
        // DOWNHILL
        // =====================================================

        new Vector2(24f, 0.80f),
        new Vector2(27f, 0.45f),
        new Vector2(30f, 0.15f),
        new Vector2(33f, 0.05f),

        // =====================================================
        // ROLLING HILLS
        // =====================================================

        new Vector2(36f, 0.30f),
        new Vector2(39f, 0.75f),
        new Vector2(42f, 0.90f),
        new Vector2(45f, 0.55f),
        new Vector2(48f, 0.20f),
        new Vector2(51f, 0.05f),

        // =====================================================
        // BIG CLIMB 1
        // =====================================================

        new Vector2(54f, 0.25f),
        new Vector2(57f, 0.65f),
        new Vector2(60f, 1.10f),
        new Vector2(63f, 1.55f),
        new Vector2(66f, 1.80f),

        // =====================================================
        // BIG DOWNHILL 1
        // =====================================================

        new Vector2(69f, 1.55f),
        new Vector2(72f, 1.10f),
        new Vector2(75f, 0.60f),
        new Vector2(78f, 0.20f),
        new Vector2(81f, 0.05f),

        // =====================================================
        // VALLEY + HILL
        // =====================================================

        new Vector2(84f, 0.20f),
        new Vector2(87f, 0.60f),
        new Vector2(90f, 0.95f),
        new Vector2(93f, 0.70f),
        new Vector2(96f, 0.30f),
        new Vector2(99f, 0.10f),

        // =====================================================
        // BIG CLIMB 2
        // =====================================================

        new Vector2(102f, 0.35f),
        new Vector2(105f, 0.85f),
        new Vector2(108f, 1.35f),
        new Vector2(111f, 1.80f),
        new Vector2(114f, 2.05f),

        // =====================================================
        // LONG DOWNHILL
        // =====================================================

        new Vector2(117f, 1.75f),
        new Vector2(120f, 1.35f),
        new Vector2(123f, 0.90f),
        new Vector2(126f, 0.50f),
        new Vector2(129f, 0.25f),

        // =====================================================
        // FINAL CLIMB
        // =====================================================

        new Vector2(132f, 0.35f),
        new Vector2(135f, 0.75f),
        new Vector2(138f, 1.20f),
        new Vector2(141f, 1.65f),
        new Vector2(144f, 2.00f),

        // =====================================================
        // FINISH
        // Matches FinishLine X = 147.5, Y = 2.3
        // =====================================================

        new Vector2(147.5f, 2.30f)
    };

    private EdgeCollider2D edgeCollider;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        SetupComponents();
    }

    private void Start()
    {
        GenerateTrack();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        SetupComponents();

        if (!Application.isPlaying)
        {
            GenerateTrack();
        }
    }
#endif

    // =========================================================
    // COMPONENT SETUP
    // =========================================================

    private void SetupComponents()
    {
        if (edgeCollider == null)
        {
            edgeCollider = GetComponent<EdgeCollider2D>();
        }

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = false;

        lineRenderer.numCapVertices = 8;
        lineRenderer.numCornerVertices = 8;

        lineRenderer.alignment = LineAlignment.TransformZ;

        lineRenderer.textureMode =
            LineTextureMode.Stretch;

        lineRenderer.sortingLayerName = "Default";

        // Road behind car and pickups.
        lineRenderer.sortingOrder = -10;
    }

    // =========================================================
    // GENERATE TRACK
    // =========================================================

    private void GenerateTrack()
    {
        if (trackPoints == null ||
            trackPoints.Length < 2)
        {
            return;
        }

        SetupComponents();

        GenerateRoadVisual();
        GenerateRoadCollision();
    }

    // =========================================================
    // ROAD VISUAL
    // =========================================================

    private void GenerateRoadVisual()
    {
        lineRenderer.positionCount =
            trackPoints.Length;

        for (int i = 0; i < trackPoints.Length; i++)
        {
            lineRenderer.SetPosition(
                i,
                new Vector3(
                    trackPoints[i].x,
                    trackPoints[i].y,
                    0f
                )
            );
        }

        lineRenderer.startWidth = roadWidth;
        lineRenderer.endWidth = roadWidth;

        lineRenderer.startColor = roadColor;
        lineRenderer.endColor = roadColor;

        lineRenderer.loop = false;
    }

    // =========================================================
    // ROAD COLLISION
    // =========================================================

    private void GenerateRoadCollision()
    {
        Vector2[] collisionPoints =
            new Vector2[trackPoints.Length];

        float halfWidth =
            roadWidth * 0.5f;

        for (int i = 0;
             i < trackPoints.Length;
             i++)
        {
            Vector2 tangent;

            if (i == 0)
            {
                tangent =
                    trackPoints[1] -
                    trackPoints[0];
            }
            else if (i == trackPoints.Length - 1)
            {
                tangent =
                    trackPoints[i] -
                    trackPoints[i - 1];
            }
            else
            {
                tangent =
                    trackPoints[i + 1] -
                    trackPoints[i - 1];
            }

            tangent.Normalize();

            Vector2 normal =
                new Vector2(
                    -tangent.y,
                    tangent.x
                );

            // Keep collision surface on top
            // of the road.
            if (normal.y < 0f)
            {
                normal *= -1f;
            }

            collisionPoints[i] =
                trackPoints[i] +
                normal * halfWidth;
        }

        edgeCollider.points =
            collisionPoints;

        edgeCollider.edgeRadius = 0.08f;
    }

    // =========================================================
    // PUBLIC HELPERS
    // =========================================================

    public Vector2[] GetTrackPoints()
    {
        return trackPoints;
    }

    public Vector2 GetTrackStart()
    {
        return trackPoints[0];
    }

    public Vector2 GetTrackFinish()
    {
        return trackPoints[trackPoints.Length - 1];
    }

    public float GetRoadWidth()
    {
        return roadWidth;
    }
}