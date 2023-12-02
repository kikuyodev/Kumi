/*
    Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.

     Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input.Events;
using osu.Framework.Layout;
using osu.Framework.Timing;
using osu.Framework.Utils;
using osuTK;

namespace Kumi.Game.Graphics.Containers;

public partial class ZoomableScrollContainer : BasicScrollContainer
{
    public double ZoomDuration;

    public Easing ZoomEasing;

    private readonly Container zoomedContent;
    protected override Container<Drawable> Content => zoomedContent;

    public float CurrentZoom { get; private set; } = 1f;

    private bool isZoomSetUp;

    [Resolved]
    private IFrameBasedClock? editorClock { get; set; }

    private readonly LayoutValue zoomedContentWidthCache = new LayoutValue(Invalidation.DrawSize);

    private float minZoom;
    private float maxZoom;

    protected ZoomableScrollContainer()
        : base(Direction.Horizontal)
    {
        base.Content.Add(zoomedContent = new Container
        {
            RelativeSizeAxes = Axes.Y,
            Alpha = 0
        });

        AddLayout(zoomedContentWidthCache);
    }

    protected void SetupZoom(float initial, float minimum, float maximum)
    {
        if (minimum < 1)
            throw new ArgumentException($"{nameof(minimum)} ({minimum}) must be greater than 1.", nameof(minimum));

        if (maximum < 1)
            throw new ArgumentException($"{nameof(maximum)} ({maximum}) must be greater than 1.", nameof(maximum));

        if (minimum > maximum)
            throw new ArgumentException($"{nameof(minimum)} ({minimum}) must be less than {nameof(maximum)} ({maximum}).");

        if (initial < minimum || initial > maximum)
            throw new ArgumentException($"{nameof(initial)} ({initial}) must be between {nameof(minimum)} ({minimum}) and {nameof(maximum)} ({maximum}).");

        minZoom = minimum;
        maxZoom = maximum;

        CurrentZoom = zoomTarget = initial;
        zoomedContentWidthCache.Invalidate();

        isZoomSetUp = true;
        zoomedContent.Show();
    }

    public float Zoom
    {
        get => zoomTarget;
        set => updateZoom(value);
    }

    private void updateZoom(float value)
    {
        if (!isZoomSetUp)
            return;

        var newZoom = Math.Clamp(value, minZoom, maxZoom);

        if (IsLoaded)
            setZoomTarget(newZoom, ToSpaceOfOtherDrawable(new Vector2(DrawWidth / 2, 0), zoomedContent).X);
        else
            CurrentZoom = zoomTarget = newZoom;
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (!zoomedContentWidthCache.IsValid)
            updateZoomedContentWidth();
    }

    protected override bool OnScroll(ScrollEvent e)
    {
        if (e.AltPressed)
        {
            AdjustZoomRelatively(e.ScrollDelta.Y, zoomedContent.ToLocalSpace(e.ScreenSpaceMousePosition).X);
            return true;
        }

        if (editorClock?.IsRunning == true)
            return false;
        
        return base.OnScroll(e);
    }

    private void updateZoomedContentWidth()
    {
        zoomedContent.Width = DrawWidth * CurrentZoom;
        zoomedContentWidthCache.Validate();
    }

    public void AdjustZoomRelatively(float change, float? focusPoint = null)
    {
        if (!isZoomSetUp)
            return;

        const float zoom_change_sensitivity = 0.02f;

        setZoomTarget(zoomTarget + change * (maxZoom - minZoom) * zoom_change_sensitivity, focusPoint);
    }

    private float zoomTarget = 1;

    private void setZoomTarget(float newZoom, float? focusPoint = null)
    {
        zoomTarget = Math.Clamp(newZoom, minZoom, maxZoom);
        focusPoint ??= zoomedContent.ToLocalSpace(ToScreenSpace(new Vector2(DrawWidth / 2, 0))).X;

        transformZoomTo(zoomTarget, focusPoint.Value, ZoomDuration, ZoomEasing);
        OnZoomChanged();
    }

    private void transformZoomTo(float newZoom, float focusPoint, double duration = 0, Easing easing = Easing.None)
        => this.TransformTo(this.PopulateTransform(new TransformZoom(focusPoint, zoomedContent.Width, Current), newZoom, duration, easing));

    protected virtual void OnZoomChanged()
    {
    }

    private class TransformZoom : Transform<float, ZoomableScrollContainer>
    {
        private readonly float focusPoint;
        private readonly float contentSize;
        private readonly float scrollOffset;

        public TransformZoom(float focusPoint, float contentSize, float scrollOffset)
        {
            this.focusPoint = focusPoint;
            this.contentSize = contentSize;
            this.scrollOffset = scrollOffset;
        }

        public override string TargetMember => nameof(CurrentZoom);

        private float valueAt(double time)
        {
            if (time < StartTime) return StartValue;
            if (time >= EndTime) return EndValue;

            return Interpolation.ValueAt(time, StartValue, EndValue, StartTime, EndTime, Easing);
        }

        protected override void Apply(ZoomableScrollContainer d, double time)
        {
            var newZoom = valueAt(time);

            var focusOffset = focusPoint - scrollOffset;
            var expectedWidth = d.DrawWidth * newZoom;
            var targetOffset = expectedWidth * (focusPoint / contentSize) - focusOffset;

            d.CurrentZoom = newZoom;
            d.updateZoomedContentWidth();

            d.Invalidate(Invalidation.DrawSize);
            d.ScrollTo(targetOffset, false);
        }

        protected override void ReadIntoStartValue(ZoomableScrollContainer d)
            => StartValue = d.CurrentZoom;
    }
}
