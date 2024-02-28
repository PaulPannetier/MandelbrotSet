using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using SME.Physic;

namespace SME
{
    public class Animation
    {
        public string name { get; private set; }
        public int nbFrame { get; private set; }
        public int currentFrame { get; private set; } = 0;
        private int step { get; set; }
        private float timer { get; set; }
        public Texture2D texture { get; private set; }
        private Vector2 origin;
        public Rectangle source { get; private set; }
        public float FPS { get; private set; }
        private float tempsAnim { get; set; }//le temp entre deux changement d'image
        private float waitingTime { get; set; }//le temps d'attente à la première frame
        private bool stop = false;
        public bool Isfinish { get; private set; } = false;
        public int nbTour { get; set; } = 0;
        private int nbAnimationFaite = 0;
        private int nbImgParLigne;//mettre le max
        private int nbLigne { get; set; } = 1;
        public float duration
        {
            get => nbFrame * tempsAnim;
        }
        private List<TimeAnimationEvent> timeAnimationEvents;
        private List<FrameAnimationEvent> frameAnimationEvents;

        #region Builder
        public Animation(Animation a)
        {
            Builder(a.name, a.nbFrame, a.FPS, a.step, a.texture, a.source, a.nbTour, a.nbLigne, a.nbImgParLigne, a.waitingTime);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture)
        {
            Builder("untitled", nbFrame, FPS, texture.Width / nbFrame, texture, new Rectangle(0, 0, texture.Width / nbFrame, texture.Height), 0, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, int nbTour)
        {
            Builder("untitled", nbFrame, FPS, texture.Width / nbFrame, texture, new Rectangle(0, 0, texture.Width / nbFrame, texture.Height), nbTour, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, string name)
        {
            Builder(name, nbFrame, FPS, texture.Width / nbFrame, texture, new Rectangle(0, 0, texture.Width / nbFrame, texture.Height), 0, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, int nbTour, string name)
        {
            Builder(name, nbFrame, FPS, texture.Width / nbFrame, texture, new Rectangle(0, 0, texture.Width / nbFrame, texture.Height), nbTour, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source)
        {
            Builder("untitled", nbFrame, FPS, source.Width, texture, source, 0, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, string name)
        {
            Builder(name, nbFrame, FPS, source.Width, texture, source, 0, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas)
        {
            Builder("untitled", nbFrame, FPS, pas, texture, source, 0, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas, string name)
        {
            Builder(name, nbFrame, FPS, pas, texture, source, 0, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas, int nbTour)
        {
            Builder("untitled", nbFrame, FPS, pas, texture, source, nbTour, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas, int nbTour, string name)
        {
            Builder(name, nbFrame, FPS, pas, texture, source, nbTour, 1, nbFrame, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas, int nbTour, int nbLine, int nbImgPerLine)
        {
            Builder("untitled", nbFrame, FPS, pas, texture, source, nbTour, nbLine, nbImgPerLine, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas, int nbTour, int nbLine, int nbImgPerLine, float waitingTime)
        {
            Builder("untitled", nbFrame, FPS, pas, texture, source, nbTour, nbLine, nbImgPerLine, waitingTime);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas, int nbTour, int nbLine, int nbImgPerLine, string name)
        {
            Builder(name, nbFrame, FPS, pas, texture, source, nbTour, nbLine, nbImgPerLine, 0);
        }

        public Animation(int nbFrame, float FPS, Texture2D texture, Rectangle source, int pas, int nbTour, int nbLine, int nbImgPerLine, string name, float waitingTime)
        {
            Builder(name, nbFrame, FPS, pas, texture, source, nbTour, nbLine, nbImgPerLine, waitingTime);
        }

        private void Builder(string name, int nbFrame, float FPS, int step, Texture2D texture, Rectangle source, int nbTour, int nbLigne, int nbImgParLigne, float waitingTime)
        {
            this.name = name;
            this.nbFrame = nbFrame;
            this.FPS = FPS;
            this.tempsAnim = 1f / FPS;
            this.step = step;
            this.texture = texture;
            this.source = source;
            this.origin = new Vector2(source.X, source.Y);
            this.nbTour = nbTour;
            this.nbLigne = nbLigne;
            this.nbImgParLigne = nbImgParLigne;
            timer = 0f;
            currentFrame = 0; //on commence par la frame 0!
            this.waitingTime = waitingTime;
            timeAnimationEvents = new List<TimeAnimationEvent>();
            frameAnimationEvents = new List<FrameAnimationEvent>();
        }
        #endregion

        public Animation Clone() => new Animation(this);

        public void RefreshAnime(int nbTour)
        {
            this.nbTour = nbTour;
            nbAnimationFaite = 0;
            Isfinish = false;
        }

        public void StopAnimation()
        {
            this.stop = true;
        }

        public void ResumeAnimation()
        {
            this.stop = false;
        }

        public void RemoveAnimationEvents()
        {
            frameAnimationEvents.Clear();
            timeAnimationEvents.Clear();
        }
        public void RemoveFrameAnimationEvents()
        {
            frameAnimationEvents.Clear();
        }
        public void RemoveTimeAnimationEvents()
        {
            timeAnimationEvents.Clear();
        }
        public void AddAnimationEvent(AnimationEvent animationEvent)
        {
            if(animationEvent is FrameAnimationEvent)
            {
                frameAnimationEvents.Add((FrameAnimationEvent)animationEvent);
            }
            else if(animationEvent is TimeAnimationEvent)
            {
                timeAnimationEvents.Add((TimeAnimationEvent)animationEvent);
            }
        }

        public void Play()
        {
            if (nbFrame <= 1)
                return;
            if (!stop)
            {
                timer += Time.dt;
                foreach (TimeAnimationEvent e in timeAnimationEvents)
                {
                    e.Update();
                }
                timeAnimationEvents.RemoveAll(e => e.toRemove);

                if (waitingTime != 0f && currentFrame == 0)
                {
                    if (timer >= waitingTime + tempsAnim)
                    {
                        currentFrame = Math.Min(nbFrame - 1, currentFrame + 1);
                        timer -= tempsAnim + waitingTime;
                    }
                }
                else if (timer >= tempsAnim)
                {
                    timer -= tempsAnim;
                    currentFrame++;

                    foreach(FrameAnimationEvent e in frameAnimationEvents)
                    {
                        e.SetFrame(currentFrame);
                    }

                    if (currentFrame > nbFrame - 1)
                    {
                        currentFrame = 0;
                        if (nbTour != 0)
                        {
                            nbAnimationFaite++;
                            if (nbAnimationFaite >= nbTour)
                            {
                                Isfinish = true;
                                nbAnimationFaite = 0;
                            }
                        }
                    }
                    source = new Rectangle((int)origin.X + (currentFrame % nbImgParLigne) * step, (int)origin.Y + source.Height * ((int)(currentFrame / nbImgParLigne)), source.Width, source.Height); //avant de faire le scale!
                }
            }
        }

        public abstract class AnimationEvent
        {
            public Action callBack { get; protected set; }
            public bool toRemove = false;

            public AnimationEvent(Action callBack)
            {
                this.callBack = callBack;
            }
            public virtual void Reset() { }
        }
        public class FrameAnimationEvent : AnimationEvent
        {
            public int activationFrame { get; protected set; }
            public int numberOfUse { get; protected set; }

            public FrameAnimationEvent(Action callBack, in int activationFrame, in int numberOfUse = -1) : base(callBack)
            {
                this.activationFrame = activationFrame;
                this.numberOfUse = numberOfUse;
            }

            public void SetFrame(in int frame)
            {
                if(frame == activationFrame)
                {
                    callBack();
                }
            }
        }

        public class TimeAnimationEvent : AnimationEvent
        {
            public float waitingTime { get; protected set; }
            private float timer = 0f;
            private int numberOfUse;
            public TimeAnimationEvent(Action callBack, in float waitingTime, in int numberOfUse = 1) : base(callBack)
            {
                this.waitingTime = waitingTime;
                this.numberOfUse = numberOfUse;
            }

            public void Update()
            {
                timer += Time.dt;
                if(timer >= waitingTime)
                {
                    callBack();
                    timer -= waitingTime;
                    if(numberOfUse > 0)
                    {
                        numberOfUse--;
                        if (numberOfUse == 0)
                            toRemove = true;
                    }
                }
            }
        }
    }

    public class Sprite
    {
        private static ColliderComparer colliderComparer = new ColliderComparer();

        #region Attribute

        public string tag;
        public Vector2 position = Vector2.Zero, origin = Vector2.Zero, velocity = Vector2.Zero;
        private Vector2 sumForce = Vector2.Zero, acceleration = Vector2.Zero;
        public bool ToRemove = false, showColliders = false, showTriggerColliders = false;
        public List<Collider> lstColliders { get; protected set; }//offset des colliders a partir de position
        public List<Collider> lstTriggerCollider { get; protected set; }//les colliders de trigger
        public List<string> byPassLayerMask;//les layerMask qui n'affecte pas le sprite
        public Vector2 scale { get; private set; }
        public Animation currentAnimation { get; protected set; }
        public Dictionary<string, Animation> animation;
        public float angle { get; protected set; } = 0f;
        public Color color = Color.White;
        public SpriteEffects effect;
        public RigidBody rigidBody { get; protected set; } = null;
        public bool useRigidBody = false;
        //rayon entourant tout le sprite, utiliser pour opti le system de collision
        private Circle globalCircle;

        public float layerDepth = 0.35f; //0 => 1er plan, 1 => background
        /*
         Pour le layer Deep:
            - Pour l'interface graphique layerDepth € ]0f, 0.1f[
            - Pour les sprites layerDepth € ]0.2f, 0.5f[
            - Pour les particules layerDepth € ]0.6f, 0.8f[
            - Pour le background layerDepth € ]0.8f, 0.9f[ 
        
        Pour les Sprites: 0.35f par defaut
        */

        #endregion

        #region Builder

        public Sprite(Animation anim)
        {
            Builder(anim, Vector2.One, "");
        }

        public Sprite(Animation anim, string tag)
        {
            Builder(anim, Vector2.One, tag);
        }

        private void Builder(Animation anim, in Vector2 scale, string tag)
        {
            animation = new Dictionary<string, Animation>();
            lstColliders = new List<Collider>();
            lstTriggerCollider = new List<Collider>();
            AddAnimation(anim.name, anim);
            currentAnimation = anim;
            this.scale = scale;
            this.tag = tag;
            ToRemove = false;
            SceneManager.currentScene.lstSprite.Add(this);
        }

        #endregion

        public void AddRigidbody(RigidBody rigidBody)
        {
            if (this.rigidBody == null)
            {
                useRigidBody = true;
                byPassLayerMask = new List<string>();
            }
            this.rigidBody = rigidBody;
        }
        /// <summary>
        /// !Remove all the collider of the sprite, if you just want to desactivate the rigidbody, use ridgidbody.setActive(false) instead
        /// </summary>
        public void RemoveRigidBody()
        {
            rigidBody = null;
            useRigidBody = false;
            byPassLayerMask = null;
        }

        #region GestionCollider

        private void SetGlobalCollider()
        {
            float maxRadius = 0f;
            foreach (Collider c in lstColliders)
            {
                maxRadius = MathF.Max(maxRadius, c.ToInclusiveCircle().radius + c.defaultOffset.Length());
            }
            globalCircle = new Circle(this, position, Vector2.Zero, maxRadius);
        }

        public void AddCollider(Collider collider)
        {
            collider.sprite = this;
            if(!collider.isTrigger)
            {
                if(lstColliders.Count == 0)
                {
                    SceneManager.currentScene.lstSpriteCollider.Add(this);
                }
                lstColliders.Add(collider);
                SetGlobalCollider();
                SortCollider();
            }
            else
            {
                if(lstTriggerCollider.Count == 0)
                {
                    SceneManager.currentScene.lstSpriteTrigger.Add(this);
                }
                lstTriggerCollider.Add(collider);
                SortTriggerCollider();
            }
        }

        public void RemoveCollider(Collider collider)
        {
            if(lstColliders.Remove(collider))
            {
                SetGlobalCollider();
                SortCollider();
                SortTriggerCollider(); 
            }
        }

        public void RemoveCollider(List<Collider> colliders)
        {
            bool setGlobalCol = false;
            foreach (Collider c in colliders)
            {
                if (lstColliders.Remove(c))
                {
                    setGlobalCol = true;
                }
            }
            if(setGlobalCol)
            {
                SetGlobalCollider();
                SortCollider();
                SortTriggerCollider();
            }
        }

        public List<T> GetColliders<T>() where T : Collider
        {
            List<T> col = new List<T>();
            foreach (Collider c in lstColliders)
            {
                if(c is T)
                {
                    col.Add((T)c);
                }
            }
            return col;
        }

        public void AddTriggerCollider(Collider collider)
        {
            collider.sprite = this;
            if(lstTriggerCollider.Count == 0)
            {
                SceneManager.currentScene.lstSpriteTrigger.Add(this);
            }
            lstTriggerCollider.Add(collider);
            collider.oldSpriteCollider = new List<Collider>();
            collider.newSpriteCollider = new List<Collider>();
            SortTriggerCollider();
        }

        /// <summary>
        /// Change the collider in param to an isTrigger collider
        /// </summary>
        public void ChangeToTriggerCollider(Collider c)
        {
            RemoveCollider(c);
            AddTriggerCollider(c);
        }

        public void SetCollidersPositions()
        {
            foreach (Collider c in lstColliders)
            {
                c.MoveAt(position);
            }
            foreach (Collider c in lstTriggerCollider)
            {
                c.MoveAt(position);
            }
            if(globalCircle != null)
            {
                globalCircle.MoveAt(position);    
            }
        }

        private void UpdateTriggerColliders()
        {
            foreach (Collider trigCol in lstTriggerCollider)
            {
                if (trigCol.isTrigger)
                {
                    trigCol.newSpriteCollider.Clear();
                    foreach (Sprite sprite in SceneManager.currentScene.lstSpriteCollider)
                    {
                        if (sprite.globalCircle != null && !sprite.globalCircle.Collide(trigCol.ToInclusiveCircle()))
                        {
                            continue;
                        }
                        foreach (Collider collider in sprite.lstColliders)
                        {
                            if (!collider.isTrigger && trigCol.Collide(collider))
                            {
                                trigCol.newSpriteCollider.Add(collider);
                                if (trigCol.OnTriggerEnter != null)
                                {
                                    if (!trigCol.oldSpriteCollider.Contains(collider))
                                    {
                                        trigCol.OnTriggerEnter(sprite, collider);
                                    }
                                }
                                if (trigCol.OnTriggerStay != null)
                                {
                                    trigCol.OnTriggerStay(sprite, collider);
                                }
                            }
                        }
                    }

                    if (trigCol.OnTriggerExit != null)
                    {
                        foreach (Collider collider in trigCol.oldSpriteCollider)
                        {
                            if (!trigCol.newSpriteCollider.Contains(collider))
                            {
                                trigCol.OnTriggerExit(collider.sprite, collider);
                            }
                        }
                    }
                    trigCol.oldSpriteCollider = new List<Collider>(trigCol.newSpriteCollider);
                }
            }
        }

        public bool Contains(in Vector2 p)
        {
            foreach (Collider c in lstColliders)
            {
                if (c.Contains(p))
                    return true;
            }
            return false;
        }

        private void SortCollider()
        {
            lstColliders.Sort(colliderComparer);
        }

        private void SortTriggerCollider()
        {
            lstTriggerCollider.Sort(colliderComparer);
        }

        #endregion

        #region Collide

        public bool Collide(Sprite sprite, out Collider selfCollider, out Collider spriteCollider, out Vector2 collisionPoint)
        {
            if (globalCircle != null && sprite.globalCircle != null && !globalCircle.Collide(sprite.globalCircle))
            {
                collisionPoint = Vector2.Zero;
                selfCollider = spriteCollider = null;
                return false;
            }




            collisionPoint = Vector2.Zero;
            selfCollider = spriteCollider = null;
            return false;
        }
        public bool Collide(Sprite sprite, out Collider selfCollider, out Collider spriteCollider)
        {
            if(globalCircle != null && sprite.globalCircle != null && !globalCircle.Collide(sprite.globalCircle))
            {
                selfCollider = spriteCollider =  null;
                return false;
            }

            foreach (Collider c1 in lstColliders)
            {
                foreach (Collider c2 in sprite.lstColliders)
                {
                    if (c1.Collide(c2))
                    {
                        selfCollider = c1;
                        spriteCollider = c2;
                        return true;
                    }
                }
            }
            selfCollider = spriteCollider = null;
            return false;
        }
        /// <returns>true if the sprite is in collision and have the mask specified in arg, false overwise</returns>
        public bool Collide(Sprite sprite, out Collider selfCollider, out Collider spriteCollider, string layerMask)
        {
            if (!globalCircle.Collide(sprite.globalCircle))
            {
                selfCollider = spriteCollider = null;
                return false;
            }

            foreach (Collider c1 in lstColliders)
            {
                foreach (Collider c2 in sprite.lstColliders)
                {
                    if (c1.CompareMask(layerMask) && c1.Collide(c2))
                    {
                        selfCollider = c1;
                        spriteCollider = c2;
                        return true;
                    }
                }
            }
            selfCollider = spriteCollider = null;
            return false;
        }
        /// <returns>true if the sprite is in collision and have the mask specified in arg</returns>
        public bool Collide(Sprite sprite, out Collider selfCollider, out Collider spriteCollider, List<string> layerMasks)
        {
            if (!globalCircle.Collide(sprite.globalCircle))
            {
                selfCollider = spriteCollider = null;
                return false;
            }

            foreach (Collider c1 in lstColliders)
            {
                foreach (Collider c2 in sprite.lstColliders)
                {
                    if (c1.CompareMask(layerMasks) && c1.Collide(c2))
                    {
                        selfCollider = c1;
                        spriteCollider = c2;
                        return true;
                    }
                }
            }
            selfCollider = spriteCollider = null;
            return false;
        }
        public bool Collide(Collider c, out Collider selfCollider)
        {
            foreach (Collider col in lstColliders)
            {
                if(col.Collide(c))
                {
                    selfCollider = col;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(Collider c, out Collider selfCollider, string layerMask)
        {
            foreach (Collider col in lstColliders)
            {
                if (col.CompareMask(layerMask) && col.Collide(c))
                {
                    selfCollider = col;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(Collider c, out Collider selfCollider, List<string> layerMasks)
        {
            foreach (Collider col in lstColliders)
            {
                if (col.CompareMask(layerMasks) && col.Collide(c))
                {
                    selfCollider = col;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(List<Collider> c, out Collider selfCollider, out Collider listCollider)
        {
            foreach (Collider col in lstColliders)
            {
                foreach (Collider col2 in c)
                {
                    if (col.Collide(col2))
                    {
                        selfCollider = col;
                        listCollider = col2;
                        return true;
                    }
                }
            }
            selfCollider = listCollider = null;
            return false;
        }
        public bool Collide(List<Collider> c, out Collider selfCollider, out Collider listCollider, string layerMask)
        {
            foreach (Collider col in lstColliders)
            {
                foreach (Collider col2 in c)
                {
                    if (col.CompareMask(layerMask) && col.Collide(col2))
                    {
                        selfCollider = col;
                        listCollider = col2;
                        return true;
                    }
                }
            }
            selfCollider = listCollider = null;
            return false;
        }
        public bool Collide(List<Collider> c, out Collider selfCollider, out Collider listCollider, List<string> layerMasks)
        {
            foreach(Collider col in lstColliders)
            {
                foreach (Collider col2 in c)
                {
                    if (col.CompareMask(layerMasks) && col.Collide(col2))
                    {
                        selfCollider = col;
                        listCollider = col2;
                        return true;
                    }
                }
            }
            selfCollider = listCollider = null;
            return false;
        }
        public bool Collide(Line l, out Collider selfCollider)
        {
            foreach(Collider c in lstColliders)
            {
                if(c.CollideLine(l))
                {
                    selfCollider = c;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(Line l, out Collider selfCollider, string layerMask)
        {
            foreach (Collider c in lstColliders)
            {
                if (c.CompareMask(layerMask) && c.CollideLine(l))
                {
                    selfCollider = c;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(Line l, out Collider selfCollider, List<string> layerMasks)
        {
            foreach (Collider c in lstColliders)
            {
                if (c.CompareMask(layerMasks) && c.CollideLine(l))
                {
                    selfCollider = c;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(Droite d, out Collider selfCollider)
        {
            foreach (Collider c in lstColliders)
            {
                if (c.CollideDroite(d))
                {
                    selfCollider = c;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(Droite d, out Collider selfCollider, string layerMask)
        {
            foreach (Collider c in lstColliders)
            {
                if (c.CompareMask(layerMask) && c.CollideDroite(d))
                {
                    selfCollider = c;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }
        public bool Collide(Droite d, out Collider selfCollider, List<string> layerMasks)
        {
            foreach (Collider c in lstColliders)
            {
                if (c.CompareMask(layerMasks) && c.CollideDroite(d))
                {
                    selfCollider = c;
                    return true;
                }
            }
            selfCollider = null;
            return false;
        }

        /// <returns> true if this sprite is on collision whith an other sprite collider, false otherwise</returns>
        private bool CollideWithSpriteCollider(out Collider self, out Collider other)
        {
            foreach(Sprite s in SceneManager.currentScene.lstSpriteCollider)
            {
                if(s != this && Collide(s, out Collider self2, out Collider other2))
                {
                    self = self2;
                    other = other2;
                    return true;
                }
            }
            self = other = null;
            return false;
        }

        #endregion

        public bool CompareTag(string tag) => this.tag == tag;

        public virtual void SetScale(in Vector2 newScale, in bool setColliderScaleToo = true)
        {
            if(setColliderScaleToo)
            {
                foreach (Collider c in lstColliders)
                {
                    c.SetScale(newScale, scale);
                }
                if(CollideWithSpriteCollider(out Collider self, out Collider other))
                {
                    foreach (Collider c in lstColliders)
                    {
                        c.SetScale(scale, newScale);
                    }
                }


                SetGlobalCollider();
            }
            scale = newScale;
        }

        public void SetScale(in float x, in float y, in bool setColliderScaleToo = true) { SetScale(new Vector2(x, y), setColliderScaleToo); }

        #region Animation

        public void AddAnimation(string name, Animation anim)
        {
            animation.Add(name, anim);
        }
        public void AddAnimation(Animation anim)
        {
            animation.Add(anim.name, anim);
        }
        public void RemoveAnimation(string nom)
        {
            animation.Remove(nom);
        }

        public void ChangeAnimation(string nomAnim, int nbTour)
        {
            if (currentAnimation.name != nomAnim && animation.ContainsKey(nomAnim))
            {
                ChangeAnimation(animation[nomAnim], nbTour);
            }
        }

        protected virtual void ChangeAnimation(Animation animation, int nbTour)
        {
            currentAnimation = animation;
            currentAnimation.RefreshAnime(nbTour);
        }
        public void StopAnimation()
        {
            currentAnimation.StopAnimation();
        }
        public void ResumeAnimation()
        {
            currentAnimation.ResumeAnimation();
        }

        #endregion

        #region Move/Rotate/Speed

        public virtual void Move(in float x, in float y) { Move(new Vector2(x, y)); }
        public virtual void Move(in Vector2 shift)
        {
            position = new Vector2(position.X + shift.X, position.Y);
            SetCollidersPositions();
            Collider self, other;
            if (lstColliders.Count > 0 && CollideWithSpriteCollider(out self, out other))
            {
                //appliqué la force normal!
                //nécessite le point de collision
                if(other.material != null)
                {
                    velocity = new Vector2(-other.material.bounceness * velocity.X, velocity.Y);
                    ApplyForce(-other.material.friction * velocity);
                }
                else
                {
                    velocity = new Vector2(0f, velocity.Y);
                }
                //on se positionne à la frontière du collider other
                float tempsShiftX = shift.X;
                Vector2 newPos = position;
                int nbShift = 0;
                do
                {
                    newPos = new Vector2(newPos.X - (tempsShiftX * 0.25f), newPos.Y);
                    tempsShiftX *= 0.75f;
                    MoveAt(newPos);
                    nbShift++;
                    if (nbShift >= 7)
                    {
                        do
                        {
                            newPos = new Vector2(newPos.X - MathF.Sign(tempsShiftX) * 0.1f, newPos.Y);
                            MoveAt(newPos);
                        } while (self.Collide(other));
                        break;
                    }
                } while (self.Collide(other));
            }
            position = new Vector2(position.X, position.Y + shift.Y);
            SetCollidersPositions();
            if (lstColliders.Count > 0 && CollideWithSpriteCollider(out self, out other))
            {
                if (other.material != null)
                {
                    velocity = new Vector2(velocity.X, -other.material.bounceness * velocity.Y);
                    ApplyForce((-other.material.friction) * velocity);
                }
                else
                {
                    velocity = new Vector2(velocity.X, 0f);
                }
                float tempsShiftY = shift.Y;
                Vector2 newPos = position;
                int nbShift = 0;
                do
                {
                    newPos = new Vector2(newPos.X, newPos.Y - (tempsShiftY * 0.25f));
                    tempsShiftY *= 0.75f;
                    MoveAt(newPos);
                    nbShift++;
                    if(nbShift >= 7)
                    {
                        do
                        {
                            newPos = new Vector2(newPos.X, newPos.Y - MathF.Sign(tempsShiftY) * 0.1f);
                            MoveAt(newPos);
                        } while (self.Collide(other));
                        break;
                    }
                } while (self.Collide(other));
            }
        }

        public virtual void MoveAt(in float pX, in float pY)
        {
            position = new Vector2(pX, pY);
            SetCollidersPositions();
        }
        public virtual void MoveAt(in Vector2 newPosition)
        {
            position = newPosition;
            SetCollidersPositions();
        }

        /// <summary>
        /// Change l'offset des colliders
        /// </summary>
        public void RotateAt(in float angle)
        {
            Rotate(angle - this.angle);
        }
        /// <summary>
        /// Change l'offset des colliders
        /// </summary>        
        public void Rotate(in float angle)
        {
            this.angle += angle;
            foreach (Collider c in lstColliders)
            {
                c.Rotate(angle);
            }
            if(CollideWithSpriteCollider(out Collider self, out Collider other))
            {
                this.angle -= angle;
                foreach (Collider c in lstColliders)
                {
                    c.Rotate(-angle);
                }
            }
        }

        public virtual void SetSpeed(in float vx, in float vy)
        {
            velocity = new Vector2(vx, vy);
        }
        public virtual void SetSpeed(in Vector2 newSpeed)
        {
            velocity = newSpeed;
        }
        public virtual void SetSpeed(float angle, int absoluteSpeed)
        {
            velocity = new Vector2((float)(Math.Cos(angle) * absoluteSpeed), (float)(Math.Sin(angle) * absoluteSpeed));
        }

        public void ApplyForce(in float forceX, in float forceY) { ApplyForce(new Vector2(forceX, forceY)); }
        public void ApplyForce(in Vector2 force)
        {
            sumForce += force;
        }
        public void ApplyForce(in float angle, in int forteAbsolue)
        {
            ApplyForce(new Vector2(forteAbsolue * (float)Math.Cos(angle), forteAbsolue * (float)Math.Sin(angle)));
        }

        #endregion

        public virtual string GetCurrentDirection()
        {
            if (Math.Abs(velocity.X) >= Math.Abs(velocity.Y))
            {
                return velocity.X >= 0f ? "right" : "left";
            }
            else
            {
                return velocity.Y >= 0f ? "down" : "up";
            }
        }

        #region Update

        public virtual void Update()
        {
            effect = velocity.X >= 0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            if(useRigidBody)
            {
                if(rigidBody.isActive)
                {
                    ApplyForce(rigidBody.gravityDirection * (rigidBody.gravityScale * 9.81f * rigidBody.masse));
                }
                ApplyForce((-rigidBody.frictionCoefficient) * velocity);
                acceleration = sumForce / rigidBody.masse;
                velocity += acceleration;
                sumForce = Vector2.Zero;
            }
            Move(velocity * Time.dt);
            UpdateTriggerColliders();

            currentAnimation.Play();
        }

        #endregion

        #region Draw

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(currentAnimation.texture, Camera.mainCamera.ToScreenCoordinateSystem(position), currentAnimation.source, color, angle, origin, scale * Screen.scaleFactor, effect, layerDepth);
            spriteBatch.Draw(currentAnimation.texture, Camera.mainCamera.ToScreenCoordinateSystem(position), currentAnimation.source, color, angle, origin, scale, effect, layerDepth);

            if (showColliders)
            {
                foreach (Collider c in lstColliders)
                {
                    c.Draw(spriteBatch);
                }
                //globalCircle.Draw(spriteBatch);
            }
            if (showTriggerColliders)
            {
                foreach (Collider c in lstTriggerCollider)
                {
                    c.Draw(spriteBatch);
                }
            }
        }

        #endregion
    }
}
