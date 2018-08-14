using Monocle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace LD42
{
    class Player : Entity
    {
        private Alarm BulletAlarm;
        private bool Shootable = true;
        private Image PlayerImage;
        private Image PlayerFootImage;
        private int FootAnimIndex = 0;
        private int FrameIndex = 0;
        private int HP = 6;
        public int BombNum = 3;
        private bool POW = false;

        private static Player instance;

        public static Player Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = Player.Create();
                }
                return instance;
            }
        }

        private static Player Create()
        {
            Player player = new Player();
            player.BulletAlarm = Alarm.Create(Alarm.AlarmMode.Persist, ()=> { player.Shootable = true; }, 0.1f, false);
            player.Add(player.BulletAlarm);
            player.PlayerFootImage = new Image(Game1.PlayerFootImageArray[0]);
            player.PlayerFootImage.CenterOrigin();
            player.PlayerImage = new Image(Game1.PlayerImage);
            player.PlayerImage.CenterOrigin();
            player.Add(player.PlayerFootImage);
            player.Add(player.PlayerImage);
            player.Collider = new Circle(8.0f);
            player.Position = new Vector2(300.0f, 500.0f);
            return player;
        }

        public void Shoot()
        {
            if (Shootable)
            {
                Scene.Add(Bullet.Create(Position, 10.0f * Vector2.Normalize(Cursor.Instance.Position - Position)));
                Shootable = false;
                BulletAlarm.Start();
            }
        }

        public void Move(Vector2 move)
        {
            if (move == Vector2.Zero)
            {
                FootAnimIndex /= 4;
                FootAnimIndex *= 4;
            }
            if (move.X > 0)
            {
                PlayerFootImage.FlipX = false;
            }
            else if (move.X < 0)
            {
                PlayerFootImage.FlipX = true;
            }
            PlayerFootImage.Texture = Game1.PlayerFootImageArray[FootAnimIndex];
            if (FrameIndex == 5)
            {
                FootAnimIndex += 1;
                FrameIndex = 0;
            }
            FrameIndex++;
            if (FootAnimIndex >= 4)
            {
                FootAnimIndex = 0;
            }

            Position += move * 3.0f;
            if (CollideCheck(Game1.DeadBulletTag))
            {
                Position -= move * 3.0f;
            }
            if (Position.Y < 180.0f) Position.Y = 180.0f;
            if (Position.X < 80.0f) Position.X = 80.0f;
            if (Position.Y > 620.0f) Position.Y = 620.0f;
            if (Position.X > 520.0f) Position.X = 520.0f;
        }

        public override void Update()
        {
            base.Update();

            var vec = Vector2.Normalize(Cursor.Instance.Position - Position).EightWayNormal();
            int index = 1;
            if (vec == -Vector2.UnitY) index = 7;
            if (vec == Vector2.UnitY) index = 3;
            if (vec == -Vector2.UnitX) index = 5;
            if (vec == Vector2.UnitX) index = 1;
            if (vec.X == vec.Y && vec.X < 0) index = 6;
            if (vec.X == vec.Y && vec.X > 0) index = 2;
            if (vec.X == -vec.Y && vec.X < 0) index = 4;
            if (vec.X == -vec.Y && vec.X > 0) index = 8;
            index -= 1;
            PlayerImage.Texture = Game1.PlayerImageArray[index];

            var collideItem = CollideAll(Game1.ItemTag);
            if(collideItem.Count > 0)
            {
                foreach (var item in collideItem)
                {
                    if (BombNum < 3)
                    {
                        BombNum++;
                        item.RemoveSelf();
                    }
                    else
                    {
                        break;
                    }
                }
                Scene1.Instance.PlayerBombNumImage.Texture = Game1.BombNumImageArray[3 - BombNum];
            }

            var collideBullet = CollideAll(Game1.EnemyBulletTag);
            if (collideBullet.Count > 0)
            {
                foreach(Bullet bullet in collideBullet)
                {
                    if (!POW)
                    {
                        HP--;
                        if (HP < 0)
                        {
                            HP = 0;
                        }
                        Scene1.Instance.PlayerHPImage.Texture = Game1.PlayerHPImageArray[6 - HP];
                        if (HP != 0)
                        {
                            POW = true;
                            PlayerFootImage.Color = Color.Red;
                            PlayerFootImage.Color.A = 64;
                            PlayerImage.Color = Color.Red;
                            PlayerImage.Color.A = 64;
                            Add(Alarm.Create(Alarm.AlarmMode.Oneshot, () => { POW = false; PlayerFootImage.Color = Color.White; PlayerImage.Color = Color.White; }, 1.0f, true));
                        }
                    }
                    bullet.RemoveSelf();
                }
                if (HP <= 0)
                {
                    Dead();
                }
                return;
            }

            var move = new Vector2(0.0f);
            if (MInput.Keyboard.Check(Keys.W))
            {
                move.Y -= 1.0f;
            }
            if (MInput.Keyboard.Check(Keys.A))
            {
                move.X -= 1.0f;
            }
            if (MInput.Keyboard.Check(Keys.S))
            {
                move.Y += 1.0f;
            }
            if (MInput.Keyboard.Check(Keys.D))
            {
                move.X += 1.0f;
            }
            move.Normalize();
            if (float.IsNaN(move.Length()))
            {
                move = Vector2.Zero;
            }
            Player.Instance.Move(move);

            if (MInput.Mouse.CheckLeftButton)
            {
                Player.Instance.Shoot();
            }
            if (MInput.Mouse.PressedRightButton)
            {
                Player.Instance.PlaceBomb();
            }
            Depth = (int)-Position.Y;
        }

        private void PlaceBomb()
        {
            if (BombNum > 0)
            {
                BombNum--;
                Scene.Add(Bomb.CreateAt(Position));
                Scene1.Instance.PlayerBombNumImage.Texture = Game1.BombNumImageArray[3 - BombNum];
            }
        }

        internal void Touch()
        {
            if (!POW)
            {
                HP--;
                if (HP < 0)
                {
                    HP = 0;
                }
                Scene1.Instance.PlayerHPImage.Texture = Game1.PlayerHPImageArray[6 - HP];
                if (HP != 0)
                {
                    POW = true;
                    PlayerFootImage.Color = Color.Red;
                    PlayerImage.Color = Color.Red;
                    Add(Alarm.Create(Alarm.AlarmMode.Oneshot, () => { POW = false; PlayerFootImage.Color = Color.White; PlayerImage.Color = Color.White; }, 1.0f, true));
                }
                else
                {
                    Dead();
                }
            }
        }

        public void Dead()
        {
            RemoveSelf();
        }
    }
}
