using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class EverythingRenderer : Renderer
    {
        public BlendState BlendState;
        public Camera Camera;
        public Effect Effect;
        public SamplerState SamplerState;

        public EverythingRenderer()
        {
            BlendState = BlendState.AlphaBlend;
            SamplerState = SamplerState.PointClamp;
            Camera = new Camera();
        }

        public override void Render(Scene scene)
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, DepthStencilState.None,
                RasterizerState.CullNone, Effect, Camera.Matrix * Engine.ScreenMatrix);

            scene.Entities.Render();

            if (Engine.Commands.Open) scene.Entities.DebugRender(Camera);

            Draw.SpriteBatch.End();
        }
    }
}