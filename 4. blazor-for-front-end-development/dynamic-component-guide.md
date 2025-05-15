# Blazor DynamicComponent: Code Samples & Explanations

The `DynamicComponent` is a powerful feature in Blazor that allows you to render components whose type is determined at runtime rather than compile time. This enables highly flexible UI systems where components can be selected dynamically based on data, configuration, or user actions.

## Basic Usage

```csharp
<DynamicComponent Type="@componentType" Parameters="@parameters" />

@code {
    private Type componentType = typeof(Counter);
    private Dictionary<string, object> parameters = new()
    {
        { "IncrementAmount", 10 }
    };
}
```

**Explanation:**
- `Type`: Accepts a `System.Type` that specifies which component to render
- `Parameters`: Accepts a dictionary of parameter values to pass to the component
- In this example, we're rendering the `Counter` component and passing `10` to its `IncrementAmount` parameter

## Switching Components Based on Data

```csharp
<div class="dynamic-content">
    @foreach (var item in contentItems)
    {
        <DynamicComponent Type="@GetComponentTypeForItem(item)" 
                         Parameters="@GetParametersForItem(item)" />
    }
</div>

@code {
    private List<ContentItem> contentItems = new();

    private Type GetComponentTypeForItem(ContentItem item)
    {
        return item.ContentType switch
        {
            "text" => typeof(TextDisplay),
            "image" => typeof(ImageDisplay),
            "video" => typeof(VideoPlayer),
            "chart" => typeof(ChartComponent),
            _ => typeof(DefaultDisplay)
        };
    }

    private Dictionary<string, object> GetParametersForItem(ContentItem item)
    {
        var parameters = new Dictionary<string, object>
        {
            { "ItemId", item.Id },
            { "Data", item.Data }
        };

        // Add content type specific parameters
        if (item.ContentType == "image")
        {
            parameters.Add("LazyLoad", true);
            parameters.Add("ThumbnailSize", "medium");
        }
        else if (item.ContentType == "video")
        {
            parameters.Add("AutoPlay", false);
        }

        return parameters;
    }

    public class ContentItem
    {
        public int Id { get; set; }
        public string ContentType { get; set; } // "text", "image", "video", "chart"
        public object Data { get; set; }
    }
}
```

**Explanation:**
- This example iterates through a list of content items
- For each item, we determine the appropriate component to display based on its content type
- We also construct custom parameters for each component based on the item's content type
- This creates a fully dynamic content display system

## Combined with Virtualize Component

```csharp
<div style="height: 500px; overflow-y: auto;">
    <Virtualize Items="@contentItems" Context="item">
        <DynamicComponent Type="@GetComponentTypeForItem(item)" 
                         Parameters="@GetParametersForItem(item)" />
    </Virtualize>
</div>
```

**Explanation:**
- This powerful combination uses `Virtualize` for performance optimization with large lists
- Each virtualized item renders as a dynamically selected component
- Only visible components are rendered, but the type selection is still dynamic

## Loading Components from Configuration

```csharp
@inject IConfiguration Configuration

<div class="dashboard">
    @foreach (var widget in dashboardWidgets)
    {
        <div class="widget" style="grid-area: @widget.GridPosition">
            <DynamicComponent Type="@GetWidgetType(widget.Type)" 
                             Parameters="@widget.Parameters" />
        </div>
    }
</div>

@code {
    private List<DashboardWidget> dashboardWidgets = new();

    protected override void OnInitialized()
    {
        // Load widget configuration
        var widgetConfigs = Configuration.GetSection("DashboardWidgets").Get<List<WidgetConfig>>();
        
        foreach (var config in widgetConfigs)
        {
            dashboardWidgets.Add(new DashboardWidget
            {
                Type = config.Type,
                GridPosition = config.GridPosition,
                Parameters = new Dictionary<string, object>
                {
                    { "Title", config.Title },
                    { "RefreshInterval", config.RefreshInterval },
                    { "DataSource", config.DataSource }
                }
            });
        }
    }

    private Type GetWidgetType(string widgetType)
    {
        // Map configuration names to actual component types
        var widgetTypeMappings = new Dictionary<string, Type>
        {
            { "weather", typeof(WeatherWidget) },
            { "stocks", typeof(StockTickerWidget) },
            { "calendar", typeof(CalendarWidget) },
            { "metrics", typeof(MetricsWidget) }
        };

        return widgetTypeMappings.TryGetValue(widgetType, out var type) 
            ? type 
            : typeof(UnknownWidget);
    }

    private class DashboardWidget
    {
        public string Type { get; set; }
        public string GridPosition { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    private class WidgetConfig
    {
        public string Type { get; set; }
        public string GridPosition { get; set; }
        public string Title { get; set; }
        public int RefreshInterval { get; set; }
        public string DataSource { get; set; }
    }
}
```

**Explanation:**
- This example loads dashboard widget configurations from app settings
- Each widget's type and parameters are determined by the configuration
- The layout is also dynamic, using CSS grid positions from configuration
- This creates a completely configurable dashboard without recompilation

## Loading Components Dynamically Using Assembly Scanning

