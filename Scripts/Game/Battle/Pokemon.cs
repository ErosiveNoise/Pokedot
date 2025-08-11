using System.Collections.Generic;
using Godot;

public class Attack
{
    public string Name { get; set; }
    public int Power { get; set; }
    public int Accuracy { get; set; }
    public int PP { get; set; }
    public int CurrentPP { get; set; }
    public string Type { get; set; }
}

public partial class Pokemon : Node
{
	public new string Name { get; set; }
	public int Hp { get; set; }
	public int CurrentHp { get; set; }
	public int Level { get; set; }
	public int Exp { get; set; }
	public string Type { get; set; }
	public List<Attack> Attacks { get; set; }
	public int Speed { get; set; }


	public override void _Ready()
	{
		CurrentHp = Hp;
	}

	// public void Attac(int attacNum)
	// {
	// 	int selectedAttac = attacNum;
	// 	if (selectedAttac == 0)
	// 	{
	// 		selectedAttac = GetRandomAttac();
	// 	}
	// 	//call method 
	// }

	// public int GetRandomAttac()
	// {
	// 	return 0;
	// }
}
