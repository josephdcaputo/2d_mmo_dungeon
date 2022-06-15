﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private EnemyController controller;

    [Header("Set in inspector")]
    public LayerMask mask;
    public LayerMask obstacleMask;

    [Header("Debug Settings")]
    public Transform target;
    public Vector3 targetPrev;
    public BoxCollider2D collider;
    [SerializeField] private Vector3[] path;
    [SerializeField] private int targetIndex;

    void Awake()
    {
        controller = GetComponent<EnemyController>();
        collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyDetection();
    }

    void LateUpdate()
    {
        if (target)
        {
            targetPrev = target.position;
        }
    }

    public void RequestPath()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void MoveTowards()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, controller.enemyStats.moveSpeed * Time.deltaTime);
    }

    public void StopPathfinding()
    {
        StopCoroutine("FollowPath");
    }

    public bool targetBehindObstacle()
    {
        Vector3 colliderPos = transform.position + (Vector3)collider.offset;
        Vector3 direction = target.position - (transform.position + (Vector3)collider.offset);
        float distance = Vector3.Distance(colliderPos, target.position);
        if (Physics2D.BoxCast(colliderPos, collider.size, 0f, direction, distance, obstacleMask))
        {
            Debug.Log("Target behind obstacle");
            return true;
        }
        Debug.Log("Target not behind obstacle");
        return false;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, controller.enemyStats.moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    // Return true if target is within aggro range and set target. If multiple are in range, closest target is set
    public bool EnemyDetection()
    {
        Transform closest;
        Collider2D[] raycastHit = Physics2D.OverlapCircleAll((Vector2)transform.position, controller.enemyStats.aggroRange, mask); // May need to optimize with OverlapCircleNonAlloc

        if (raycastHit.Length > 0)
        {
            closest = raycastHit[0].transform;
            // Find the closest target if multiple
            for (int i = 1; i < raycastHit.Length; i++)
            {
                if (Vector3.Distance(transform.position, raycastHit[i].transform.position) < Vector3.Distance(transform.position, closest.position))
                {
                    closest = raycastHit[i].transform;
                }
            }
            target = closest;
            return true;
        }
        target = null;
        return false;
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
        Gizmos.color = target ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, controller.enemyStats.aggroRange);
    }
}
