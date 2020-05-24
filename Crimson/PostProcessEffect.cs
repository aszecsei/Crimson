using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class PostProcessEffect
    {
        protected RenderTarget2D _renderTarget2D;

        protected RenderTarget2D _was;

        public PostProcessEffect(Effect effect)
        {
            Effect = effect;
        }

        public Effect Effect { get; }

        ~PostProcessEffect()
        {
            _renderTarget2D?.Dispose();
        }

        public virtual void BeforeRender()
        {
            if (_renderTarget2D != null &&
                (_renderTarget2D.Width != Engine.ViewWidth || _renderTarget2D.Height != Engine.ViewHeight))
            {
                _renderTarget2D.Dispose();
                _renderTarget2D = null;
            }

            if (_renderTarget2D == null)
                _renderTarget2D = Utils.CreateRenderTarget(Engine.ViewWidth, Engine.ViewHeight);

            _was = Engine.Instance.RenderTarget;
            Engine.Instance.RenderTarget = _renderTarget2D;
            Engine.Instance.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target,
                Color.Transparent, 1, 0);
        }

        public virtual void AfterRender()
        {
            Engine.Instance.RenderTarget = _was;

            Draw.SpriteBatch.Begin(effect: Effect, blendState: BlendState.AlphaBlend);
            Draw.SpriteBatch.Draw(_renderTarget2D, Vector2.Zero, Color.White);
            Draw.SpriteBatch.End();
        }
    }
}