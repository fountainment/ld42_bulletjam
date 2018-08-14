using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Monocle;

namespace LD42
{
    class Boss : Entity
    {
        public Image BossImage;
        private Alarm ShootBulletTimer;
        private Alarm StartShootBulletTimer;
        private bool StartShoot = false;
        private bool DashToPlayer = false;
        private List<Bullet> BulletDepo;
        private float FullHP = 24000;
        private float HP = 24000;
        private int Counter = 0;
        private Vector2 PlayerPosition;
        private int WalkAnimIndex = 0;

        public static Entity BossBloodEntity;
        public static Image BossBloodImage;
        public static Boss Instance;

        private Boss() : base() { }

        public static Boss Create()
        {
            var boss = new Boss();
            Instance = boss;
            boss.BossImage = new Image(Game1.BossImage);
            BossBloodImage = new Image(Game1.BossBloodImage);
            BossBloodEntity = new Entity();
            BossBloodImage.Position = new Vector2(25.0f, 10.0f);
            BossBloodEntity.Add(new Image(Game1.BossBloodBImage));
            BossBloodEntity.Add(BossBloodImage);
            BossBloodEntity.Add(new Image(Game1.BossBloodTImage));
            BossBloodEntity.Position = new Vector2(50.0f, 20.0f);
            Scene1.Instance.Add(BossBloodEntity);
            boss.Add(boss.BossImage);
            boss.BossImage.CenterOrigin();
            boss.Collider = new Circle(80.0f);
            boss.StartShootBulletTimer = Alarm.Create(Alarm.AlarmMode.Oneshot, boss.StartShootBullet, 3.0f, false);
            boss.ShootBulletTimer = Alarm.Create(Alarm.AlarmMode.Looping, boss.Shoot, 0.3f, true);
            boss.Add(boss.StartShootBulletTimer);
            boss.Add(boss.ShootBulletTimer);
            boss.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, boss.StartSuckBullet, 3.0f, true));
            boss.BulletDepo = new List<Bullet>();
            return boss;
        }

        public override void Update()
        {
            base.Update();

            WalkAnimIndex++;
            WalkAnimIndex %= 40;
            BossImage.Texture = Game1.BossWalkImageArray[WalkAnimIndex / 5];

            var bulletList = Scene.GetEntitiesByTagMask(Game1.SuckBulletTag);
            foreach (var bullet in bulletList)
            {
                var vec = Position - bullet.Position;
                float lenSq = vec.LengthSquared();
                float scale = Calc.Random.NextFloat() * 6000.0f / lenSq;
                if (scale > 1.0f)
                {
                    bullet.RemoveSelf();
                    BulletDepo.Add((Bullet)bullet);
                }
                else
                {
                    bullet.Position += scale * vec;
                }
            }

            bulletList = CollideAll(Game1.BulletTag);
            if (bulletList.Count > 0)
            {
                foreach (Bullet bullet in bulletList)
                {
                    Hit(bullet.Damage);
                    bullet.Damage = 0;
                    bullet.RemoveSelf();
                    if (HP < 0)
                    {
                        break;
                    }
                }
            }
            if (HP < 0)
            {
                Dead();
                return;
            }

            if (DashToPlayer)
            {
                var deadBulletList = CollideAll(Game1.DeadBulletTag);
                foreach(Bullet bullet in deadBulletList)
                {
                    bullet.RemoveSelf();
                }
                var vec = PlayerPosition - Position;
                if (vec.LengthSquared() < 25.0f)
                {
                    DashToPlayer = false;
                    StartShoot = true;
                }
                Position += vec * 0.03f;
            }
        }

        public void Hit(float damage)
        {
            HP -= damage;
            BossBloodImage.Scale.X = MathHelper.Max(HP, 0.0f) / FullHP;
        }

        public void Dead()
        {
            BossBloodImage.Scale.X = 0.0f;
            RemoveSelf();
        }

        public void StartSuckBullet()
        {
            var bulletList = Scene.GetEntitiesByTagMask(Game1.DeadBulletTag);
            foreach (var bullet in bulletList)
            {
                bullet.Tag = Game1.SuckBulletTag;
            }
            StartShootBulletTimer.Start();
        }

        public void StartShootBullet()
        {
            StartShoot = true;
        }

        public void Shoot()
        {
            if (StartShoot)
            {
                Vector2 playerVec = Calc.SafeNormalize(Player.Instance.Position - Position, Vector2.UnitY);
                if (Scene1.Instance.CollideCheck(Position, Player.Instance.Position, Game1.DeadBulletTag.ID))
                {
                    StartShoot = false;
                    DashToPlayer = true;
                    PlayerPosition = Player.Instance.Position;
                }
                float[] angleOffset = new float[] { -2, -1, 0, 1, 2};
                Counter++;
                Counter %= 2;
                if (Counter % 2 == 0) angleOffset = new float[] { -1.5f, -0.5f, 0.5f, 1.5f };
                for (int i = 0; i < angleOffset.Length; i++)
                {
                    var playerVecCopy = playerVec.Rotate(angleOffset[i] * (float)Math.PI * 0.08f);
                    if (BulletDepo.Count > 0)
                    {
                        Bullet bullet = BulletDepo[0];
                        var addBullet = Bullet.CreateEnemyBullet(Position, playerVecCopy * 3.0f);
                        addBullet.BulletImage.Color = bullet.BulletImage.Color;
                        addBullet.BulletImage.Texture = bullet.BulletImage.Texture;
                        addBullet.Collider = new Circle(5.0f);
                        Scene.Add(addBullet);
                        BulletDepo.Remove(bullet);
                    }
                    else
                    {
                        var addBullet = Bullet.CreateEnemyBullet(Position, playerVecCopy * 3.0f);
                        addBullet.Collider = new Circle(5.0f);
                        Scene.Add(addBullet);
                    }
                }
            }
        }
    }
}
