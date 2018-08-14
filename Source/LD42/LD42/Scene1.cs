using Monocle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace LD42
{
    class Scene1 : Scene
    {
        private Alarm EnemyTimer;
        public static Scene1 Instance;
        public static int AliveEnemyCounter = 0;

        public Image PlayerHPImage;
        public Image PlayerBombNumImage;

        public override void Begin()
        {
            Instance = this;
            base.Begin();

            Renderer renderer = new EverythingRenderer();
            var wave = 1;
            EnemyTimer = Alarm.Create(Alarm.AlarmMode.Persist, () => { GenerateEnemy(wave); wave++; }, 3.0f, false);
            var colorTimer = Alarm.Create(Alarm.AlarmMode.Looping, null, 60, true);
            colorTimer.OnComplete = () => {
                var bulletList = GetEntitiesByTagMask(Game1.DeadBulletTag);
                foreach (Bullet bullet in bulletList)
                {
                    var oldColor = bullet.BulletImage.Color.ToVector3();
                    if (MathHelper.Max(oldColor.X, MathHelper.Max(oldColor.Y, oldColor.Z)) > 0.5f)
                    {
                        var newColor = oldColor * 0.8f;
                        var progress = 0.0f;
                        var bulletColorTimer = Alarm.Create(Alarm.AlarmMode.Looping, null, 0.01f, true);
                        bulletColorTimer.OnComplete = () => {
                            if (progress >= 1.0f)
                            {
                                bulletColorTimer.RemoveSelf();
                            }
                            bullet.BulletImage.Color = new Color(Vector3.Lerp(oldColor, newColor, progress));
                            progress += 0.01f;
                        };
                        bullet.Add(bulletColorTimer);
                    }
                }
            };
            Add(new Entity() { colorTimer });

            PlayerHPImage = new Image(Game1.PlayerHPImageArray[0]);
            PlayerHPImage.Position = new Vector2(50.0f, 80.0f);
            PlayerBombNumImage = new Image(Game1.BombNumImageArray[0]);
            PlayerBombNumImage.Position = new Vector2(400.0f, 80.0f);

            Add(renderer);
            Add(new Entity() { new Image(Game1.BGImage) });
            Add(new Entity() { PlayerHPImage });
            Add(new Entity() { PlayerBombNumImage });
            Add(Player.Instance);
            Add(Cursor.Instance);
            Player.Instance.Add(EnemyTimer);

            GenerateEnemy();
        }

        public Vector2 GetValidSpawnPosition()
        {
            var angle = Calc.Random.NextAngle();
            var randVec = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            var position = LineWalkCheck(Player.Instance.Position, Player.Instance.Position + randVec * 500.0f, Game1.DeadBulletTag.ID, 5.0f);
            Game1.CheckOutsidePlayzone(ref position, true);
            position = position + (Player.Instance.Position - position) * 0.1f;
            return position;
        }

        public void AddEnemy(int[] typeArray)
        {
            foreach (int i in typeArray)
            {
                AddEnemy(i);
            }
        }

        public void AddEnemyAt(int type, Vector2 position)
        {
            var enemy = Enemy.Create(type);
            enemy.Position = position;
            Add(enemy);
            AliveEnemyCounter++;
        }

        public void AddEnemy(int type)
        {
            var position = GetValidSpawnPosition();
            AddEnemyAt(type, position);
        }

        public void AddBoss()
        {
            var boss = Boss.Create();
            boss.Position = GetValidSpawnPosition();
            Add(boss);
        }

        public void GenerateEnemy(int wave = 0)
        {
            int[] enemyArray = new int[] { };
            switch (wave)
            {
                case 0:
                    enemyArray = new int[] { 0, 0, 0 };
                    break;
                case 1:
                    enemyArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case 2:
                    enemyArray = new int[] { 1, 1, 1, 1, 1, 1, 1 };
                    break;
                case 3:
                    enemyArray = new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1 };
                    break;
                case 4:
                    enemyArray = new int[] { 0, 0, 0, 1, 1, 2, 2, 2, 3 };
                    break;
                case 5:
                    enemyArray = new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 3 };
                    break;
                case 6:
                    enemyArray = new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3 };
                    break;
                case 7:
                    AddBoss();
                    break;
            }
            AddEnemy(enemyArray);
        }

        public static void OnEnemyDead()
        {
            AliveEnemyCounter--;
            if (AliveEnemyCounter == 0 && !Instance.EnemyTimer.Active)
            {
                Instance.EnemyTimer.Start();
            }
        }

        public override void Update()
        {
            base.Update();

            if (MInput.Keyboard.Check(Keys.F))
            {
                var bulletList = GetEntitiesByTagMask(Game1.DeadBulletTag);
                foreach(Bullet bullet in bulletList)
                {
                    bullet.Speed = Calc.Random.ShakeVector();
                }
            }
        }
    }
}
