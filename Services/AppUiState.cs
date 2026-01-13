namespace Tutor.Services;

/// <summary>
/// Represents the active top-level navigation tab.
/// </summary>
public enum TopTab
{
    Home,
    Learn,
    Courses,
    KnowledgeGraph,
    Settings
}

/// <summary>
/// Represents a node in the side navigation hierarchy.
/// </summary>
public class NavNode
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public List<NavNode> Children { get; set; } = [];
    public bool IsExpanded { get; set; }
    public bool IsSelected { get; set; }
    public object? Data { get; set; }
}

/// <summary>
/// Service for managing shared UI state across components.
/// Tracks the active navigation tab and provides side navigation nodes.
/// </summary>
public sealed class AppUiState
{
    private TopTab _activeTab = TopTab.Learn;
    private List<NavNode> _sideNavNodes = [];
    private string? _selectedNodeId;

    /// <summary>
    /// Event fired when the active tab changes.
    /// </summary>
    public event Action? OnTabChanged;

    /// <summary>
    /// Event fired when the side navigation nodes change.
    /// </summary>
    public event Action? OnSideNavChanged;

    /// <summary>
    /// Event fired when a side nav node is selected.
    /// </summary>
    public event Action<NavNode?>? OnNodeSelected;

    /// <summary>
    /// Gets the currently active top-level tab.
    /// </summary>
    public TopTab ActiveTab => _activeTab;

    /// <summary>
    /// Gets the side navigation nodes for the current context.
    /// </summary>
    public IReadOnlyList<NavNode> SideNavNodes => _sideNavNodes;

    /// <summary>
    /// Gets the currently selected node ID.
    /// </summary>
    public string? SelectedNodeId => _selectedNodeId;

    /// <summary>
    /// Sets the active top-level tab and notifies listeners.
    /// </summary>
    public void SetActiveTab(TopTab tab)
    {
        if (_activeTab != tab)
        {
            _activeTab = tab;
            OnTabChanged?.Invoke();
        }
    }

    /// <summary>
    /// Updates the side navigation nodes for the current context.
    /// </summary>
    public void SetSideNavNodes(List<NavNode> nodes)
    {
        _sideNavNodes = nodes;
        OnSideNavChanged?.Invoke();
    }

    /// <summary>
    /// Selects a node in the side navigation.
    /// </summary>
    public void SelectNode(string? nodeId)
    {
        if (_selectedNodeId != nodeId)
        {
            // Clear previous selection
            ClearSelection(_sideNavNodes);

            _selectedNodeId = nodeId;

            // Set new selection
            if (nodeId != null)
            {
                var node = FindNode(_sideNavNodes, nodeId);
                if (node != null)
                {
                    node.IsSelected = true;
                    ExpandParents(_sideNavNodes, nodeId);
                }
                OnNodeSelected?.Invoke(node);
            }
            else
            {
                OnNodeSelected?.Invoke(null);
            }

            OnSideNavChanged?.Invoke();
        }
    }

    /// <summary>
    /// Toggles the expanded state of a node.
    /// </summary>
    public void ToggleNode(string nodeId)
    {
        var node = FindNode(_sideNavNodes, nodeId);
        if (node != null)
        {
            node.IsExpanded = !node.IsExpanded;
            OnSideNavChanged?.Invoke();
        }
    }

    /// <summary>
    /// Expands all nodes to a specific node.
    /// </summary>
    public void ExpandToNode(string nodeId)
    {
        ExpandParents(_sideNavNodes, nodeId);
        OnSideNavChanged?.Invoke();
    }

    private static void ClearSelection(List<NavNode> nodes)
    {
        foreach (var node in nodes)
        {
            node.IsSelected = false;
            ClearSelection(node.Children);
        }
    }

    private static NavNode? FindNode(List<NavNode> nodes, string id)
    {
        foreach (var node in nodes)
        {
            if (node.Id == id)
                return node;

            var found = FindNode(node.Children, id);
            if (found != null)
                return found;
        }
        return null;
    }

    private static bool ExpandParents(List<NavNode> nodes, string targetId)
    {
        foreach (var node in nodes)
        {
            if (node.Id == targetId)
                return true;

            if (ExpandParents(node.Children, targetId))
            {
                node.IsExpanded = true;
                return true;
            }
        }
        return false;
    }
}
