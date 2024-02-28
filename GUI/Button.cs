using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SME
{
    public delegate void OnClick(Button pSender);

    public class Button : Sprite
    {
        private bool IsHover;// la souris est dessus
        public OnClick onClick { get; set; } // une reference de fonction
        private string soundHoverID = null;
        private string soundClickID = null;
        private float timerSoundHover = 0f;
        private const float timeBetweenSoundHover = 0.5f;

        public Button(Animation anim, string pType, string sndHoverID = null, string sndClickID = null, Animation animHover = null) : base(anim)
        {
            if (animHover != null)
                AddAnimation("hover", animHover);
            if (sndHoverID != null)
                soundHoverID = sndHoverID;
            if (sndClickID != null)
                soundClickID = sndClickID;

        }

        public override void Update()
        {
            timerSoundHover += Time.dt;
            /*
            if (hitBox.Contains(G.newMouseState.Position))
            {
                if (!IsHover)
                {
                    IsHover = true;
                    ChangeAnimation("hover", 0);
                    if (timerSoundHover > timeBetweenSoundHover)
                    {
                        timerSoundHover = 0f;
                        if (soundHoverID != null)
                            AssetsManager.PlaySoundEffect(soundHoverID);
                    }
                }
            }
            else
            {
                if (IsHover)
                {
                    IsHover = false;
                    ChangeAnimation("idle", 0);
                }
            }
            */
            if (IsHover)
            {
                /*
                if (G.newMouseState.LeftButton == ButtonState.Pressed && G.oldMouseState.LeftButton == ButtonState.Released)
                {
                    if (soundClickID != null)
                        AssetsManager.PlaySoundEffect(soundClickID);
                    onClick?.Invoke(this);
                }
                */
            }

            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            base.Draw(spriteBatch);
        }
    }
}
