using Monocle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HelloMonocle
{
    class MyScene : Scene
    {
        public MyScene() : base()
        {
        }
    }

    static class HelloMonocle
    {
        static void Main(string[] args)
        {
            Engine.Scene = new Scene();
            Engine game = new Engine(1024, 768, 1024, 768, "HelloGame", false);
            game.RunWithLogging();
        }
    }
}
