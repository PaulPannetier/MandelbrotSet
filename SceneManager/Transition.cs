using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace SME
{
    /// <summary>
    /// Use this class to make transition between Scene, can also be use to make loading screen
    /// </summary>
    abstract public class Transition : Scene
    {
        protected Scene newScene, currentScene;

        public Transition(Scene newScene) : base()
        {
            this.newScene = newScene;
        }

        protected void LoadNewScene()
        {
            currentScene.UnLoad();
            newScene.Load();
        }

        public virtual void Start()
        {
            Load();
            currentScene = SceneManager.currentScene;
        }

        public override void Load()
        {
            lstSpriteCollider = new List<Sprite>();
            lstSpriteTrigger = new List<Sprite>();
            lstSprite = new List<Sprite>();
        }

        protected void RunNewScene()
        {
            SceneManager.LoadScene(newScene, false);
        }

        public override void Update()
        {
            currentScene.Update();
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            currentScene.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }
    }
    /// <summary>
    /// Make a fade transition between two scene
    /// </summary>
    public class Fade : Transition
    {
        protected ColorTweener colorTweenerIn, colorTweenerOut;
        protected Rectangle screen;
        protected bool isAveragePass = false;
        protected Scene sceneToDraw;

        public Fade(Scene newScene, in Color color, in float duration) : base(newScene)
        {
            colorTweenerIn = new ColorTweener(Color.White * 0f, color, TweeningFunction.Linear, duration / 2f);
            colorTweenerOut = new ColorTweener(color, Color.White * 0f, TweeningFunction.Linear, duration / 2f);
            screen = Screen.screenBound;
            sceneToDraw = null;
        }
        public Fade(Scene newScene, in Color color, in float duration, function tweeningFunction) : base(newScene)
        {
            colorTweenerIn = new ColorTweener(Color.White * 0f, color, tweeningFunction, duration / 2f);
            colorTweenerOut = new ColorTweener(color, Color.White * 0f, tweeningFunction, duration / 2f);
            screen = new Rectangle(0, 0, Screen.width, Screen.height);
            sceneToDraw = null;
        }

        public override void Start()
        {
            base.Start();
            sceneToDraw = currentScene;
        }

        public override void Update()
        {
            if(!isAveragePass)
            {
                if(colorTweenerIn.isFinish)
                {
                    //on affiche la nouvelle scene
                    LoadNewScene();
                    sceneToDraw = newScene;
                    isAveragePass = true;
                }
                colorTweenerIn.Update();
            }
            else
            {
                if(colorTweenerOut.isFinish)
                {
                    RunNewScene();
                }
                colorTweenerOut.Update();
            }
            sceneToDraw.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            sceneToDraw.Draw(spriteBatch);
            if (isAveragePass)
            {
                spriteBatch.DrawRectangleFill(screen, colorTweenerOut.GetColor, 0f);
            }
            else
            {
                spriteBatch.DrawRectangleFill(screen, colorTweenerIn.GetColor, 0f);
            }
        }
    }

    public class CircleGrowth : Transition
    {
        private List<Circle> circles;
        private float duration, maxRadius;
        private int minCircles, maxCircles;
        private Color color;
        private bool isAveragePass = false;
        private List<Tweener> tweenerIn, tweenerOut;
        private function tweeningFunction;
        private Scene sceneToDraw;

        public CircleGrowth(Scene newScene, in float duration, in Color color, in int minCircles, in int maxCircles, in float maxRadius) : base(newScene)
        {
            Builder(duration, color, minCircles, maxCircles, maxRadius, TweeningFunction.Linear);
        }
        public CircleGrowth(Scene newScene, in float duration, in Color color, in int minCircles, in int maxCircles, in float maxRadius, function tweeningFunction) : base(newScene)
        {
            Builder(duration, color, minCircles, maxCircles, maxRadius, tweeningFunction);
        }

        private void Builder(in float duration, in Color color, in int minCircles, in int maxCircles, in float maxRadius, function tweeningFunction)
        {
            this.duration = duration;
            this.color = color;
            this.maxCircles = maxCircles;
            this.minCircles = minCircles;
            this.maxRadius = maxRadius;
            this.tweeningFunction = tweeningFunction;
        }

        public override void Start()
        {
            base.Start();
            sceneToDraw = currentScene;
            tweenerIn = new List<Tweener>();
            tweenerOut = new List<Tweener>();
            circles = new List<Circle>();
            int nbCircles = Random.Rand(minCircles, maxCircles);
            for (int i = 0; i < nbCircles; i++)
            {
                Vector2 pos = new Vector2(Random.Rand(-0.1f * Screen.width, Screen.width), Random.Rand(-0.1f * Screen.height, Screen.height));
                Circle c = new Circle(pos, Random.Rand(1f, maxRadius / 4f));
                c.color = color;
                circles.Add(c);
                tweenerIn.Add(new Tweener(c.radius, maxRadius, duration / 2f, tweeningFunction));
                tweenerOut.Add(new Tweener(maxRadius, 0f, duration / 2f, tweeningFunction));
            }
        }

        public override void Update()
        {
            base.Update();
            if(isAveragePass)
            {
                for (int i = 0; i < circles.Count; i++)
                {
                    Circle c = circles[i];
                    c.radius = tweenerOut[i].GetValue;
                    c.thickness = c.radius;
                    c.sidesNumber = (int)(c.radius * 2f);
                    tweenerOut[i].Update();
                }
                if (tweenerOut[0].isFinish)
                {
                    RunNewScene();
                }
            }
            else
            {
                for (int i = 0; i < circles.Count; i++)
                {
                    Circle c = circles[i];
                    c.radius = tweenerIn[i].GetValue;
                    c.thickness = c.radius;
                    c.sidesNumber = (int)(c.radius * 2f);
                    tweenerIn[i].Update();
                }
                if (tweenerIn[0].isFinish)
                {
                    LoadNewScene();
                    isAveragePass = true;
                    sceneToDraw = newScene;
                }
            }
            sceneToDraw.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            sceneToDraw.Draw(spriteBatch);
            foreach (Circle c in circles)
            {
                c.Draw(spriteBatch);
            }
        }
    }
}
