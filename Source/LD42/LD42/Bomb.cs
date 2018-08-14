using Monocle;
using Microsoft.Xna.Framework;

namespace LD42
{
    class Bomb : Entity
    {
        private Image BombImage;
        private float TimeToExplode = 3.0f;
        private float ExplodeRadius = 100.0f;
        private int BlinkPeriod = 8;
        private int BlinkIndex = 0;

        public static Bomb CreateAt(Vector2 position)
        {
            var bomb = new Bomb();
            bomb.Position = position;
            bomb.BombImage = new Image(Game1.BombImage);
            bomb.BombImage.CenterOrigin();
            bomb.Add(bomb.BombImage);
            bomb.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, () => { bomb.Explode(); }, bomb.TimeToExplode, true));
            bomb.Add(Alarm.Create(Alarm.AlarmMode.Oneshot, () => { bomb.BlinkIndex = 0; bomb.BlinkPeriod = 5; }, bomb.TimeToExplode * 0.7f, true));
            bomb.Collider = new Circle(bomb.ExplodeRadius);
            return bomb;
        }

        private void Explode()
        {
            RemoveSelf();
            var bangImage = new Image(Game1.BombBangImageArray[0]);
            var bangIndex = 0;
            bangImage.CenterOrigin();
            var bangEntity = new Entity();
            bangEntity.Position = Position;
            bangEntity.Add(bangImage);
            bangEntity.Add(Alarm.Create(Alarm.AlarmMode.Looping, ()=> { bangIndex++; if (bangIndex >= 10) { bangEntity.RemoveSelf(); return; } bangImage.Texture = Game1.BombBangImageArray[bangIndex]; }, 0.0833f, true));
            Scene1.Instance.Add(bangEntity);
            var deadBulletList = CollideAll(Game1.DeadBulletTag);
            foreach(Bullet bullet in deadBulletList)
            {
                bullet.RemoveSelf();
            }

            var enemyList = CollideAll(Game1.EnemyTag);
            foreach (Enemy enemy in enemyList)
            {
                enemy.Hit(1000, Vector2.Normalize(enemy.Position - Position) * 10.0f);
            }
            if (Boss.Instance != null)
            {
                if (CollideCheck(Boss.Instance))
                {
                    Boss.Instance.Hit(5000);
                }
            }
        }

        public override void Update()
        {
            base.Update();

            BlinkIndex++;
            if (BlinkIndex % (BlinkPeriod * 2) == 0)
            {
                BombImage.Texture = Game1.BombImage;
                BlinkIndex = 0;
            }
            else if (BlinkIndex % BlinkPeriod == 0)
            {
                BombImage.Texture = Game1.BombRedImage;
            }
        }
    }
}
