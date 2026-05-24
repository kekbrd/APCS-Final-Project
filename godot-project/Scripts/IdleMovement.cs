using Godot;
using System;

public partial class IdleMovement : Node
{
   private static Random r = new Random();
   public static char setRandomDirection()
   {
   int i = r.Next(0, 4); // inclusive and exclusive
       if (i == 0)
       {
           return 'N';
       }
       else if (i == 1)
       {
           return 'E';
       }
       else if (i == 2)
       {
           return 'S';
       }
       else
       {
           return 'W';
       }
   }


   public static Vector2 startWalking(char cardinalDirection, AnimatedSprite2D sprite, float walkSpeed)
   {
       Vector2 result;
       result = setWalkVelocity(cardinalDirection);
       setWalkAnimation(sprite, result);
       return result *= walkSpeed;
   }


   private static Vector2 setWalkVelocity(char cardinalDirection)
   {
       if (cardinalDirection == 'N')
       {
           return new Vector2(0, -1);
       }
       else if (cardinalDirection == 'E')
       {
           return new Vector2(1, 0);
       }
       else if (cardinalDirection == 'S')
       {
           return new Vector2(0, 1);
       }
       else
       {
           return new Vector2(-1, 0);
       }
   }
   private static void setWalkAnimation(AnimatedSprite2D sprite, Vector2 Velocity)
   {
       if ((Velocity.Angle() < (-1 * Mathf.Pi / 4)) && (Velocity.Angle() > (-3 * Mathf.Pi / 4)))
       {
           sprite.Play("walk_up");
       }
       else if ((Velocity.Angle() <= (Mathf.Pi / 4)) && (Velocity.Angle() >= (-1 * Mathf.Pi / 4)))
       {
           sprite.Play("walk_right");
       }
       else if ((Velocity.Angle() > (Mathf.Pi / 4)) && (Velocity.Angle() < (3 * Mathf.Pi / 4)))
       {
           sprite.Play("walk_down");
       }
       else
       {
           sprite.Play("walk_left");
       }
   }


   public static void setIdleAnimation(AnimatedSprite2D sprite, char cardinalDirection)
   {
       if (cardinalDirection == 'N')
       {
           sprite.Play("idle_up");
       }
       else if (cardinalDirection == 'E')
       {
           sprite.Play("idle_right");
       }
       else if (cardinalDirection == 'S')
       {
           sprite.Play("idle_down");
       }
       else
       {
           sprite.Play("idle_left");
       }
   }
}