using Monocle;
using Microsoft.Xna.Framework;
using System;

namespace LD42
{
    class Cursor : Entity
    {
        private static Cursor instance;

        public static Cursor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Cursor.Create();
                }
                return instance;
            }
        }

        private Cursor() : base() {}

        private static Cursor Create()
        {
            var cursor = new Cursor();
            var image = new Image(Game1.CursorImage);
            image.CenterOrigin();
            cursor.Add(image);
            cursor.Depth = -999;
            return cursor;
        }

        public override void Update()
        {
            base.Update();
            Position = MInput.Mouse.Position;
        }
    }
}