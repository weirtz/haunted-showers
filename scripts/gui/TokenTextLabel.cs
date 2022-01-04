using Godot;
using System;

public class TokenTextLabel : RichTextLabel
{

    // Member variables here, example:
    // private int a = 2;
    // private string b = "textvar";

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
    }

    public override void _Process(float delta)
    {
        string[] key = Text.Split(" : ");
        key[1] = "" + ((Player)GetNode("/root/Main/Functional/Player")).money;
        Text = key[0] + " : " + key[1];
    }
    

}
