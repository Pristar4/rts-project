#region Info
// -----------------------------------------------------------------------
// CharacterRTS.cs
// 
// Felix Jung 07.11.2021
// -----------------------------------------------------------------------
#endregion
#region
using CodeMonkey.Utils;
using UnityEngine;
#endregion
public class CharacterRTS : MonoBehaviour, IGetPosition
{

	[SerializeField] private bool isPlayer;
	private SwordAttack attack;

	private HealthSystem healthSystem;
	private IMovePosition movePosition;
	private GameObject selectedGameObject;
	private CharacterRTS targetCharacterRTS;

	private void Awake()
	{
		healthSystem = new HealthSystem(100);
		movePosition = GetComponent<IMovePosition>();
		attack = GetComponent<SwordAttack>();
		selectedGameObject = transform.Find("Selected").gameObject;
		SetSelectedGameObjectVisible(false);
	}

	private void Start()
	{
		SetMovePosition(GetPosition());
	}

	private void Update()
	{
		if (targetCharacterRTS != null)
		{
			var attackDistance = 14f;
			if (Vector3.Distance(GetPosition(),
					    targetCharacterRTS.GetPosition()) <
			    attackDistance)
			{
				var attackDir
						= (targetCharacterRTS.GetPosition() - GetPosition())
						.normalized;
				FunctionTimer.Create(() => targetCharacterRTS.Damage(this),
						.05f);
				enabled = false;
				attack.Attack(attackDir, () => { enabled = true; });
			}
		}
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void SetSelectedGameObjectVisible(bool visible)
	{
		selectedGameObject.SetActive(visible);
	}

	public void Damage(CharacterRTS attacker)
	{
		healthSystem.Damage(56);

		var dirFromAttacker
				= (GetPosition() - attacker.GetPosition()).normalized;
		Blood_Handler.SpawnBlood(GetPosition(), dirFromAttacker);

		if (healthSystem.IsDead())
		{
			FlyingBody.Create(GameAssets.i.pfEnemyFlyingBody, GetPosition(),
					dirFromAttacker);
			Destroy(gameObject);
		}
	}

	public void SetMovePosition(Vector3 moveTargetPosition)
	{
		movePosition.SetMovePosition(moveTargetPosition);
	}

	public void SetTarget(CharacterRTS targetCharacterRTS)
	{
		this.targetCharacterRTS = targetCharacterRTS;

		if (targetCharacterRTS != null)
			SetMovePosition(targetCharacterRTS.GetPosition());
	}

	public HealthSystem GetHealthSystem()
	{
		return healthSystem;
	}

	public bool IsPlayer()
	{
		return isPlayer;
	}
}
