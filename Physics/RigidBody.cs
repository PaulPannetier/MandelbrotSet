using Microsoft.Xna.Framework;

namespace SME.Physic
{
    public class Component
    {
        public bool isActive { get; protected set; }

        public Component()
        {
            isActive = true;
        }

        public virtual void SetActive(in bool active)
        {
            isActive = active;
        }

    }
    /// <summary>
    /// Enable gravity and collision for your sprite
    /// </summary>
    public class RigidBody : Component
    {
        public float masse, gravityScale, frictionCoefficient;
        public Vector2 gravityDirection = Vector2.UnitY;

        public RigidBody(in float masse, in float gravityScale, in float frictionCoefficient, in bool useGravity, in Vector2 gravityDirection) : base()
        {
            Builder(masse, gravityScale, frictionCoefficient, useGravity, gravityDirection);
        }

        public RigidBody(in float masse) : base()
        {
            Builder(masse, 1f, 0.5f, true, Vector2.UnitY);
        }

        private void Builder(in float masse, in float gravityScale, in float frictionCoefficient, in bool useGravity, in Vector2 gravityDirection)
        {
            this.masse = masse;
            this.gravityScale = gravityScale;
            this.frictionCoefficient = frictionCoefficient;
            this.gravityDirection = gravityDirection;
        }
    }
}