```csharp
@inject AssemblyLoader AssemblyLoader

<div class="plugin-container">
    @if (selectedPlugin != null)
    {
        <DynamicComponent Type="@selectedPlugin.ComponentType" 
                         Parameters="@GetPluginParameters()" />
    }
    else
    {
        <div class="no-plugin-selected">Select a plugin to load</div>
    }
</div>

<div class="plugin-selector">
    @foreach (var plugin in availablePlugins)
    {
        <button class="plugin-button @(plugin == selectedPlugin ? "selected" : "")"
                @onclick="() => SelectPlugin(plugin)">
            @plugin.Name
        </button>
    }
</div>

@code {
    private List<PluginInfo> availablePlugins = new();
    private PluginInfo selectedPlugin;

    protected override async Task OnInitializedAsync()
    {
        // Scan assemblies for plugin components
        availablePlugins = await AssemblyLoader.ScanForPluginsAsync();
    }

    private void SelectPlugin(PluginInfo plugin)
    {
        selectedPlugin = plugin;
    }

    private Dictionary<string, object> GetPluginParameters()
    {
        return new Dictionary<string, object>
        {
            { "UserId", CurrentUser.Id },
            { "Settings", UserSettings },
            { "OnSettingsChanged", EventCallback.Factory.Create<SettingsChangedEventArgs>(this, HandleSettingsChanged) }
        };
    }

    private void HandleSettingsChanged(SettingsChangedEventArgs args)
    {
        // Update settings based on plugin output
        UserSettings = args.NewSettings;
    }

    public class PluginInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Type ComponentType { get; set; }
    }

    // This would be implemented as a service
    public class AssemblyLoader
    {
        public async Task<List<PluginInfo>> ScanForPluginsAsync()
        {
            // In a real implementation, this would scan assemblies
            // for components with [Plugin] attribute or implementing IPlugin
            
            // Example static implementation
            return new List<PluginInfo>
            {
                new PluginInfo 
                { 
                    Name = "Data Visualizer", 
                    Description = "Visualize data as charts",
                    ComponentType = typeof(Plugins.DataVisualizer) 
                },
                new PluginInfo 
                { 
                    Name = "Report Generator", 
                    Description = "Generate printable reports",
                    ComponentType = typeof(Plugins.ReportGenerator) 
                }
            };
        }
    }
}
```

**Explanation:**
- This example demonstrates a plugin system using DynamicComponent
- Plugins are discovered by scanning assemblies
- Users can select which plugin to load and use
- Each plugin is rendered as a Blazor component with appropriate parameters
- This enables extensible applications where new features can be added without modifying the core code

## Creating a Tab System with DynamicComponent

```csharp
<div class="tab-container">
    <div class="tab-header">
        @foreach (var tab in tabs)
        {
            <button class="tab-button @(activeTabIndex == tab.Index ? "active" : "")"
                    @onclick="() => ActivateTab(tab.Index)">
                @tab.Title
            </button>
        }
    </div>
    
    <div class="tab-content">
        @if (activeTabIndex >= 0 && activeTabIndex < tabs.Count)
        {
            var activeTab = tabs[activeTabIndex];
            <DynamicComponent Type="@activeTab.ComponentType"
                             Parameters="@activeTab.Parameters" />
        }
    </div>
</div>

@code {
    private List<TabInfo> tabs = new();
    private int activeTabIndex = 0;

    protected override void OnInitialized()
    {
        tabs = new List<TabInfo>
        {
            new TabInfo 
            { 
                Index = 0,
                Title = "User Profile", 
                ComponentType = typeof(UserProfileTab),
                Parameters = new Dictionary<string, object>
                {
                    { "UserId", CurrentUserId },
                    { "EditMode", false }
                }
            },
            new TabInfo 
            { 
                Index = 1,
                Title = "Settings", 
                ComponentType = typeof(SettingsTab),
                Parameters = new Dictionary<string, object>
                {
                    { "Categories", new[] { "General", "Privacy", "Notifications" } }
                }
            },
            new TabInfo 
            { 
                Index = 2,
                Title = "Statistics", 
                ComponentType = typeof(StatisticsTab),
                Parameters = new Dictionary<string, object>
                {
                    { "TimeRange", "Monthly" },
                    { "ShowCharts", true }
                }
            }
        };
    }

    private void ActivateTab(int index)
    {
        activeTabIndex = index;
    }

    private class TabInfo
    {
        public int Index { get; set; }
        public string Title { get; set; }
        public Type ComponentType { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
```

**Explanation:**
- This example creates a tab system where each tab can be a completely different component
- The active tab's component is rendered using DynamicComponent
- Parameters are configured for each tab component
- This pattern is useful for complex forms, settings pages, or dashboard sections

## Key Benefits of DynamicComponent

1. **Runtime Flexibility**: Components can be selected at runtime based on data or user choices
2. **Configurable UI**: Application UI can be configured without recompilation
3. **Plugin Architecture**: Supports modular application design with plugin systems
4. **Content Management**: Different content types can use specialized components
5. **Reduced Conditional Rendering**: Cleaner code than massive if/else trees for different components

## Considerations and Best Practices

1. **Type Safety**: Error handling for missing components or parameters must be considered
2. **Performance**: Creating components dynamically may have slightly higher overhead
3. **Debugging**: Errors in dynamically loaded components can be harder to trace
4. **Security**: When loading external components, validate sources appropriately
5. **Documentation**: Document the expected parameters for each component that might be loaded dynamically

DynamicComponent is one of the most powerful features in Blazor for creating truly flexible, configurable interfaces that can adapt to different data types, user preferences, or business requirements.
