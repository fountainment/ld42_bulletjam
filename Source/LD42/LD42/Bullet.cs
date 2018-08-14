using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace LD42
{
    class Bullet : Entity
    {
        public Vector2 Speed;
        public Image BulletImage;
        public static float InitialScale = 0.5f;
        public int Damage = 40;

        public static Bullet Create(Vector2 position, Vector2 speed, bool needRandom = true)
        {
            var bullet = new Bullet();
            bullet.BulletImage = new Image(Game1.BulletImage);
            bullet.BulletImage.Scale = new Vector2(InitialScale);
            bullet.BulletImage.CenterOrigin();
            if (needRandom) bullet.BulletImage.Rotation += Calc.Random.NextFloat();
            bullet.Add(bullet.BulletImage);
            bullet.Collider = new Circle(Game1.BulletImage.Height / 2.0f);
            bullet.Position = position;
            bullet.Speed = speed;
            if (float.IsNaN(speed.Length()))
            {
                bullet.Speed = -Vector2.UnitY * 10.0f;
            }
            if (needRandom) bullet.Speed.X += Calc.Random.NextFloat() - 0.5f;
            bullet.Tag = Game1.BulletTag;
            return bullet;
        }

        public static Bullet CreateEnemyBullet(Vector2 position, Vector2 speed, bool needRandom = true)
        {
            var bullet = Create(position, speed, needRandom);
            bullet.Tag = Game1.EnemyBulletTag;
            return bullet;
        }

        public override void Update()
        {
            base.Update();
            Position += Speed;
            bool outsideScreen = Game1.CheckOutsidePlayzone(ref Position);
            if (Tag == Game1.LittleBulletTag)
            {
                if (outsideScreen || CollideCheck(Game1.DeadBulletTag))
                {
                    Speed = Vector2.Zero;
                    Tag = Game1.DeadBulletTag;
                    if (outsideScreen)
                    {
                        Game1.CheckOutsidePlayzone(ref Position, true);
                    }
                }
            }
            else if (Tag != Game1.DeadBulletTag)
            {
                int collideCount = 0;
                if (!outsideScreen)
                {
                    collideCount = CollideAll(Game1.DeadBulletTag).Count;
                }
                if (outsideScreen || collideCount > 0) {
                    BulletImage.Scale.Y *= (float)Math.Pow(0.95f, collideCount + 1);
                    if (BulletImage.Scale.Y == InitialScale)
                    {
                        Speed.X += (Calc.Random.NextFloat() - 0.5f) * 5.0f;
                    }
                    if (BulletImage.Scale.Y <= InitialScale * 0.8f || outsideScreen)
                    {
                        RemoveSelf();
                        int[] offset = new int[5] { -2, -1, 0, 1, 2 };
                        for (var i = 0; i < 5; i++)
                        {
                            var pvec = Vector2.Normalize(Speed.Perpendicular());
                            var bullet = Bullet.Create(Position + Speed * 0.5f + offset[i] * 10.0f * pvec, Speed + offset[i] * 2.0f * pvec);
                            bullet.BulletImage.Color = BulletImage.Color;
                            bullet.BulletImage.Texture = BulletImage.Texture;
                            float scale = 0.7f;
                            int x = Math.Abs(offset[i]);
                            if (x > 0)
                            {
                                if (x == 1) scale *= 0.8f;
                                else if (x == 2) scale *= 0.6f;
                            }
                            float colliderSize = scale * 0.5f - 0.05f;
                            bullet.BulletImage.Scale = new Vector2(scale);
                            bullet.Collider = new Circle(BulletImage.Width * colliderSize);
                            bullet.Tag = Game1.LittleBulletTag;
                            Scene.Add(bullet);
                        }
                    }
                }
            }
            //Depth = (int)-Position.Y;
        }
    }
}