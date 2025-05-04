using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Vector2 moveDirection;

    private void Awake()
    {
        moveDirection = Vector2.up;
        StartCoroutine(ScaleUp());
    }

    private void OnEnable()
    {
        GameplayManager.Instance.GameEnd += GameEnded;
    }

    private void OnDisable()
    {
        GameplayManager.Instance.GameEnd -= GameEnded;
    }

    [SerializeField] private GameObject clickParticle, scoreParticle, playerParticle;
    [SerializeField] private AudioClip moveClip, _scoreSoundClip;

    private void Update()
    {
        Vector3? inputPosition = null;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
        }
#elif UNITY_ANDROID || UNITY_IOS
    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
    {
        inputPosition = Input.GetTouch(0).position;
    }
#endif

        if (inputPosition.HasValue)
        {
            SoundManager.Instance.PlaySound(moveClip);

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(inputPosition.Value);
            Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

            moveDirection = (worldPos2D - (Vector2)transform.position).normalized;

            Destroy(Instantiate(clickParticle, new Vector3(worldPos.x, worldPos.y, 0f), Quaternion.identity), 1f);
        }

        float cosAngle = Mathf.Acos(moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, cosAngle * (moveDirection.y < 0f ? -1f : 1f));
    }


    private void FixedUpdate()
    {
        transform.position += (Vector3)(moveSpeed * Time.fixedDeltaTime * moveDirection);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Side"))
        {
            moveDirection.x *= -1f;
        }

        if (collision.CompareTag("Top"))
        {
            moveDirection.y *= -1f;
        }

        if(collision.CompareTag("Score"))
        {

            SoundManager.Instance.PlaySound(_scoreSoundClip);
            Destroy(Instantiate(scoreParticle, collision.gameObject.transform.position, Quaternion.identity), 1f);
            GameplayManager.Instance.UpdateScore();
            StartCoroutine(ScoreDestroy(collision.gameObject));
        }
    }

    [SerializeField] private AnimationClip _scoreDestroyClip;

    private IEnumerator ScoreDestroy(GameObject scoreObject)
    {
        scoreObject.GetComponent<Collider2D>().enabled = false;
        scoreObject.GetComponent<Animator>().Play(_scoreDestroyClip.name, -1, 0f);
        yield return new WaitForSeconds(_scoreDestroyClip.length);
        Destroy(scoreObject);
    }

    [SerializeField] private float _animationTime;
    [SerializeField] private AnimationCurve _scaleUpCurve, _scaleDownCurve;

    private IEnumerator ScaleUp()
    {
        float timeElapsed = 0f;
        float speed = 1 / _animationTime;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        Vector3 scaleOffset = endScale - startScale;

        while(timeElapsed < 1f)
        {
            timeElapsed += speed * Time.deltaTime;
            transform.localScale = startScale + _scaleUpCurve.Evaluate(timeElapsed) * scaleOffset;
            yield return null;
        }

        transform.localScale = Vector3.one;
    }

    private void GameEnded()
    {
        StartCoroutine(ScaleDown());
    }

    private IEnumerator ScaleDown()
    {
        Destroy(Instantiate(playerParticle, transform.position, Quaternion.identity), 1f);

        float timeElapsed = 0f;
        float speed = 1 / _animationTime;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;
        Vector3 scaleOffset = endScale - startScale;

        while (timeElapsed < 1f)
        {
            timeElapsed += speed * Time.deltaTime;
            transform.localScale = startScale + _scaleUpCurve.Evaluate(timeElapsed) * scaleOffset;
            yield return null;
        }

        Destroy(gameObject);
    }
}
