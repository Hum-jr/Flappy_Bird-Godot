using Godot;

public partial class Pipes : StaticBody2D
{
    [Export] public float Speed = 200f;
    
    public override void _Ready()
    {
        // Make sure the pipe is in the pipes group for collision detection
        AddToGroup("pipes");
    }
    
    public override void _Process(double delta)
    {
        // Move pipe towards the bird (left)
        Vector2 newPos = GlobalPosition;
        newPos.X -= Speed * (float)delta;
        GlobalPosition = newPos;
        
        // Remove when completely off-screen
        if (newPos.X < -100)
        {
            QueueFree();
        }
    }
}