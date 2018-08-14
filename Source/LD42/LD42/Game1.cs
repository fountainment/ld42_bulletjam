using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Monocle;

namespace LD42
{
    public class Game1 : Engine
    {
        public static MTexture BGImage;
        public static MTexture BulletImage;
        public static MTexture WhiteBulletImage;
        public static MTexture PlayerImage;
        public static MTexture CursorImage;
        public static MTexture EnemyImage;
        public static MTexture[] PlayerImageArray;
        public static MTexture[] PlayerHPImageArray;
        public static MTexture[] PlayerFootImageArray;
        public static MTexture[] SlimeWalkImageArray;
        public static MTexture[] SlimeBWalkImageArray;
        public static MTexture[] SlimeCWalkImageArray;
        public static MTexture[] SlimeCCWalkImageArray;
        public static MTexture[] SlimeDWalkImageArray;
        public static MTexture[] BossWalkImageArray;
        public static MTexture[] BombBangImageArray;
        public static MTexture[] BombNumImageArray;
        public static MTexture BombItemImage;
        public static MTexture BombImage;
        public static MTexture BombRedImage;
        public static MTexture BossImage;
        public static MTexture BossBloodBImage;
        public static MTexture BossBloodImage;
        public static MTexture BossBloodTImage;

        public static BitTag WallTag;
        public static BitTag BulletTag;
        public static BitTag LittleBulletTag;
        public static BitTag DeadBulletTag;
        public static BitTag SuckBulletTag;
        public static BitTag EnemyBulletTag;
        public static BitTag EnemyTag;
        public static BitTag ItemTag;

        public Game1() : base(600, 700, 600, 700, "LD42", false)
        {
            WallTag = new BitTag("wall");
            BulletTag = new BitTag("bullet");
            LittleBulletTag = new BitTag("little_bullet");
            DeadBulletTag = new BitTag("dead_bullet");
            SuckBulletTag = new BitTag("suck_bullet");
            EnemyBulletTag = new BitTag("enemy_bullet");
            EnemyTag = new BitTag("enemy");
            ItemTag = new BitTag("item");

            Engine.ClearColor = Color.DarkCyan;
        }

        public MTexture LoadTex(String name)
        {
            //return LoadTexFromFile(name + ".png");
            return new MTexture(Content.Load<Texture2D>(name));
        }

        public MTexture LoadTexFromFile(String name)
        {
            return MTexture.FromFile(name);
        }

        private void Restart()
        {
            var bulletList = Engine.Scene.GetEntitiesByTagMask(Game1.BulletTag);
            foreach (var bullet in bulletList)
            {
                Engine.Scene.Remove(bullet);
            }
            var enemyBulletList = Engine.Scene.GetEntitiesByTagMask(Game1.EnemyBulletTag);
            foreach (var bullet in enemyBulletList)
            {
                Engine.Scene.Remove(bullet);
            }
            var deadBulletList = Engine.Scene.GetEntitiesByTagMask(Game1.DeadBulletTag);
            foreach (var bullet in deadBulletList)
            {
                Engine.Scene.Remove(bullet);
            }
            var enemyList = Engine.Scene.GetEntitiesByTagMask(Game1.EnemyTag);
            foreach (var enemy in enemyList)
            {
                Engine.Scene.Remove(enemy);
            }
            Engine.Scene.Add(Player.Instance);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            BGImage = LoadTex("BG");
            BulletImage = LoadTex("bullet");
            WhiteBulletImage = LoadTex("bullet_white");
            CursorImage = LoadTex("mouse");
            EnemyImage = LoadTex("player1");
            PlayerImage = LoadTex("player1");
            PlayerFootImageArray = new MTexture[4];
            PlayerFootImageArray[0] = LoadTex("playerfoot1");
            PlayerFootImageArray[1] = LoadTex("playerfoot2");
            PlayerFootImageArray[2] = LoadTex("playerfoot3");
            PlayerFootImageArray[3] = LoadTex("playerfoot4");
            SlimeWalkImageArray = new MTexture[8];
            SlimeBWalkImageArray = new MTexture[8];
            SlimeCWalkImageArray = new MTexture[8];
            SlimeCCWalkImageArray = new MTexture[8];
            SlimeDWalkImageArray = new MTexture[8];
            BossWalkImageArray = new MTexture[8];
            PlayerImageArray = new MTexture[8];
            for (int i = 0; i < 8; i++)
            {
                PlayerImageArray[i] = LoadTex(String.Format("player{0}", i + 1));
                SlimeWalkImageArray[i] = LoadTex(String.Format("slime_walk{0}", i + 1));
                SlimeBWalkImageArray[i] = LoadTex(String.Format("slimeB_walk{0}", i + 1));
                SlimeCWalkImageArray[i] = LoadTex(String.Format("slimeC_walk{0}", i + 1));
                SlimeCCWalkImageArray[i] = LoadTex(String.Format("slimeCC_walk{0}", i + 1));
                SlimeDWalkImageArray[i] = LoadTex(String.Format("slimeD_walk{0}", i + 1));
                BossWalkImageArray[i] = LoadTex(String.Format("boss_walk{0}", i + 1));
            }
            BombBangImageArray = new MTexture[10];
            for (int i = 0; i < 10; i++)
            {
                BombBangImageArray[i] = LoadTex(String.Format("bomb_clear_bang{0}", i + 1));
            }
            BombImage = LoadTex("bomb_clear1");
            BombRedImage = LoadTex("bomb_clear2");
            BossImage = LoadTex("boss");
            BossBloodImage = LoadTex("boss_blood");
            BossBloodBImage = LoadTex("boss_HPb");
            BossBloodTImage = LoadTex("boss_HP");
            PlayerHPImageArray = new MTexture[7];
            for (int i = 0; i < 7; i++)
            {
                PlayerHPImageArray[i] = LoadTex(String.Format("player_HP{0}", i + 1));
            }
            BombNumImageArray = new MTexture[4];
            BombNumImageArray[0] = LoadTex("bomb_num1");
            BombNumImageArray[1] = LoadTex("bomb_num2");
            BombNumImageArray[2] = LoadTex("bomb_num3");
            BombNumImageArray[3] = LoadTex("bomb_num4");
            BombItemImage = LoadTex("bomb_icon");
        }

        protected override void Initialize()
        {
            base.Initialize();

            //Window
            Window.AllowUserResizing = false;

            //Scene
            Scene scene = new Scene1();
            Engine.Scene = scene;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (MInput.Keyboard.Pressed(Keys.R))
            {
                Restart();
            }
        }

        public static bool CheckOutsidePlayzone(ref Vector2 position, bool normalize = false)
        {
            bool outside = position.Y < 150.0f || position.X < 50.0f || position.Y > 650.0f || position.X > 550.0f;
            if (normalize)
            {
                if (position.X < 50.0f) position.X = 50.0f;
                if (position.Y < 150.0f) position.Y = 150.0f;
                if (position.X > 550.0f) position.X = 550.0f;
                if (position.Y > 650.0f) position.Y = 650.0f;
            }
            return outside;
        }
    }
}
