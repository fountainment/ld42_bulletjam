using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Microsoft.Xna.Framework;

namespace LD42
{
    class Enemy : Entity
    {
        private Enemy() : base() { }

        private int HP = 100;
        private Alarm ShootTimer;
        private Vector2 ShootVec;
        private Image EnemyImage;
        private float EnemyTime = 0.0f;
        private int WalkAnimIndex = 0;
        private bool IsDead = false;
        private Vector2 DeadSpeed = Vector2.Zero;
        private MTexture[] AnimTex;
        private float JamSize = 1.0f;
        private Color JamColor;
        private int SlimeType = 0;

        public static Enemy Create(int type = 0)
        {
            var enemy = new Enemy();
            float colliderSize = 8.0f;
            enemy.Collider = new Circle(colliderSize);
            enemy.SlimeType = type;
            if (type == 0)
            {
                //Green
                enemy.AnimTex = Game1.SlimeWalkImageArray;
                enemy.HP = 100;
                enemy.JamColor = new Color(55, 148, 110);
                enemy.JamSize = 1.0f;
                enemy.Collider = new Circle(colliderSize);
            }
            else if (type == 1)
            {
                //Yellow
                enemy.AnimTex = Game1.SlimeBWalkImageArray;
                enemy.HP = 200;
                enemy.JamColor = new Color(227, 186, 50);
                enemy.JamSize = 1.5f;
                enemy.Collider = new Circle(colliderSize * 2.0f);
            }
            else if (type == 2)
            {
                //Purple
                enemy.AnimTex = Game1.SlimeCWalkImageArray;
                enemy.HP = 150;
                enemy.JamColor = new Color(179, 70, 139);
                enemy.JamSize = 2.0f;
                enemy.Collider = new Circle(colliderSize * 2.5f);
            }
            else if (type == 3)
            {
                //Rainbow
                enemy.AnimTex = Game1.SlimeDWalkImageArray;
                enemy.HP = 150;
                enemy.JamColor = new Color(227, 186, 50);
                enemy.JamSize = 2.0f;
                enemy.Collider = new Circle(colliderSize * 2.5f);
            }
            else if (type == 4)
            {
                //Little Purple
                enemy.AnimTex = Game1.SlimeCCWalkImageArray;
                enemy.HP = 100;
                enemy.JamColor = new Color(179, 70, 139);
                enemy.JamSize = 1.0f;
                enemy.Collider = new Circle(colliderSize);
            }
            enemy.EnemyImage = new Image(enemy.AnimTex[0]);
            enemy.EnemyImage.CenterOrigin();
            enemy.Add(enemy.EnemyImage);
            
            enemy.ShootTimer = Alarm.Create(Alarm.AlarmMode.Looping, enemy.Shoot, 0.1f, false);
            //enemy.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, () => { enemy.ShootTimer.Start(); }, 3.0f, true));
            enemy.Add(enemy.ShootTimer);
            enemy.Position = new Vector2(300.0f);
            enemy.ShootVec = Vector2.UnitX;
            enemy.Tag = Game1.EnemyTag;
            enemy.WalkAnimIndex = Calc.Random.Range(0, 40);

            return enemy;
        }

        private void Shoot()
        {
            ShootVec = ShootVec.Rotate(0.5f);
            Scene.Add(Bullet.CreateEnemyBullet(Position, ShootVec * 3.0f));
        }

        public void Hit(int damage, Vector2 direction)
        {
            HP -= damage;
            DeadSpeed = direction;
            if (HP < 0)
            {
                Dead();
            }
        }

