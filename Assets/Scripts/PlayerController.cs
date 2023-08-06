using UnityEngine;

namespace ClearSky
{
    public class PlayerController : MonoBehaviour
    {
        public float movePower = 10f;
        public float jumpPower = 15f; //Set Gravity Scale in Rigidbody2D Component to 5
        public GameObject wizard;
		public bool controlOn = true;

		private Rigidbody2D rb;
        private Animator anim;
        private bool isJumping = false;
        

        private float horizontalMove;
        private Vector3 moveVelocity;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = wizard.GetComponent<Animator>();
        }

		private void Update()
		{
            if (controlOn) 
            {
				horizontalMove = Input.GetAxisRaw("Horizontal");

				if ((Input.GetButtonDown("Jump") || Input.GetAxis("Vertical") > 0)
				&& !anim.GetBool("isJump"))
				{
					isJumping = true;
					anim.SetBool("isJump", true);
				}
            }
            else
            {
                horizontalMove = 0;
				isJumping = false;
				anim.SetBool("isJump", false);
			}
			
		}

		private void FixedUpdate()
        {
			moveVelocity = Vector3.zero;
			anim.SetBool("isRun", false);
			if (horizontalMove != 0)
			{
				wizard.transform.localScale = new Vector3(horizontalMove, 1, 1);
				moveVelocity = new Vector3(horizontalMove, 0, 0);
				transform.position += moveVelocity * movePower * Time.fixedDeltaTime;
				if (!anim.GetBool("isJump"))
					anim.SetBool("isRun", true);
			}

            if (isJumping)
            {
				rb.velocity = Vector2.zero;

				Vector2 jumpVelocity = new Vector2(0, jumpPower);
				rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

				isJumping = false;
			}
		}

        private void OnTriggerEnter2D(Collider2D other)
        {
            anim.SetBool("isJump", false);
        }

        void Attack()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                anim.SetTrigger("attack");
            }
        }
        void Hurt()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                anim.SetTrigger("hurt");
                if (horizontalMove == 1)
                    rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
                else
                    rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
            }
        }
        void Die()
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                anim.SetTrigger("die");
                
            }
        }
        void Restart()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                anim.SetTrigger("idle");
               
            }
        }
    }
}