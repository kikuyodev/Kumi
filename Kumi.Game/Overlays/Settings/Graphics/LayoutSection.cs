using System.Drawing;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Overlays.Settings.Components;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Framework.Platform.Windows;

namespace Kumi.Game.Overlays.Settings.Graphics;

public partial class LayoutSection : SettingSection
{
    protected override LocalisableString Header => "Layout";

    private readonly Bindable<Display> currentDisplay = new Bindable<Display>();

    private Bindable<Size> sizeFullscreen = null!;

    private readonly BindableList<Size> resolutions = new BindableList<Size>(new[] { new Size(9999, 9999) });
    private readonly IBindable<FullscreenCapability> fullscreenCapability = new Bindable<FullscreenCapability>();

    [Resolved]
    private GameHost host { get; set; } = null!;

    private IWindow? window;

    private DropdownSettingItem<Size> resolutionDropdown = null!;
    private DropdownSettingItem<Display> displayDropdown = null!;
    private DropdownSettingItem<WindowMode> windowModeDropdown = null!;

    [BackgroundDependencyLoader]
    private void load(FrameworkConfigManager config)
    {
        window = host.Window;

        sizeFullscreen = config.GetBindable<Size>(FrameworkSetting.SizeFullscreen);

        if (window != null)
        {
            currentDisplay.BindTo(window.CurrentDisplayBindable);
            window.DisplaysChanged += onDisplaysChanged;
        }

        if (host.Renderer is IWindowsRenderer windowsRenderer)
            fullscreenCapability.BindTo(windowsRenderer.FullscreenCapability);

        Children = new Drawable[]
        {
            windowModeDropdown = new DropdownSettingItem<WindowMode>
            {
                Label = "Screen Mode",
                Items = window?.SupportedWindowModes!,
                CanBeShown = { Value = window?.SupportedWindowModes.Count() > 1 },
                Current = config.GetBindable<WindowMode>(FrameworkSetting.WindowMode)
            },
            displayDropdown = new DisplayDropdownSettingItem
            {
                Label = "Display",
                Items = window?.Displays!,
                Current = currentDisplay
            },
            resolutionDropdown = new ResolutionDropdownSettingItem
            {
                Label = "Resolution",
                ItemSource = resolutions,
                Current = sizeFullscreen
            }
        };

        fullscreenCapability.BindValueChanged(_ => Schedule(updateScreenModeWarning), true);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        currentDisplay.BindValueChanged(display => Schedule(() =>
        {
            if (display.NewValue == null)
            {
                resolutions.Clear();
                return;
            }

            resolutions.ReplaceRange(1, resolutions.Count - 1, display.NewValue.DisplayModes
               .Where(m => m.Size is { Width: >= 800, Height: >= 600 })
               .OrderByDescending(m => Math.Max(m.Size.Height, m.Size.Width))
               .Select(m => m.Size)
               .Distinct());

            updateDisplaySettingsVisibility();
        }), true);
    }

    private void onDisplaysChanged(IEnumerable<Display> displays)
    {
        Scheduler.AddOnce(d =>
        {
            if (!displayDropdown.Items.SequenceEqual(d, DisplayListComparer.DEFAULT))
                displayDropdown.Items = d;

            updateDisplaySettingsVisibility();
        }, displays);
    }

    private void updateDisplaySettingsVisibility()
    {
        resolutionDropdown.CanBeShown.Value = resolutions.Count > 1 && windowModeDropdown.Current.Value == WindowMode.Fullscreen;
        displayDropdown.CanBeShown.Value = displayDropdown.Items.Count() > 1;
    }

    private void updateScreenModeWarning()
    {
        if (host.Renderer is IWindowsRenderer)
        {
            switch (fullscreenCapability.Value)
            {
                case FullscreenCapability.Unknown:
                    windowModeDropdown.Description = "Screen mode is unknown.";
                    break;

                case FullscreenCapability.Capable:
                    windowModeDropdown.Description = "Kumi is running exclusive fullscreen mode.";
                    break;

                case FullscreenCapability.Incapable:
                    windowModeDropdown.Description = "Kumi is running borderless windowed mode.";
                    break;
            }
        }
        else
        {
            windowModeDropdown.Description = string.Empty;
        }
    }

    private partial class DisplayDropdownSettingItem : DropdownSettingItem<Display>
    {
        protected override Drawable CreateControl()
            => new DisplayDropdownControl();

        private partial class DisplayDropdownControl : DropdownComponent<Display>
        {
            protected override KumiDropdown<Display> CreateDropdown()
                => new DisplayDropdown();

            private partial class DisplayDropdown : KumiDropdown<Display>
            {
                protected override LocalisableString GenerateItemText(Display item)
                {
                    return $"{item.Index}: {item.Name} ({item.Bounds.Width}x{item.Bounds.Height})";
                }
            }
        }
    }

    private partial class ResolutionDropdownSettingItem : DropdownSettingItem<Size>
    {
        protected override Drawable CreateControl()
            => new ResolutionDropdownControl();

        private partial class ResolutionDropdownControl : DropdownComponent<Size>
        {
            protected override KumiDropdown<Size> CreateDropdown()
                => new ResolutionDropdown();

            private partial class ResolutionDropdown : KumiDropdown<Size>
            {
                protected override LocalisableString GenerateItemText(Size item)
                {
                    if (item == new Size(9999, 9999))
                        return "Default";

                    return $"{item.Width}x{item.Height}";
                }
            }
        }
    }

    private class DisplayListComparer : IEqualityComparer<Display>
    {
        public static readonly DisplayListComparer DEFAULT = new DisplayListComparer();

        public bool Equals(Display? x, Display? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            return x.Index == y.Index && x.Name == y.Name && x.DisplayModes.SequenceEqual(y.DisplayModes);
        }

        public int GetHashCode(Display obj)
        {
            var hashCode = new HashCode();

            hashCode.Add(obj.Index);
            hashCode.Add(obj.Name);
            hashCode.Add(obj.DisplayModes.Length);
            foreach (var displayMode in obj.DisplayModes)
                hashCode.Add(displayMode);

            return hashCode.ToHashCode();
        }
    }
}
