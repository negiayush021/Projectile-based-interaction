using System.Collections;
using TMPro;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI score_txt;
    int score = 0;

    [SerializeField] private GameObject AlertIMG;

    private Camera cam;
    private Vector2 camOriginalPos;
    private Rigidbody2D rb;

    private Animator text_animator;



    private void Start()
    {
        cam = Camera.main;
        camOriginalPos = (Vector2)cam.transform.position;
        rb = GetComponent<Rigidbody2D>();

        text_animator = score_txt.GetComponent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Pipe")
        {
            score++;
            score_txt.text = score.ToString();
            GameManager.instance.ResetBallPos();

            text_animator.Play("Bloom");
        }

        if(collision.tag == "OutsideCollider")
        {
            StartCoroutine(CameraShakeEffect());
            score = 0;
            score_txt.text = score.ToString();
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    IEnumerator CameraShakeEffect()
    {
        AlertIMG.SetActive(true);

        float duration = 0.2f;
        float elapsed = 0;


        while(elapsed < duration)
        {
            float speed = 30f;
            float amount = 0.2f;

            float pos = Mathf.Sin(Time.time * speed) * amount;
            cam.transform.position = new Vector3(pos, 0 , -10);

            elapsed += Time.deltaTime;

            yield return null;
        }
        cam.transform.position = new Vector3(camOriginalPos.x, camOriginalPos.y, -10);

        AlertIMG.SetActive(false);
        GameManager.instance.ShowRestartBtn();
    }
}
