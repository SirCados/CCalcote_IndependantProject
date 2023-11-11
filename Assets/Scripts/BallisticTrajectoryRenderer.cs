using System.Collections.Generic;
using UnityEngine;

//From this article: https://www.patrykgalach.com/2020/03/23/drawing-ballistic-trajectory-in-unity/

/// <summary>
/// Ballistic trajectory renderer.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class BallisticTrajectoryRenderer : MonoBehaviour
{
    public bool IsBlasting = false;

    private LineRenderer _line;
    // Initial trajectory position
    [SerializeField] Vector3 _startPosition;
    Vector3 _targetPosition;
    // Initial trajectory velocity
    [SerializeField] Vector3 _startVelocity;
    // Step distance for the trajectory
    [SerializeField] float _trajectoryVertDist = 0.25f;
    [Header("Debug")]
    // Flag for always drawing trajectory
    [SerializeField] bool _debugAlwaysDrawTrajectory = false;

    /// <summary>
    /// Method called on initialization.
    /// </summary>
    private void Awake()
    {
        // Get line renderer reference
        _line = GetComponent<LineRenderer>();
    }
    /// <summary>
    /// Method called on every frame.
    /// </summary>
    private void Update()
    {
        // Draw trajectory while pressing button
        if (IsBlasting)
        {
            // Draw trajectory
            DrawTrajectory();
        }
        // Clear trajectory after releasing button
        if (!IsBlasting)        
        {
            // Clear trajectory
            ClearTrajectory();
        }
    }
    /// <summary>
    /// Sets ballistic values for trajectory.
    /// </summary>
    /// <param name="startPosition">Start position.</param>
    /// <param name="startVelocity">Start velocity.</param>
    public void SetBallisticValues(Vector3 startPosition, Vector3 startVelocity)
    {
        _startPosition = startPosition;
        _startVelocity = startVelocity;
    }

    /// <summary>
    /// Draws the trajectory with line renderer.
    /// </summary>
    private void DrawTrajectory()
    {
        // Create a list of trajectory points
        List<Vector3> curvePoints = new List<Vector3>();
        curvePoints.Add(_startPosition);

        // Initial values for trajectory
        Vector3 currentPosition = _startPosition;
        Vector3 currentVelocity = _startVelocity;

        // Init physics variables
        RaycastHit hit;
        Ray ray = new Ray(currentPosition, currentVelocity.normalized);

        // Loop until hit something or distance is too great
        while (!Physics.Raycast(ray, out hit, _trajectoryVertDist))
        {
            // Time to travel distance of trajectoryVertDist
            float trajectory = _trajectoryVertDist / currentVelocity.magnitude;

            // Update position and velocity
            currentVelocity = currentVelocity + trajectory * Physics.gravity;
            currentPosition = currentPosition + trajectory * currentVelocity;

            // Add point to the trajectory
            curvePoints.Add(currentPosition);

            // Create new ray
            ray = new Ray(currentPosition, currentVelocity.normalized);
        }

        // If something was hit, add last point there
        if (hit.transform)
        {
            print("hit! " + hit.transform.position);
            curvePoints.Add(hit.point);
        }

        // Display line with all points
        _line.positionCount = curvePoints.Count;
        _line.SetPositions(curvePoints.ToArray());
    }

    void MyDrawTrajectory()
    {
        float distance = Vector3.Distance(_startPosition, _targetPosition);
        Vector3 currentPosition = _startPosition;        
        //_targetPosition;
        
    }

    /// <summary>
    /// Clears the trajectory.
    /// </summary>
    private void ClearTrajectory()
    {
        // Hide line
        _line.positionCount = 0;
    }
}