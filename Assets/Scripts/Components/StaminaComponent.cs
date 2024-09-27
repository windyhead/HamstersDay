using Unity.Entities;
public struct StaminaComponent : IComponentData
{
	public int MaxStamina;
	public int Stamina;
	public int Fatigue;

	public StaminaComponent(int maxStamina)
	{
		MaxStamina = maxStamina;
		Stamina = MaxStamina;
		Fatigue = 0;
	}
	
	public int StaminaLeft => Stamina - Fatigue;
	
	public bool HasStamina()
	{
		return Stamina > Fatigue;
	}
	
	public void Move()
	{
		Fatigue ++;
	}
	

	public void Rest()
	{
		Fatigue = 0;
	}

	public void CalcStamina(int fat)
	{
		Stamina = MaxStamina - fat;
	}

	public void Reset()
	{
		Stamina = MaxStamina;
		Fatigue = 0;
	}
}
