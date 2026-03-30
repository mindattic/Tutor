using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class AppUiStateTests
{
    [Fact]
    public void DefaultTab_IsLearn()
    {
        var state = new AppUiState();
        Assert.Equal(TopTab.Learn, state.ActiveTab);
    }

    [Fact]
    public void SideNavNodes_InitiallyEmpty()
    {
        var state = new AppUiState();
        Assert.Empty(state.SideNavNodes);
    }

    [Fact]
    public void SelectedNodeId_InitiallyNull()
    {
        var state = new AppUiState();
        Assert.Null(state.SelectedNodeId);
    }

    [Fact]
    public void SetActiveTab_ChangesTab()
    {
        var state = new AppUiState();
        state.SetActiveTab(TopTab.Courses);
        Assert.Equal(TopTab.Courses, state.ActiveTab);
    }

    [Fact]
    public void SetActiveTab_FiresEvent()
    {
        var state = new AppUiState();
        var fired = false;
        state.OnTabChanged += () => fired = true;
        state.SetActiveTab(TopTab.Settings);
        Assert.True(fired);
    }

    [Fact]
    public void SetActiveTab_SameTab_DoesNotFireEvent()
    {
        var state = new AppUiState();
        state.SetActiveTab(TopTab.Home);
        var fired = false;
        state.OnTabChanged += () => fired = true;
        state.SetActiveTab(TopTab.Home); // same tab again
        Assert.False(fired);
    }

    [Fact]
    public void SetSideNavNodes_UpdatesNodes()
    {
        var state = new AppUiState();
        var nodes = new List<NavNode>
        {
            new() { Id = "n1", Title = "Lesson 1" },
            new() { Id = "n2", Title = "Lesson 2" }
        };
        state.SetSideNavNodes(nodes);
        Assert.Equal(2, state.SideNavNodes.Count);
    }

    [Fact]
    public void SetSideNavNodes_FiresEvent()
    {
        var state = new AppUiState();
        var fired = false;
        state.OnSideNavChanged += () => fired = true;
        state.SetSideNavNodes([new NavNode { Id = "n1" }]);
        Assert.True(fired);
    }

    [Fact]
    public void SelectNode_UpdatesSelectedNodeId()
    {
        var state = new AppUiState();
        state.SetSideNavNodes([new NavNode { Id = "n1" }]);
        state.SelectNode("n1");
        Assert.Equal("n1", state.SelectedNodeId);
    }

    [Fact]
    public void SelectNode_MarksNodeAsSelected()
    {
        var node = new NavNode { Id = "n1" };
        var state = new AppUiState();
        state.SetSideNavNodes([node]);
        state.SelectNode("n1");
        Assert.True(node.IsSelected);
    }

    [Fact]
    public void SelectNode_ClearsPreviousSelection()
    {
        var n1 = new NavNode { Id = "n1" };
        var n2 = new NavNode { Id = "n2" };
        var state = new AppUiState();
        state.SetSideNavNodes([n1, n2]);

        state.SelectNode("n1");
        Assert.True(n1.IsSelected);

        state.SelectNode("n2");
        Assert.False(n1.IsSelected);
        Assert.True(n2.IsSelected);
    }

    [Fact]
    public void SelectNode_Null_ClearsSelection()
    {
        var n1 = new NavNode { Id = "n1" };
        var state = new AppUiState();
        state.SetSideNavNodes([n1]);
        state.SelectNode("n1");
        state.SelectNode(null);
        Assert.Null(state.SelectedNodeId);
        Assert.False(n1.IsSelected);
    }

    [Fact]
    public void SelectNode_FiresOnNodeSelected()
    {
        var state = new AppUiState();
        var node = new NavNode { Id = "n1", Title = "Test" };
        state.SetSideNavNodes([node]);

        NavNode? selectedNode = null;
        state.OnNodeSelected += n => selectedNode = n;
        state.SelectNode("n1");
        Assert.NotNull(selectedNode);
        Assert.Equal("n1", selectedNode.Id);
    }

    [Fact]
    public void ToggleNode_FlipsExpandedState()
    {
        var node = new NavNode { Id = "n1", IsExpanded = false };
        var state = new AppUiState();
        state.SetSideNavNodes([node]);

        state.ToggleNode("n1");
        Assert.True(node.IsExpanded);

        state.ToggleNode("n1");
        Assert.False(node.IsExpanded);
    }

    [Fact]
    public void SelectNode_ExpandsParents()
    {
        var child = new NavNode { Id = "child" };
        var parent = new NavNode { Id = "parent", Children = [child], IsExpanded = false };
        var state = new AppUiState();
        state.SetSideNavNodes([parent]);

        state.SelectNode("child");
        Assert.True(parent.IsExpanded);
    }

    [Fact]
    public void ExpandToNode_ExpandsAncestors()
    {
        var grandchild = new NavNode { Id = "gc" };
        var child = new NavNode { Id = "child", Children = [grandchild], IsExpanded = false };
        var root = new NavNode { Id = "root", Children = [child], IsExpanded = false };
        var state = new AppUiState();
        state.SetSideNavNodes([root]);

        state.ExpandToNode("gc");
        Assert.True(root.IsExpanded);
        Assert.True(child.IsExpanded);
    }

    [Fact]
    public void NavNode_Defaults()
    {
        var node = new NavNode();
        Assert.Equal("", node.Id);
        Assert.Equal("", node.Title);
        Assert.Empty(node.Children);
        Assert.False(node.IsExpanded);
        Assert.False(node.IsSelected);
        Assert.Equal(Tutor.Core.Models.SectionStatus.NotStarted, node.Status);
        Assert.Equal(0, node.CompletionPercentage);
        Assert.Equal(0, node.Depth);
    }

    [Theory]
    [InlineData(TopTab.Home)]
    [InlineData(TopTab.Learn)]
    [InlineData(TopTab.Courses)]
    [InlineData(TopTab.KnowledgeGraph)]
    [InlineData(TopTab.Settings)]
    public void TopTab_AllValues(TopTab tab)
    {
        Assert.True(Enum.IsDefined(tab));
    }
}
