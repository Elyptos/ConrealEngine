/*
	Copyright (c) 2017 Thomas Schöngrundner

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.
*/

using ConUI.Helper;
using ConUI.Widgets;
using ConUI.Widgets.Layout;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterBattleSimulator
{
    /// <summary>
    /// Domain data class
    /// </summary>
    public class MonsterData
    {
        //---------------------------------------------------------------TYPE DECLARATIONS----------------------------------------------------------------------
        public enum EMonsterType
        {
            EMONSTER_TYPE_ORK = 1,
            EMONSTER_TYPE_TROLL = 2,
            EMONSTER_TYPE_GOBLIN = 3
        }

        //---------------------------------------------------------------PUBLIC MEMBER VARIABLES----------------------------------------------------------------------
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int MaxHealth { get; set; } = 0;
        public int CurrentHealth { get; set; } = 0;
        public int MaxMana { get; set; } = 0;
        public int CurrentMana { get; set; } = 0;
        public int Strength { get; set; } = 0;
        public int Defense { get; set; } = 0;
        public int Agility { get; set; } = 0;
        public EMonsterType Type { get; set; } = EMonsterType.EMONSTER_TYPE_ORK;

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        /// <summary>
        /// Converts type of monster to string
        /// </summary>
        /// <returns></returns>
        public string TypeToString()
        {
            switch (Type)
            {
                case EMonsterType.EMONSTER_TYPE_GOBLIN:
                    return "Goblin";
                case EMonsterType.EMONSTER_TYPE_TROLL:
                    return "Troll";
                default:
                    return "Ork";
            }
        }

        /// <summary>
        /// Determines if this monster data is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return !String.IsNullOrEmpty(Name) && MaxHealth > 0 && Strength > 0 && Defense > 0 && Agility > 0;
        }

        /// <summary>
        /// Initializes this monster data.
        /// </summary>
        public void Init()
        {
            CurrentHealth = MaxHealth;
            CurrentMana = MaxMana;
        }
    }

    /// <summary>
    /// Monster registry
    /// </summary>
    static class MonsterLib
    {
        //---------------------------------------------------------------PUBLIC VARIABLES----------------------------------------------------------------------
        public static readonly string PATH_TO_SAVEFILE = "Save/Monsterlist.txt";

        public static List<MonsterData> MonsterList { get; set; } = new List<MonsterData>();

        //---------------------------------------------------------------PUBLIC METHODS------------------------------------------------------------------------
        /// <summary>
        /// Saves entire monster list to disk
        /// </summary>
        public static void SaveLib()
        {
            string json = JsonConvert.SerializeObject(MonsterLib.MonsterList);

            try
            {
                File.WriteAllText(PATH_TO_SAVEFILE, json);
            }
            catch (IOException)
            {
                Directory.CreateDirectory("Save");
                File.WriteAllText(PATH_TO_SAVEFILE, json);
            }
        }

        /// <summary>
        /// Loads monster list frim disk
        /// </summary>
        public static void LoadLib()
        {
            try
            {
                string json = File.ReadAllText(PATH_TO_SAVEFILE);

                MonsterLib.MonsterList = JsonConvert.DeserializeObject<List<MonsterData>>(json);
            }
            catch (IOException) { }
        }
    }

    /// <summary>
    /// Monster widget
    /// </summary>
    public class WMonster : WPanel
    {
        //---------------------------------------------------------------PRIVATE MEMBER VARIABLES----------------------------------------------------------------------
        private MonsterData _data;
        private bool _showFront;
        private WMonster monsterToAttack;

        //Animation variables
        private WPanel hitImage = new WPanel();
        private const float HIT_IMAGE_DURATION = 0.1f;
        private float hitCurrentTime = 0.0f;

        private const float FLASH_DURATION = 0.05f;
        private const int NUMBER_OF_FLASHES = 5;
        private int currNumberOfFlashes = 0;

        private IntVector2 startPos = new IntVector2();
        private IntVector2 initPos = new IntVector2();
        private IntVector2 destPos = new IntVector2();
        private IntVector2 attackOffset = new IntVector2(5, -5);
        private float animSpeed = 8.0f;
        private float animAlpha = 0.0f;
        

        //Animation states
        private bool showHitImage = false;
        private bool playAttackAnim = false;
        private bool playFlashAnim = false;

        //---------------------------------------------------------------PUBLIC MEMBER VARIABLES----------------------------------------------------------------------
        public MonsterData Status
        {
            get { return _data; }
            set
            {
                _data = value;
                UpdateImage();
            }
        }

        public bool ShowFront
        {
            get { return _showFront; }
            set
            {
                _showFront = value;
                UpdateImage();
            }
        }

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        public WMonster(MonsterData attributes)
        {
            _data = attributes;
        }

        /// <summary>
        /// Set monster domain data
        /// </summary>
        /// <param name="m">The monster data to add</param>
        /// <returns></returns>
        public WMonster SetMonster(MonsterData m)
        {
            Status = m;
            return this;
        }

        /// <summary>
        /// Show front sprites of monster
        /// </summary>
        /// <param name="b">Show front sprites</param>
        /// <returns></returns>
        public WMonster SetShowFront(bool b)
        {
            ShowFront = b;
            return this;
        }

        /// <summary>
        /// Perform attack against opponent
        /// </summary>
        /// <param name="other">The target to attack</param>
        public void Attack(WMonster other)
        {
            if (other == null)
                return;

            int damage = MathHelper.Clamp(this.Status.Strength - other.Status.Defense, 0, this.Status.Strength);
            other.Status.CurrentHealth = MathHelper.Clamp(other.Status.CurrentHealth - damage, 0, other.Status.MaxHealth);

            PlayAttackAnimations(other);
        }

        //---------------------------------------------------------------PROTECTED METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        /// <summary>
        /// Display hit sprite
        /// </summary>
        protected void ShowHitImage()
        {
            if(!showHitImage)
            {
                hitCurrentTime = 0.0f;
                hitImage.IsVisible = true;
                showHitImage = true;
            }
        }

        /// <summary>
        /// Trigger attack animation
        /// </summary>
        /// <param name="other">The monster to attack</param>
        protected void PlayAttackAnimations(WMonster other)
        {
            //Save monster for further remote access
            monsterToAttack = other;

            //Calculate necessary animation values
            startPos = LayoutSlot.RelativePosition;
            initPos = startPos;
            animAlpha = 0.0f;
            destPos = _showFront ? (startPos - attackOffset) : (startPos + attackOffset);

            playAttackAnim = true;
        }

        /// <summary>
        /// Should this widget tick
        /// </summary>
        /// <returns></returns>
        protected override bool ShouldTick()
        {
            return true;
        }

        //---------------------------------------------------------------PUBLIC METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        /// <summary>
        /// The construction function
        /// </summary>
        public override void OnConstruction()
        {
            this.SetTransparency(true);

            this.SetLayoutManager<LLayoutManager>(new LAlignmentLayout()
                + new LLayoutManager.Slot()
                .SetSizeManagement(LLayoutManager.ESizeManagement.ESIZEMANAGEMENT_ABSOLUTE)
                .SetSize(new IntVector2(10, 10))
                [
                    hitImage
                    .SetTransparency(true)
                    .DrawImage("Images/Hit.png")
                ]);

            hitImage.IsVisible = false;

            base.OnConstruction();
        }
        
        /// <summary>
        /// Plays animations
        /// </summary>
        /// <param name="deltaSeconds"></param>
        public override void OnTick(float deltaSeconds)
        {
            base.OnTick(deltaSeconds);

            if(showHitImage)
            {
                hitCurrentTime = MathHelper.Clamp(hitCurrentTime + deltaSeconds, 0f, HIT_IMAGE_DURATION);

                if(hitCurrentTime == HIT_IMAGE_DURATION)
                {
                    showHitImage = false;
                    hitImage.IsVisible = false;

                    playFlashAnim = true;
                    hitCurrentTime = 0.0f;
                    currNumberOfFlashes = 0;
                }
            }
            else if(playFlashAnim)
            {
                hitCurrentTime = MathHelper.Clamp(hitCurrentTime + deltaSeconds, 0f, FLASH_DURATION);

                if(hitCurrentTime == FLASH_DURATION)
                {
                    currNumberOfFlashes++;

                    if(currNumberOfFlashes == NUMBER_OF_FLASHES)
                    {
                        playFlashAnim = false;
                        this.IsVisible = true;
                    }
                    else
                    {
                        this.IsVisible = !this.IsVisible;
                        hitCurrentTime = 0f;
                    }
                }
            }

            if (playAttackAnim)
            {
                animAlpha = MathHelper.Clamp(animAlpha + animSpeed * deltaSeconds, 0f, 1f);

                if(animAlpha == 1f && LayoutSlot.RelativePosition == initPos)
                {
                    playAttackAnim = false;
                    monsterToAttack = null;
                }
                else if(animAlpha == 1f)
                {
                    animAlpha = 0f;

                    destPos = initPos;
                    startPos = destPos;

                    monsterToAttack.ShowHitImage();
                }
                else
                {
                    IntVector2 currPos = MathHelper.Lerp(startPos, destPos, animAlpha);

                    LayoutSlot.RelativePosition = currPos;
                }
            }
        }

        //---------------------------------------------------------------PRIVATE METHODS AND CONSTRUCTORS----------------------------------------------------------------------
        /// <summary>
        /// Changes sprites according to type of monster
        /// </summary>
        private void UpdateImage()
        {
            if (_showFront)
            {
                switch (Status.Type)
                {
                    case MonsterData.EMonsterType.EMONSTER_TYPE_ORK:
                        this.SpriteAnimations = new Dictionary<string, SpriteAnimation>() { { "Idle", new SpriteAnimation("Images/OrcFront", "Idle") } };
                        break;
                    case MonsterData.EMonsterType.EMONSTER_TYPE_GOBLIN:
                        this.SpriteAnimations = new Dictionary<string, SpriteAnimation>() { { "Idle", new SpriteAnimation("Images/GoblinFront", "Idle") } };
                        break;
                    default:
                        this.SpriteAnimations = new Dictionary<string, SpriteAnimation>() { { "Idle", new SpriteAnimation("Images/TrollFront", "Idle") } };
                        break;

                }

                this.CurrentAnimation = this.SpriteAnimations["Idle"];
                this.CurrentAnimation.AnimSpeed = 10.0f;
            }
            else
            {
                switch (Status.Type)
                {
                    case MonsterData.EMonsterType.EMONSTER_TYPE_ORK:
                        this.SpriteAnimations = new Dictionary<string, SpriteAnimation>() { { "Idle", new SpriteAnimation("Images/OrcBack", "Idle") } };
                        break;
                    case MonsterData.EMonsterType.EMONSTER_TYPE_GOBLIN:
                        this.SpriteAnimations = new Dictionary<string, SpriteAnimation>() { { "Idle", new SpriteAnimation("Images/GoblinBack", "Idle") } };
                        break;
                    default:
                        this.SpriteAnimations = new Dictionary<string, SpriteAnimation>() { { "Idle", new SpriteAnimation("Images/TrollBack", "Idle") } };
                        break;

                }

                this.CurrentAnimation = this.SpriteAnimations["Idle"];
                this.CurrentAnimation.AnimSpeed = 10.0f;
            }
        }
    }
}
