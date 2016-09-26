using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D _boxCollider;
    private Rigidbody2D _rigidBody2d;
    private float _inverseMoveTime;

    // Use this for initialization
    protected virtual void Start() {
        _boxCollider = GetComponent<BoxCollider2D>();
        _rigidBody2d = GetComponent<Rigidbody2D>();
        _inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        _boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        _boxCollider.enabled = true;

        if(hit.transform == null) {
            StartCoroutine(SmoothMovement(end));
            return true;
        } else {
            return false;
        }


    }

    protected IEnumerator SmoothMovement(Vector3 end) {

        float sqrRemainingDist = (transform.position - end).sqrMagnitude;
        while(sqrRemainingDist > float.Epsilon) {
            Vector3 newPos = Vector3.MoveTowards(_rigidBody2d.position, end, _inverseMoveTime * Time.deltaTime);
            _rigidBody2d.MovePosition(newPos);

            sqrRemainingDist = (transform.position - end).sqrMagnitude;
            yield return null;
        }

    }


    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if(hit.transform == null) {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();

        if(!canMove && hitComponent != null) {
            OnCantMove(hitComponent);
        }

    }

    protected abstract void OnCantMove<T>(T component) 
        where T : Component;
    

	// Update is called once per frame
	void Update () {
	
	}
}
