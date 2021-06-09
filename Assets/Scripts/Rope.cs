using System.Collections.Generic;
using UnityEngine;

// Rope with segments.
public class Rope : MonoBehaviour
{
    [Tooltip("Template rope segment for creating new chains.")]
    public GameObject ropeSegmentPrefab;
    
    // List of rope segments.
    private List<GameObject> _ropeSegments = new List<GameObject>();
    
    // The rope getting longer or shorter?
    public bool IsIncreasing { get; set; }
    public bool IsDecreasing { get; set; }

    [Tooltip("The object where to connect the rope tail.")]
    public Rigidbody2D connectedObject;

    [Tooltip("The maximum length of each rope segment.")]
    public float maxRopeSegmentLength = 1.0f;

    [Tooltip("How fast a new segment should be created.")]
    public float ropeSpeed = 4.0f;

    // Renderer of the rope.
    private LineRenderer _lineRenderer;
    
    // Start is called before the first frame update.
    void Start()
    {
        // Caching link to the render to avoid searching during frame updates.
        _lineRenderer = GetComponent<LineRenderer>();
        
        ResetLength();
    }

    // Destroy all rope segments and creates a new one.
    public void ResetLength()
    {
        foreach (var segment in _ropeSegments)
        {
            Destroy(segment);
        }

        _ropeSegments = new List<GameObject>();

        IsDecreasing = false;
        IsIncreasing = false;

        CreateRopeSegment();
    }

    // Creates a new rope segment and append it to top.
    void CreateRopeSegment()
    {
        // Creates a new rope segment.
        var segment = Instantiate(ropeSegmentPrefab, transform.position, Quaternion.identity);
        
        // Makes the segment to ba a child of this and save global coordinates.
        segment.transform.SetParent(transform, true);
        
        // Gets a rigid body of the segment.
        var segmentBody = segment.GetComponent<Rigidbody2D>();
        
        // Gets the joints length.
        var segmentJoint = segment.GetComponent<SpringJoint2D>();
        
        // If the segment doesn't have rigid body or joint -> Error.
        if (segmentBody == null || segmentJoint == null)
        {
            Debug.LogError("Rope segment body prefab has no Rigidbody2D and/or SpringJoint2D!");
            return;
        }
        
        // Insert the segment at the begin of the list.
        _ropeSegments.Insert(0, segment);
        
        // If it "the first" rope segment -> connect with leg.
        if (_ropeSegments.Count == 1)
        {
            // Connects joints.
            var connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();
            connectedObjectJoint.connectedBody = segmentBody;
            connectedObjectJoint.distance = 0.1f;
            
            // Sets the segment's length to max.
            segmentJoint.distance = maxRopeSegmentLength;
        }
        else
        {
            // It's not the first segment. Connect with a previous rope segment.
            // Gets the previous segment.
            var prevSegment = _ropeSegments[1];
            
            // Gets joint for connection.
            var prevSegmentJoint = prevSegment.GetComponent<SpringJoint2D>();
            
            // Connect joints.
            prevSegmentJoint.connectedBody = segmentBody;
            
            // Sets the initial segment length - it grows automatically.
            segmentJoint.distance = 0.0f;
        }
        
        // Connect the segment with the rope's base.
        segmentJoint.connectedBody = GetComponent<Rigidbody2D>();
    }

    // Calls whenever it should make the rope shorter and delete the highest segment.
    void RemoveRopeSegment()
    {
        // If the count of rope segments is less than 2 -> return.
        if (_ropeSegments.Count < 2)
        {
            return;
        }
        
        // Gets the highest rope segment and the under that one.
        var topSegment = _ropeSegments[0];
        var nextSegment = _ropeSegments[1];
        
        // Connects the second segment with the rope's base.
        var nextSegmentJoint = nextSegment.GetComponent<SpringJoint2D>();
        nextSegmentJoint.connectedBody = GetComponent<Rigidbody2D>();
        
        // Removes and destroys the highest segment.
        _ropeSegments.RemoveAt(0);
        Destroy(topSegment);
    }

    // Update is called once per frame.
    // If it needed the rope getting longer or shorter every frame.
    void Update()
    {
        // Gets the highest rope segment and it joint.
        var topSegment = _ropeSegments[0];
        var topSegmentJoint = topSegment.GetComponent<SpringJoint2D>();

        if (IsIncreasing)
        {
            // The rope should get longer.
            // If the top segment length is max or bigger -> add a new segment.
            // Otherwise, increase the top segment's length.
            if (topSegmentJoint.distance >= maxRopeSegmentLength)
            {
                CreateRopeSegment();
            }
            else
            {
                topSegmentJoint.distance += ropeSpeed * Time.deltaTime;
            }
        }

        if (IsDecreasing)
        {
            // The rope should get shorter.
            // If the top segment length is close to 0 value -> remove the segment.
            // Otherwise, decrease the top segment's length.
            if (topSegmentJoint.distance <= 0.005f)
            {
                RemoveRopeSegment();
            }
            else
            {
                topSegmentJoint.distance -= ropeSpeed * Time.deltaTime;
            }
        }

        if (_lineRenderer != null)
        {
            // The renderer draws line by a collection of points.
            // These points should match the position of the rope segments.
            
            // The amount of vertex is equal to count of the segments plus one for the rope's base
            // and another one at leg.
            _lineRenderer.positionCount = _ropeSegments.Count + 2;
            
            // The highest vertex always equals to base's position.
            _lineRenderer.SetPosition(0, transform.position);
            
            // Applies the coordinate of the rope segments.
            for (var i = 0; i < _ropeSegments.Count; i++)
            {
                _lineRenderer.SetPosition(i+1, _ropeSegments[i].transform.position);
            }
            
            // The last point always at leg.
            var connectedObjectJoint = connectedObject.GetComponent<SpringJoint2D>();
            _lineRenderer.SetPosition(_ropeSegments.Count + 1,
                connectedObjectJoint.transform.TransformPoint(connectedObjectJoint.anchor));
        }
    }
}
