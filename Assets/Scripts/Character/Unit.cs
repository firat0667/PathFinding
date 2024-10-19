using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform Target;
    public float Speed = 5;
    Vector3[] path;
    int TargetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, Target.position, OnpathFound);
    }

    public void OnpathFound(Vector3[] newPath,bool pathSuccesFul)
    {
        if (pathSuccesFul)
        {
            path = newPath;
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
                TargetIndex++;
                if (TargetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[TargetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for(int i= TargetIndex; i<path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == TargetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }

        }
    }

}
