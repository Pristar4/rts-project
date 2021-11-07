#region Info
// -----------------------------------------------------------------------
// PlayerPunch.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion

/*
 * Player movement with Arrow keys
 * Attack with Space
 * */
public class PlayerPunch : MonoBehaviour
{

	private const float SPEED = 50f;

	public static PlayerPunch instance;
	private Character_Base characterBase;
	private Material material;
	private Color materialTintColor;

	private PlayerMain playerMain;
	private State state;

	private void Awake()
	{
		instance = this;
		playerMain = GetComponent<PlayerMain>();
		characterBase = gameObject.GetComponent<Character_Base>();
		material = transform.Find("Body").GetComponent<MeshRenderer>().material;
		materialTintColor = new Color(1, 0, 0, 0);
	}

	private void Start()
	{
		SetStateNormal();
	}

	private void Update()
	{
		switch (state)
		{
			case State.Normal:
				//HandleMovement();
				HandleAttack();
				break;
			case State.Attacking:
				HandleAttack();
				break;
		}

		if (materialTintColor.a > 0)
		{
			var tintFadeSpeed = 6f;
			materialTintColor.a -= tintFadeSpeed * Time.deltaTime;
			material.SetColor("_Tint", materialTintColor);
		}
	}

	private void SetStateNormal()
	{
		state = State.Normal;
		playerMain.PlayerMovementHandler.Enable();
	}

	private void SetStateAttacking()
	{
		state = State.Attacking;
		playerMain.PlayerMovementHandler.Disable();
	}

	/*
	private void HandleMovement() {
	    float moveX = 0f;
	    float moveY = 0f;
	    
	    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
	        moveY = +1f;
	    }
	    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
	        moveY = -1f;
	    }
	    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
	        moveX = -1f;
	    }
	    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
	        moveX = +1f;
	    }
  
	    Vector3 moveDir = new Vector3(moveX, moveY).normalized;
	    bool isIdle = moveX == 0 && moveY == 0;
	    if (isIdle) {
	        characterBase.PlayIdleAnim();
	    } else {
	        characterBase.PlayMoveAnim(moveDir);
	        transform.position += moveDir * SPEED * Time.deltaTime;
	    }
	}
	*/

	private void HandleAttack()
	{
		if (Input.GetMouseButtonDown(0))
		{
			// Attack
			SetStateAttacking();

			var attackDir = (UtilsClass.GetMouseWorldPosition() - GetPosition())
					.normalized;

			var enemyHandler
					= EnemyHandler.GetClosestEnemy(
							GetPosition() + attackDir * 4f, 20f);
			bool hitEnemy;
			if (enemyHandler != null)
			{
				//enemyHandler.Damage(this);
				hitEnemy = true;
				attackDir = (enemyHandler.GetPosition() - GetPosition())
						.normalized;
				transform.position
						= enemyHandler.GetPosition() + attackDir * -12f;
			}
			else
			{
				hitEnemy = false;
				transform.position = transform.position + attackDir * 4f;
			}

			var attackAngle = UtilsClass.GetAngleFromVectorFloat(attackDir);

			// Play attack animation
			if (characterBase
					.IsPlayingPunchAnimation()) // Play Kick animation since punch animation is currently active
				characterBase.PlayKickAnimation(attackDir, impactPosition =>
				{
					if (hitEnemy)
					{
						impactPosition
								+= UtilsClass.GetVectorFromAngle(
										(int)attackAngle) * 4f;
						var impactEffect = Instantiate(
								GameAssets.i.pfImpactEffect,
								impactPosition, Quaternion.identity);
						impactEffect.eulerAngles
								= new Vector3(0, 0, attackAngle - 90);
					}
				}, SetStateNormal);
			else // Play Punch animation
				characterBase.PlayPunchAnimation(attackDir, impactPosition =>
				{
					if (hitEnemy)
					{
						impactPosition
								+= UtilsClass.GetVectorFromAngle(
										(int)attackAngle) * 4f;
						var impactEffect = Instantiate(
								GameAssets.i.pfImpactEffect,
								impactPosition, Quaternion.identity);
						impactEffect.eulerAngles
								= new Vector3(0, 0, attackAngle - 90);
					}
				}, SetStateNormal);
		}
	}

	private void DamageFlash()
	{
		materialTintColor = new Color(1, 0, 0, 1f);
		material.SetColor("_Tint", materialTintColor);
	}

	public void DamageKnockback(Vector3 knockbackDir, float knockbackDistance)
	{
		transform.position += knockbackDir * knockbackDistance;
		DamageFlash();
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	private enum State
	{
		Normal,
		Attacking
	}
}
