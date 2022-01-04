using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Godot;

public class InputActionAwaiter : Node
{

    TaskCompletionSource<bool> IsSomethingLoading;
    private string action;

    public override void _Ready()
    {
        IsSomethingLoading = new TaskCompletionSource<bool>(false);
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed(action))
            IsSomethingLoading.SetResult(true);
        
    }

    public async Task ToAction(string action)
    {
        this.action = action;
        await IsSomethingLoading.Task;
        IsSomethingLoading.SetResult(false);
    }
}
