using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class PostProcessEffect
    {
        protected VirtualRenderTarget? Src;
        protected RenderTarget2D?      Dst;

        public int? OverrideWidth  = null;
        public int? OverrideHeight = null;

        public PostProcessEffect(Effect effect)
        {
            Effect    = effect;
        }

        public Effect Effect { get; }

        ~PostProcessEffect()
        {
            Src?.Dispose();
        }

        public virtual void Update()
        {

        }

        public virtual void BeforeRender()
        {
            int width  = OverrideWidth  ?? Engine.ViewWidth;
            int height = OverrideHeight ?? Engine.ViewHeight;

            if (Src != null && (Src.Width != width || Src.Height != height))
            {
                Src.Dispose();
                Src = null;
            }

            if ( Src == null )
                Src = VirtualContent.CreateRenderTarget("post-process", width, height);

            Dst = Engine.Instance.RenderTarget;
            Engine.Instance.RenderTarget = Src;
            Engine.Instance.GraphicsDevice.Clear(ClearOptions.DepthBuffer | ClearOptions.Stencil | ClearOptions.Target,
                Color.Transparent, 1, 0);
        }

        protected virtual void SetEffectParameters()
        {

        }

        public virtual void AfterRender()
        {
            int width  = OverrideWidth  ?? Engine.ViewWidth;
            int height = OverrideHeight ?? Engine.ViewHeight;

            int dstWidth  = Dst?.Width ?? Engine.ViewWidth;
            int dstHeight = Dst?.Height ?? Engine.ViewHeight;

            Engine.Instance.RenderTarget = Dst;

            SetEffectParameters();

            Draw.SpriteBatch.Begin(effect: Effect, samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);
            Draw.SpriteBatch.Draw(Src!, new Rectangle(0, 0, dstWidth, dstHeight), new Rectangle(0, 0, width, height), Color.White);
            Draw.SpriteBatch.End();
        }
    }
}
