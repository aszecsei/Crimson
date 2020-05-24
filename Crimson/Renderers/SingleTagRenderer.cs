using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    internal class SingleTagRenderer : Renderer
    {
        public BlendState BlendState;
        public Camera Camera;
        public Effect Effect;
        public SamplerState SamplerState;
        public BitTag Tag;

        public SingleTagRenderer(BitTag tag)
        {
            Tag = tag;
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.PointClamp;
            Camera = new Camera();
        }

        public override void Render(Scene scene)
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, DepthStencilState.None,
                RasterizerState.CullNone, Effect, Camera.Matrix * Engine.ScreenMatrix);

            foreach (Entity entity in scene[Tag])
                if (entity.Visible)
                    entity.Render();

            if (Engine.Commands.Open)
                foreach (Entity entity in scene[Tag])
                    entity.DebugRender(Camera);

            Draw.SpriteBatch.End();
        }
    }
}