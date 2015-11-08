using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using org.flixel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lemonade
{
    public class Actor : FlxPlatformActor
    {
        public bool piggyBacking;
        public float trampolineTimer = 200000;
        protected const float trampolineMaxLimit = 0.065f;
        public float dashTimer = 200000;
        protected const float dashMaxLimit = 0.075f;
        public string methodOfDeath;

        /// <summary>
        /// reference to liselot for destroying piggyback action
        /// </summary>
        public Liselot liselot;

        public Actor(int xPos, int yPos)
            : base(xPos, yPos)
        {
            trampolineTimer = float.MaxValue;


            addAnimationCallback(resetAfterDeath);

            play("idle");

            piggyBacking = false;

        }

        override public void update()
        {
            if (control == Controls.none) alpha = 0.95f;
            else alpha = 1.0f;


            if (piggyBacking == true)
            {
                animationPrefix = "piggyback_";
            }
            else
            {
                animationPrefix = "";
            }

            trampolineTimer += FlxG.elapsed;
            dashTimer += FlxG.elapsed;

            if (trampolineTimer < trampolineMaxLimit)
            {
                acceleration.Y = Lemonade_Globals.GRAVITY *-1;
            }
            else
            {
                acceleration.Y = Lemonade_Globals.GRAVITY;
            }

            if (x < 0) x = FlxG.levelWidth;
            if (x > FlxG.levelWidth) x = 10;
            //if (y < 0) y = FlxG.levelHeight;
            if (y > FlxG.levelHeight) y = 0;

            base.update();
        }

        public override void hitBottom(FlxObject Contact, float Velocity)
        {
            base.hitBottom(Contact, Velocity);
        }

        public override void overlapped(FlxObject obj)
        {
            base.overlapped(obj);

            

            if (obj.GetType().ToString() == "Lemonade.Trampoline" && !dead)
            {
                velocity.Y = -1000;
                trampolineTimer = 0.0f;
                //this.methodOfDeath = obj.GetType().ToString();
            }
            else if (obj.GetType().ToString() == "Lemonade.Ramp")
            {
                float delta = x % 20;
                //this.methodOfDeath = obj.GetType().ToString();
                //FlxU.solveXCollision(obj, null);

            }
            else if (obj.GetType().ToString() == "Lemonade.Spike")
            {
                //Console.WriteLine("Spike overlapp: " + this.GetType().ToString());

                if (dead == false && onScreen() )  FlxG.play("Lemonade/sfx/deathSFX", 0.8f, false);
                
                hurt(1);

                this.methodOfDeath = obj.GetType().ToString();

            }
        }

        public void resetAfterDeath(string Name, uint Frame, int FrameIndex)
        {
            //Console.WriteLine("Name {0} Frame {1}",Name, Frame);

            if (this.GetType().ToString() == "Lemonade.Army" || 
                this.GetType().ToString() == "Lemonade.Inspector" || 
                this.GetType().ToString() == "Lemonade.Worker" || 
                this.GetType().ToString() == "Lemonade.Chef"  )
            {
                if (Name == "death" && Frame >= _curAnim.frames.Length - 1 && this.methodOfDeath=="Lemonade.Spike")
                {
                    reset(originalPosition.X, originalPosition.Y);
                    dead = false;
                    //control = Controls.player;
                    //play("idle");

                    this.startPlayingBack();

                }
            }
        }

        public override void kill()
        {
            
            control = Controls.none;
            dead = true;

            //base.kill();
        }


    }
}
