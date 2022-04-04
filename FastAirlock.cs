﻿using Planetbase;
using System;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;
using PlanetbaseModUtilities;
using System.Text;
using System.Diagnostics;


namespace FastAirlock
{
    public class FastAirlock : ModBase
    {
        
        public static float speedmult;
        public static string pathy;
        public static new void Init(ModEntry modEntry) => InitializeMod(new FastAirlock(), modEntry, "FastAirlock");

        public override void OnInitialized(ModEntry modEntry)
        {
            var path = "./Mods/FastAirlock/config.txt";
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            line = file.ReadLine();
            line = line.Substring(13);
            speedmult = float.Parse(line);
        }

        public override void OnUpdate(ModEntry modEntry, float timeStep)
        {
			var newUpdate = new CustomAirlock();
            newUpdate.update(timeStep);
            //CustomAirlock.update(timeStep);
        }

    }

    public class CustomAirlock : InteractionAirlock
    {
        public override bool update(float timeStep2)
        {
			
            try
			{
                float timeStep = timeStep2 * FastAirlock.speedmult;
                if (this.mSelectable is Construction construction && !construction.isPowered() && this.mStage == InteractionAirlock.Stage.Wait)
                {
                    return true;
                }

                if (this.mSelectable is Construction && this.mSelectable.getFirstInteraction() == this)
                {
                    this.mStageProgress += timeStep;
                    if (this.mStageProgress > 1f || this.mStage == InteractionAirlock.Stage.Wait)
                    {
                        bool flag = this.mStage == InteractionAirlock.Stage.Exit;
                        this.onStageDone();
                        this.mStageProgress = 0f;
                        if (flag)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    this.mStage = InteractionAirlock.Stage.Wait;
                    this.mTarget = this.getQueuePosition(this.mSelectable.getInteractionIndex(this));
                }
                Vector3 direction = this.mTarget - this.mCharacter.getPosition();
                float magnitude = direction.magnitude;
                float d = Mathf.Min(4f * timeStep, magnitude);
                if (magnitude > 0.25f)
                {
                    Vector3 target;
                    if (this.mStage == InteractionAirlock.Stage.Wait && magnitude < 1f && this.mSelectable is Construction)
                    {
                        target = (this.mSelectable.getPosition() - this.mCharacter.getPosition()).flatDirection();
                    }
                    else
                    {
                        target = direction.flatDirection();
                    }
                    Vector3 direction2 = this.mCharacter.getDirection();
                    this.mCharacter.setPosition(this.mCharacter.getPosition() + direction.normalized * d);
                    this.mCharacter.setDirection(Vector3.RotateTowards(direction2, target, 6.28318548f * timeStep, 0.1f));
                    if (this.mAnimationType != CharacterAnimationType.Walk)
                    {
                        this.mAnimationType = CharacterAnimationType.Walk;
                        this.mCharacter.playWalkAnimation();
                    }
                }
                else
                {
                    if (this.mStage == InteractionAirlock.Stage.GoEntry)
                    {
                        this.mStageProgress = 1f;
                    }
                    if (this.mAnimationType != CharacterAnimationType.Idle)
                    {
                        this.mAnimationType = CharacterAnimationType.Idle;
                        this.mCharacter.playIdleAnimation(CharacterAnimation.PlayMode.CrossFade);
                    }
                }
                return false;
            }
            catch (NullReferenceException exception)
			{
                Logger.LogException(exception);
            }
            return true;
        }

		protected override Vector3 getQueuePosition(int i)
		{
			throw new NotImplementedException();
		}

		protected override void onStageDone()
		{
			throw new NotImplementedException();
		}
	}
    public class ExtensionAttribute : Attribute { }
}
