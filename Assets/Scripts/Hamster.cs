using UnityEngine;

public class Hamster : MonoBehaviour, IPlanetHamster, IHamster
{
	public int Age;
	
	[SerializeField] private Transform transform;
	
	public void Move()
	{
		
		transform.Translate( Vector3.forward * Time.deltaTime * 30);
	}

	public void Fall()
	{
		
	}
	
	public void Jump()
	{
		transform.Translate(new Vector3(1,2,0)  * Time.deltaTime * 40);
	}

	public void Eat()
	{
		
	}
}