using Godot;
using System;
using FlappyBird;

public partial class Bird : CharacterBody2D
{
    [Signal]
    public delegate void BirdDiedEventHandler();
    
    [Export] public float JumpStrength = -400f;
    [Export] public float Gravity = 980f;
    [Export] public float MaxFallSpeed = 600f;
    
    private bool _isAlive = true;
    private bool _gameStarted = false;
    private float _floatTimer = 0f;
    
    public override void _Ready()
    {
        base._Ready();
        
        // Start with bird gently floating (pre-game state)
        Velocity = Vector2.Zero;
        
        

    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isAlive)
            return;
            
        // Handle input for jumping
        if (Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("ui_up") || 
            Input.IsKeyPressed(Key.Space))
        {
            Jump();
            if (!_gameStarted)
                _gameStarted = true;
        }
        
        // Apply gravity only after game starts
        if (_gameStarted)
        {
            // Apply gravity
            Vector2 newVelocity = Velocity;
            newVelocity.Y += Gravity * (float)delta;
            
            // Limit fall speed
            if (newVelocity.Y > MaxFallSpeed)
                newVelocity.Y = MaxFallSpeed;
                
            Velocity = newVelocity;
        }
        else
        {
            // Gentle floating animation before game starts (centered oscillation)
            _floatTimer += (float)delta;
            float floatOffset = Mathf.Sin(_floatTimer * 3) * 30;
            Velocity = new Vector2(0, floatOffset);
        }
        
        // Move the bird
        MoveAndSlide();
        
        // Check for collisions
        CheckCollisions();
        
        // Keep bird rotation based on velocity
        UpdateRotation();
    }
    
    private void Jump()
    {
        if (!_isAlive)
            return;
            
        Velocity = new Vector2(Velocity.X, JumpStrength);
    }
    
    private void CheckCollisions()
    {
        // Check if bird hit the ground or ceiling
        if (GlobalPosition.Y > GetViewportRect().Size.Y || GlobalPosition.Y < 0)
        {
            Die();
            return;
        }
        
        // Check collisions with pipes (collision layers)
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision.GetCollider() is Node2D collider)
            {
                // If we hit a pipe
                if (collider.IsInGroup("pipes"))
                {
                    Die();
                    return;
                }
            }
        }
    }
    
    private void UpdateRotation()
    {
        if (!_gameStarted)
        {
            Rotation = 0;
            return;
        }
        
        // Rotate bird based on velocity
        float targetRotation = Mathf.Clamp(Velocity.Y * 0.001f, -0.5f, 1.5f);
        Rotation = Mathf.LerpAngle(Rotation, targetRotation, 0.1f);
    }
    
    private void Die()
    {
        if (!_isAlive)
            return;
            
        _isAlive = false;
        EmitSignal(SignalName.BirdDied);
        
        // Optional: Add death animation or effects here
        // You might want to disable collision or add particle effects
    }
    
    public void Reset()
    {
        _isAlive = true;
        _gameStarted = false;
        Velocity = Vector2.Zero;
        Rotation = 0;
        
        // Reset position to starting position
        // You'll want to set this to your desired starting position
        GlobalPosition = new Vector2(100, GetViewportRect().Size.Y / 2);
    }
    
    public bool IsAlive()
    {
        return _isAlive;
    }
    
    public bool HasGameStarted()
    {
        return _gameStarted;
    }
}