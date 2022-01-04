using Godot;
using System;

public class Environment : Spatial
{
	// Member variables here, example:
	// private int a = 2;
	// private string b = "textvar";
	private ReflectionProbe reflectionProbe;
	private WorldEnvironment worldEnvironment;

	public override void _Ready()
	{
		reflectionProbe = (ReflectionProbe)GetNode("ReflectionProbe");
		worldEnvironment = (WorldEnvironment)GetNode("WorldEnvironment");
		worldEnvironment.Environment.FogEnabled = false;
	}

	public override void _Process(float delta)
	{

	}
}
