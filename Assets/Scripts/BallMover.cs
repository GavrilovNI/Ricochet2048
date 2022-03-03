using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BallMover : MonoBehaviour
{
    public event Action<Collider2D> HittedObject;

    [SerializeField, Min(0f)] private float _speed = 1;
    [SerializeField, Min(0f)] private float _skin = 0.05f;

    private CircleCollider2D _collider;

    private Vector3 Direction { get => transform.up; set => transform.up = value; }
    private float GlobalColliderRadius => _collider.radius * Mathf.Max(transform.localScale.x, transform.localScale.y);

    private void Awake()
    {
        _collider = GetComponent<CircleCollider2D>();
    }
    RaycastHit2D raycastHit;

    List<Vector3> hits = new List<Vector3>();

    private void Move(float distance)
    {
        if(distance < 0)
            throw new System.ArgumentOutOfRangeException(nameof(distance));

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = false;
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = ~(1 << gameObject.layer);

        RaycastHit2D[] results = new RaycastHit2D[1];
        int hitCount = Physics2D.CircleCast(transform.position, GlobalColliderRadius, Direction, contactFilter, results, distance);
        bool hitted = hitCount > 0;
        RaycastHit2D hit = hitted ? results[0] : new RaycastHit2D();
        //RaycastHit2D hit = Physics2D.CircleCast(transform.position, GlobalColliderRadius, Direction, distance, ~(1<<gameObject.layer));
        //bool hitted = hit.collider != null;

        Vector2 newPos = transform.position + Direction * distance;


        raycastHit = hit;

        float distanceCanMove;
        if(hitted)
        {
            distanceCanMove = hit.distance;
            float skinRollBack = Mathf.Cos(Vector3.Angle(-hit.normal, Direction) * Mathf.Deg2Rad) * _skin;
            if (distanceCanMove <= skinRollBack)
            {
                Debug.LogWarning($"Ball stacked in another object, set bigger value for '{nameof(_skin)}'");
                return;
            }
            else
            {
                distanceCanMove -= skinRollBack;
            }
        }
        else
        {
            distanceCanMove = distance;
        }

        //transform.position += Direction * distanceCanMove;
        transform.Translate(Direction * distanceCanMove, Space.World);

        hits.Add(transform.position);
        if (hits.Count > 10)
            hits.RemoveAt(0);

        if (hitted)
        {

            HittedObject?.Invoke(hit.collider);

            Vector2 newRotation = Vector2.Reflect(Direction, hit.normal);
            Direction = newRotation;

            if (distance > distanceCanMove)
                Move(distance - distanceCanMove);
        }
    }

    private void OnDrawGizmos()
    {
        bool colliderIsNull = _collider == null;
        if (colliderIsNull)
            _collider = GetComponent<CircleCollider2D>();
        //skin circle
        Handles.color = Color.blue;
        UnityEditor.Handles.DrawWireDisc(transform.position, -transform.forward, GlobalColliderRadius + _skin);

        Handles.color = Color.red;
        for (int i = 0; i < hits.Count; i++)
            UnityEditor.Handles.DrawWireDisc(hits[i], -transform.forward, GlobalColliderRadius + _skin);

        if (colliderIsNull)
            _collider = null;
    }

    private void FixedUpdate()
    {
        Move(Time.fixedDeltaTime * _speed);
    }
}
