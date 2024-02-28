using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace SME
{
    public class Camera
    {
        public static Camera mainCamera;

        public Vector2 _position;
        public Vector2 position
        {
            get => _position;
            set
            {
                _position = value;
                topLeft = new Vector2(position.X - (bound.Width / 2f), position.Y - (bound.Height / 2f));
            }
        }
        private Vector2 topLeft;
        public Vector2 offset, scale = Vector2.One;
        public Sprite target = null;
        public Color backGroundColor = Color.CornflowerBlue;
        public float delay;
        private float timerDelay = 0f, timerShake = 0f;
        private Rectangle bound;
        private Queue<TimedVector2> targetPositions;
        //les vibrations
        public float shakeIntensity;
        private float shakeDuration;
        private bool isShaking = false;
        private Vector2 oldPositionShake;

        public Camera()
        {
            bound = Screen.screenBound;
            targetPositions = new Queue<TimedVector2>();
        }

        public Vector2 ToScreenCoordinateSystem(in Vector2 position) => position - topLeft;
        public Vector2 ToWorldCoordinateSystem(in Vector2 position) => position + topLeft;

        public void SetTarget(Sprite target, in Vector2 offset, in float delay = 0f)
        {
            this.offset = offset;
            this.target = target;
            this.delay = delay;
            position = target.position + offset;
        }
        public void SetTarget(Sprite target, in float delay = 0f)
        {
            SetTarget(target, Vector2.Zero, delay);
        }

        public void Move(in Vector2 shift)
        {
            position += shift;
            if(isShaking)
            {
                oldPositionShake = position;
            }
        }
        public void MoveAt(in Vector2 newPosition)
        {
            position = newPosition;
        }
        public void Shake(in float shakeIntensity, in float duration)
        {
            this.shakeIntensity = shakeIntensity;
            shakeDuration = duration;
            isShaking = true;
        }

        public void Update()
        {
            if(target != null)
            {
                timerDelay += Time.dt;
                targetPositions.Enqueue(new TimedVector2(target.position, timerDelay));
                while(targetPositions.Count > 0 && timerDelay - targetPositions.Peek().time >= delay)
                {
                    TimedVector2 temp = targetPositions.Dequeue();
                    this.position = temp.pos + offset;
                }
            }
            if (isShaking)
            {
                timerShake += Time.dt;
                position += Random.RandomVector2(Random.Rand(0f, shakeIntensity));
                if (timerShake >= shakeDuration)
                {
                    isShaking = false;
                    if(target == null)
                    {
                        MoveAt(oldPositionShake);
                    }
                }
            }
            
        }

        private class TimedVector2
        {
            public Vector2 pos;
            public float time;
            public TimedVector2(in Vector2 pos, in float time)
            {
                this.pos = pos; this.time = time;
            }
        }
    }
}