        public void Dead()
        {
            if (IsDead) return;
            IsDead = true;
            if (SlimeType == 2)
            {
                //Purple Divide
                var vec = Calc.SafeNormalize(DeadSpeed.Perpendicular(), Vector2.UnitX);
                Scene1.Instance.AddEnemyAt(4, Position + vec * 20.0f);
                Scene1.Instance.AddEnemyAt(4, Position + vec * -20.0f);
                RemoveSelf();
            }
            if (SlimeType == 3)
            {
                //Rainbow
                Color[] rainbowColors = new Color[] { new Color(223,62,35),
                                                      new Color(250,106,10),
                                                      new Color(255,213,65),
                                                      new Color(89,193,53),
                                                      new Color(32,214,199),
                                                      new Color(40,92,196),
                                                      new Color(152,21,179),
                                                    };
                for (int i = 0; i < 7; i++)
                {
                    var bullet = Bullet.Create(Position, Calc.AngleToVector((float)Math.PI * 2.0f / 7.0f * i, 3.0f), false);
                    bullet.BulletImage.Texture = Game1.WhiteBulletImage;
                    bullet.BulletImage.Color = rainbowColors[i];
                    bullet.BulletImage.Texture = Game1.WhiteBulletImage;
                    bullet.BulletImage.Scale = new Vector2(0.6f);
                    bullet.Tag = Game1.EnemyBulletTag;
                    Scene.Add(bullet);
                }
                RemoveSelf();

                if (Player.Instance.BombNum < 3)
                {
                    var itemImage = new Image(Game1.BombItemImage);
                    itemImage.CenterOrigin();
                    var itemEntity = new Entity() { itemImage };
                    itemEntity.Tag = Game1.ItemTag;
                    itemEntity.Collider = new Circle(8.0f);
                    itemEntity.Position = Position;
                    Scene.Add(itemEntity);
                }
            }
            Scene1.OnEnemyDead();
        }

        public override void Update()
        {
            base.Update();

            Depth = (int)-Position.Y;

            if (IsDead)
            {
                Position += DeadSpeed;
                if (Game1.CheckOutsidePlayzone(ref Position, true) || CollideCheck(Game1.DeadBulletTag))
                {
                    RemoveSelf();
                    var Speed = DeadSpeed;
                    int[] offset = new int[7] {-3, -2, -1, 0, 1, 2, 3};
                    for (var i = 0; i < 7; i++)
                    {
                        var pvec = Vector2.Normalize(Speed.Perpendicular());
                        var bullet = Bullet.Create(Position + Speed * 0.5f + offset[i] * 10.0f * JamSize * pvec, Speed + offset[i] * 2.0f * pvec);
                        bullet.BulletImage.Texture = Game1.WhiteBulletImage;
                        bullet.BulletImage.Color = JamColor;
                        float scale = 1.0f * JamSize;
                        int x = Math.Abs(offset[i]);
                        if (x > 0)
                        {
                            if (x == 1) scale *= 0.8f;
                            else if (x == 2) scale *= 0.7f;
                            else if (x == 3) scale *= 0.3f;
                        }
                        float colliderSize = scale * 0.5f - 0.05f;
                        bullet.BulletImage.Scale = new Vector2(scale);
                        bullet.Collider = new Circle(bullet.BulletImage.Width * colliderSize);
                        bullet.Tag = Game1.LittleBulletTag;
                        Scene.Add(bullet);
                    }
                }
                return;
            }
            //EnemyImage.Scale.Y = ((float)Math.Sin(EnemyTime * 2.0f) + 1.0f) / 2.0f * 0.3f + 0.7f;
            EnemyTime += Engine.DeltaTime;
            /*
            if (CollideCheck(Game1.DeadBulletTag))
            {
                RemoveSelf();
                return;
            }
            */
            var bulletList = CollideAll(Game1.BulletTag);
            if (bulletList.Count > 0)
            {
                foreach (Bullet bullet in bulletList)
                {
                    Hit(bullet.Damage, bullet.Speed);
                    bullet.Damage = 0;
                    bullet.RemoveSelf();
                    if (HP < 0)
                    {
                        break;
                    }
                }
                if (HP < 0)
                {
                    Dead();
                    return;
                }
            }
            else
            {
                var move = Calc.SafeNormalize(Player.Instance.Position - Position, Vector2.UnitY) * 0.7f;
                Position += move;
                var collideList = CollideAll(Game1.EnemyTag);
                if (CollideCheck(Player.Instance))
                {
                    Player.Instance.Touch();
                    collideList.Add(Player.Instance);
                }
                if (collideList.Count > 0)
                {
                    Position -= move;
                }
                collideList = CollideAll(Game1.EnemyTag);
                if (CollideCheck(Player.Instance))
                {
                    collideList.Add(Player.Instance);
                }
                if (collideList.Count > 0)
                {
                    var awayVec = Position - collideList[0].Position;
                    awayVec = Calc.SafeNormalize(awayVec, Vector2.UnitX);
                    Position += awayVec * 10.0f;
                }
            }
            WalkAnimIndex++;
            WalkAnimIndex %= 40;
            EnemyImage.Texture = AnimTex[WalkAnimIndex / 5];
        }
    }
}
