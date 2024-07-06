using UnityEngine;

public class Player : MonoBehaviour
{
	private IHamster hamster;
	private IPlanetHamster planetHamster;
	private bool isLeftPressed;
	private bool isRightPressed;

	private void Awake()
	{
		Initialize();
		InputManager.OnLeftPressed += LeftInputPressed;
		InputManager.onRightPressed += RightInputPressed;
	}

	private void RightInputPressed()
	{
		if (isRightPressed)
		{
			Fall();
			return;
		}

		isRightPressed = true;
		Move();
	}

	private void LeftInputPressed()
	{
		if (isLeftPressed)
		{
			Fall();
			return;
		}

		isLeftPressed = true;
		Move();
	}

	private void Fall()
	{
		planetHamster.Fall();
		Reset();
	}

	private void Move()
	{
		if (!isRightPressed || !isLeftPressed)
			return;
		
		Reset();
		planetHamster.Move();
	}

	private void Reset()
	{
		isLeftPressed = false;
		isRightPressed = false;
	}

	private void Initialize()
	{
		this.hamster = GetComponent<IHamster>();
		this.planetHamster = GetComponent<IPlanetHamster>();
	}
}