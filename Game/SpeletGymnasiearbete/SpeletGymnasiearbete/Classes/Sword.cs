using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

#nullable enable


using static SpeletGymnasiearbete.Utils;
namespace SpeletGymnasiearbete.Classes;

class Sword : AnimatedSprite{
    public Timer cdTimer; // Timer
    public int damage; // The damge to my brain
    public float cooldown;
    public Vector2 offset; // Where the holder is
    public int activateKey;
    public KeyboardState keyboard; // Keyboard
    public Sword(Texture2D? texture, Vector2 position, int TheLengthOfAnimationAmount, float FrameTick, int dmg, float CD , Vector2 OF, int aK47, KeyboardState keybeard) : base(texture, position, TheLengthOfAnimationAmount, FrameTick, false ){
        cooldown = CD;  
        offset = OF;     
        damage = dmg;
        cdTimer = new Timer(cooldown, false);
        activateKey = aK47;
        keyboard = keybeard;
    } 
    public void Update(GameTime gay){
        base.Update(gay); //Gaytime ;)
        cdTimer.Update(gay);
        if (!cdTimer.Finished){
            return;
        }else{
            if (keyboard.IsKeyDown(Keys.Space)){
                newTimer.StartTimer();
                cdTimer.StartTimer();
                // TODO GÖR COLLISION!
            }
        }

    }
  
}
