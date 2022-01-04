using Godot;
using System;

public class CustomButton : Container
{
	[Signal]
	public delegate void pressed();

	private MarginContainer contents;
	private bool mouseInside;
	private NinePatchRect texture;
	private Label label;
	private Control animatedSelect;
	private bool disabled;

	public override void _Ready()
	{
		mouseInside = false;
		label = (Label)GetNode("Label");
		label.Connect("mouse_entered", this, "MouseEntered");
		label.Connect("mouse_exited", this, "MouseExited");
		animatedSelect = (Control)GetNode("AnimatedSelect");
		disabled = false;

	}

	public override void _Process(float delta)
	{
		if (MouseFilter == MouseFilterEnum.Ignore)
			disabled = true;
		else
			disabled = false;
		if (!disabled)
		{
			if (mouseInside && Input.IsActionJustPressed("select"))
			{
				Pressed();
			}
			else if (mouseInside)
			{
				Hovered();
			}
			else
			{
				UnHovered();
			}
			
			if (mouseInside && Input.IsActionJustReleased("select"))
			{
				UnHovered();
				EmitSignal("pressed");
			}
		} else
		{
			label.SetSelfModulate(new Color("555555"));
		}
	}

	private void Pressed()
	{
		label.SetSelfModulate(new Color("555555"));
	}

	private void Hovered()
	{
		animatedSelect.SetVisible(true);
	}

	private void UnHovered()
	{
		label.SetSelfModulate(new Color("ffffff"));
		animatedSelect.SetVisible(false);
	}

	private void Released()
	{
		label.SetSelfModulate(new Color("ffffff"));
	}

	public void MouseEntered() 
	{
		mouseInside = true;
		
	}

	public void MouseExited()
	{
		mouseInside = false;

	}
}
