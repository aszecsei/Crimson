using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class PostProcessEffect
    {
        protected RenderTarget2D? RenderTarget2D;

        protected RenderTarget2D? Was;

        public PostProcessEffect(Effect effect)
        {
            Effect = effect;
        }

        public Effect Effect { get; }

        ~PostProcessEffect()
        {
            RenderTarget2D?.Dispose();
        }

        public virtual void BeforeRender()
        {
            if (RenderTarget2D != null &&
                (RenderTarget2D.Width != Engine.ViewWidth || RenderTarget2D.Height != Engine.ViewHeight))
            {
                RenderTarget2D.Dispose();
                RenderTarget2D = null;
            }

            if (RenderTarget2D == null)
                RenderTarget2D = Utils.CreateRenderTarget(Engine.ViewWidth, Engine.ViewHeight);

            Was = Engine.Instance.RenderTarget;
            Engine.Instance.RenderTarget = RenderTarget2D;
            Engine.Instance.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target,
                Color.Transparent, 1, 0);
        }

        public virtual void AfterRender()
        {
            Engine.Instance.RenderTarget = Was;

            Draw.SpriteBatch.Begin(effect: Effect, blendState: BlendState.AlphaBlend);
            Draw.SpriteBatch.Draw(RenderTarget2D, Vector2.Zero, Color.White);
            Draw.SpriteBatch.End();
        }
    }
}