using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Overlays.Settings.Components;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace Kumi.Game.Overlays.Settings.Graphics;

public partial class RendererSettings : SettingSection
{
    protected override LocalisableString Header => "Renderer";

    private bool automaticRendererInUse;

    [BackgroundDependencyLoader]
    private void load(FrameworkConfigManager config, GameHost host)
    {
        var renderer = config.GetBindable<RendererType>(FrameworkSetting.Renderer);
        automaticRendererInUse = renderer.Value == RendererType.Automatic;

        DropdownEnumSettingItem<RendererType> rendererDropdown;

        Children = new Drawable[]
        {
            rendererDropdown = new RendererDropdownSettingItem
            {
                Label = "Renderer",
                Description = "A restart is required for this setting to take effect.",
                Current = renderer,
                // Vulkan is highly unstable on osu-framework, so we're disabling it for now.
                Items = host.GetPreferredRenderersForCurrentPlatform().OrderBy(t => t).Where(r => r != RendererType.Vulkan)
            },
            new DropdownEnumSettingItem<FrameSync>
            {
                Label = "Frame limiter",
                Current = config.GetBindable<FrameSync>(FrameworkSetting.FrameSync)
            },
            new DropdownEnumSettingItem<ExecutionMode>
            {
                Label = "Threading mode",
                Current = config.GetBindable<ExecutionMode>(FrameworkSetting.ExecutionMode)
            }
        };
        
        renderer.BindValueChanged(r =>
        {
            if (r.NewValue == host.ResolvedRenderer)
                return;
            
            if (r.NewValue == RendererType.Automatic && automaticRendererInUse)
                return;
            
            // TODO: prompt for restart
            
        });
    }

    private partial class RendererDropdownSettingItem : DropdownEnumSettingItem<RendererType>
    {
        protected override Drawable CreateControl()
            => new RendererDropdownControl();

        private partial class RendererDropdownControl : DropdownComponent<RendererType>
        {
            protected override KumiDropdown<RendererType> CreateDropdown()
                => new RendererDropdown();

            private partial class RendererDropdown : KumiDropdown<RendererType>
            {
                private RendererType hostResolvedRenderer;
                private bool automaticRendererInUse;

                [BackgroundDependencyLoader]
                private void load(FrameworkConfigManager config, GameHost host)
                {
                    var renderer = config.GetBindable<RendererType>(FrameworkSetting.Renderer);
                    automaticRendererInUse = renderer.Value == RendererType.Automatic;
                    hostResolvedRenderer = host.ResolvedRenderer;
                }
                
                protected override LocalisableString GenerateItemText(RendererType item)
                {
                    if (item == RendererType.Automatic && automaticRendererInUse)
                        return $"{base.GenerateItemText(item)} ({hostResolvedRenderer.GetDescription()})";

                    return base.GenerateItemText(item);
                }
            }
        }
    }
}
