using Godot;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class Game : Node
{
	[Signal]
	public delegate void load_thread_finished();
	public Node CurrentScene { get; set; }
	public int waitFrames;
	public bool isLoading;
	private ResourceInteractiveLoader loader;
	public int loadProgress;
	private const int timeMax = 100; //milliseconds
	public NodePath currentPlayerNodePath;
	private bool threadRunning;
	public string next_level;
	public bool isFirstLaunch;

	public override void _Ready()
	{
		Viewport root = GetTree().GetRoot();
		CurrentScene = GetTree().CurrentScene;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		AddToGroup("Persist");
	}

	public void SaveNodes()
	{
		var saveGame = new File();

		saveGame.Open("user://game.dat", File.ModeFlags.Write);
		
		var saveNodes = GetTree().GetNodesInGroup("Persist");

		foreach (Node saveNode in saveNodes)
		{
			var nodeData = saveNode.Call("Save", this);
			saveGame.StoreLine(JSON.Print(nodeData));
		}
		saveGame.Close();
		GD.Print("GAME: Saved");
		/*
		if (saveGame.FileExists("user://game.dat"))
		{
			saveGame.Open("user://game.dat", (int)File.ModeFlags.ReadWrite);
			var saveNodes = GetTree().GetNodesInGroup("Persist");

			foreach (Node saveNode in saveNodes)
			{
				var currentLine = saveGame.GetLine();
				if (!currentLine.Contains(saveNode.GetPath()))
				{
					saveGame.SeekEnd();
				}
					
				var nodeData = saveNode.Call("Save", this);
				saveGame.StoreLine(JSON.Print(nodeData));
			}
			saveGame.Close();
			GD.Print("GAME: Saved");
		} else
		{
			saveGame.Open("user://game.dat", (int)File.ModeFlags.Write);
			var saveNodes = GetTree().GetNodesInGroup("Persist");

			foreach (Node saveNode in saveNodes)
			{
				var nodeData = saveNode.Call("Save", this);
				saveGame.StoreLine(JSON.Print(nodeData));
			}
			saveGame.Close();
			GD.Print("GAME: Saved");
		}
		*/
	}
			/*added string, object */
	public Dictionary<string, object> GetSaveDictionary(Node saveNode)
	{
		var helper = new Node();
		helper.SetScript(ResourceLoader.Load("res://scripts/autoload/load_helper.gd"));
		var path = saveNode.GetPath().ToString();
		var dict = helper.Call("get_save_dict", "path", path);
		helper.QueueFree();
		return (Dictionary<string, object>)dict;
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("fullscreen"))
		{
			OS.SetWindowMaximized(!OS.WindowMaximized);
		}
		if (Input.IsActionJustPressed("ui_up"))
			SaveNodes();
	}

	public void GoToScene(string path)
	{
		loader = ResourceLoader.LoadInteractive(path);

		//switch to loading screen scene
		var loadingScreen = (PackedScene)ResourceLoader.Load("res://scenes/menus/loading/LoadingScreen.tscn");
		GetTree().ChangeSceneTo(loadingScreen);

		waitFrames = 1;
		isLoading = true;
	}
	
	public void BackgroundLoadScene(string path)
	{
		loader = ResourceLoader.LoadInteractive(path);

		var loadingScreen = (PackedScene)ResourceLoader.Load("res://scenes/menus/loading/LoadingScreen.tscn");

		GetTree().ChangeSceneTo(loadingScreen);

		System.Threading.Thread thread = new System.Threading.Thread(ThreadedLoad);   
		thread.Start();
	}

	private void ThreadedLoad()
	{
		threadRunning = true;
		bool workDone = false;
		loadProgress = 0;

		while(threadRunning && !workDone)
		{
			if (loader == null)
			{
				isLoading = false;
				break;
			}

			if (waitFrames > 0)
			{
				waitFrames--;
				break;
			}

			var t = OS.GetTicksMsec();

			var err = loader.Poll();

			if (err == Error.FileEof)
			{
				//Reached end of file, ready to get resource from the loader
				var resource = loader.GetResource();
				loader = null;

				//Change CurrentScene to new scene from the resource
				var newScene = (PackedScene)resource;
				GetTree().ChangeSceneTo(newScene);

				//stop this and clean up
				workDone = true;
				EmitSignal("load_thread_finished");
				break;
			}
			else if (err == Godot.Error.Ok)
			{
				//update load progress so loading screen visuals follow along
				loadProgress = Mathf.RoundToInt((float)loader.GetStage() / loader.GetStageCount() * 100);
			}
			else
			{
				GD.PrintErr(err);
				loader = null;
				break;
			}

			if (workDone)
				return;
			
		}

		loadProgress = 0;
		threadRunning = false;
	}

	public void LoadNodes()
	{
		var saveGame = new File();
		if (saveGame.FileExists("user://game.dat"))
		{
			var saveNodes = GetTree().GetNodesInGroup("Persist");

			// Load the file line by line and process that dictionary to restore the object it represents
			// saveGame.OpenEncryptedWithPass("user://game.dat", (int)File.ModeFlags.Read, "nullistruenullisgood");
			//	                              removed (int) \/	
			//saveGame.OpenEncryptedWithPass("user://game.dat", File.ModeFlags.Read, "nullistruenullisgood");
			saveGame.Open("user://game.dat", File.ModeFlags.Read);

			//Scan line by line
			while (!saveGame.EofReached())
			{
				var currentLine = (Dictionary<string, object>)JSON.Parse(saveGame.GetLine()).Result;
				if (currentLine == null)
					continue;

				//iterates through each saveNode in the group
				foreach (Node saveNode in saveNodes)
				{
					// Now we set the variables.
					foreach (var i in currentLine.Keys)
					{
						var key = i.ToString();

						//ignores the key if its the node path
						if (key == "path")
							continue;

						if (currentLine.TryGetValue(i, out var value))
						{
							//checks if this current line represents the saveNode by comparing paths
							if (saveNode.GetPath() == currentLine["path"].ToString())
							{
								//if true, then set its properties
								saveNode.Set(key, value);
								GD.Print("GAME: Setting SaveNode " + saveNode.Name + " KEY '" + key + "' VALUE '" + value + "'");
							}
						}
					}
				}

			}
			saveGame.Close();
			GD.Print("GAME: Loaded");
		}
		else
		{
			GD.Print("GAME: No save file found to load");
		}


	}

	public Dictionary<string, object> Save(Game game)
	{
		var dict = GetSaveDictionary(this);
		dict.Add("next_level", next_level);
		dict.Add("isFirstLaunch", isFirstLaunch);
		return dict;
	}
}
