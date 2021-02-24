using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Crimson
{
    public class EverythingRenderer : Renderer
    {
        public BlendState BlendState;
        public Camera Camera;
        public Effect Effect;
        public SamplerState SamplerState;

        public bool UseEngineScreenMatrix;

        public EverythingRenderer(bool useEngineScreenMatrix = true)
        {
            BlendState            = BlendState.AlphaBlend;
            SamplerState          = SamplerState.PointClamp;
            Camera                = new Camera();
            UseEngineScreenMatrix = useEngineScreenMatrix;
        }

        public override void Render(Scene scene)
        {
            Matrix matrix                       = Camera.Matrix;
            if ( UseEngineScreenMatrix ) matrix *= Engine.ScreenMatrix;

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, DepthStencilState.None,
                RasterizerState.CullNone, Effect, matrix);

            scene.Entities.Render();

            if (Engine.Commands.Open) scene.Entities.DebugRender(Camera);

            Draw.SpriteBatch.End();
        }
    }
}
