using System.Runtime.InteropServices;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Shaders.Types;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;
using osuTK.Graphics.ES30;

namespace Kumi.Game.Graphics.Sprites;

public partial class LoadAnimation : Sprite
{
    [BackgroundDependencyLoader]
    private void load(ShaderManager shaders)
    {
        TextureShader = shaders.Load("LoadAnimation", "LoadAnimation");
    }

    private float animationProgress;

    public float AnimationProgress
    {
        get => animationProgress;
        set
        {
            if (animationProgress == value)
                return;

            animationProgress = value;
            Invalidate(Invalidation.DrawInfo);
        }
    }

    public override bool IsPresent => true;

    protected override DrawNode CreateDrawNode() => new LoadAnimationDrawNode(this);

    private class LoadAnimationDrawNode : SpriteDrawNode
    {
        private LoadAnimation source => (LoadAnimation) Source;

        private float progress;
        private readonly Action<TexturedVertex2D> addVertexAction;

        public LoadAnimationDrawNode(LoadAnimation source)
            : base(source)
        {
            addVertexAction = v =>
            {
                animationBatch!.Add(new LoadAnimationVertex
                {
                    Position = v.Position,
                    Colour = v.Colour,
                    TexturePosition = v.TexturePosition
                });
            };
        }

        public override void ApplyState()
        {
            base.ApplyState();

            progress = source.animationProgress;
        }

        private IUniformBuffer<AnimationData>? animationBuffer;
        private IVertexBatch<LoadAnimationVertex>? animationBatch;

        protected override void BindUniformResources(IShader shader, IRenderer renderer)
        {
            base.BindUniformResources(shader, renderer);

            animationBuffer ??= renderer.CreateUniformBuffer<AnimationData>();
            animationBatch ??= renderer.CreateQuadBatch<LoadAnimationVertex>(1, 2);

            animationBuffer.Data = animationBuffer.Data with { Progress = progress };

            shader.BindUniformBlock(@"m_AnimationData", animationBuffer);
        }

        protected override void Blit(IRenderer renderer)
        {
            if (DrawRectangle.Width == 0 || DrawRectangle.Height == 0)
                return;

            base.Blit(renderer);

            renderer.DrawQuad(
                Texture,
                ScreenSpaceDrawQuad,
                DrawColourInfo.Colour,
                inflationPercentage: new Vector2(InflationAmount.X / DrawRectangle.Width, InflationAmount.Y / DrawRectangle.Height),
                textureCoords: TextureCoords,
                vertexAction: addVertexAction
            );
        }

        protected override bool CanDrawOpaqueInterior => false;

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            animationBuffer?.Dispose();
            animationBatch?.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private record struct AnimationData
        {
            public UniformFloat Progress;
            private readonly UniformPadding12 _p1;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LoadAnimationVertex : IVertex, IEquatable<LoadAnimationVertex>
        {
            [VertexMember(2, VertexAttribPointerType.Float)]
            public Vector2 Position;

            [VertexMember(4, VertexAttribPointerType.Float)]
            public Color4 Colour;

            [VertexMember(2, VertexAttribPointerType.Float)]
            public Vector2 TexturePosition;

            public bool Equals(LoadAnimationVertex other)
                => Position.Equals(other.Position) &&
                   Colour.Equals(other.Colour) &&
                   TexturePosition.Equals(other.TexturePosition);
        }
    }
}
