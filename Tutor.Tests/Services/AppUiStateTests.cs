using Tutor.Core.Services;

namespace Tutor.Tests.Services;

public class AppUiStateTests
{
    [Test]
    public void DefaultTab_IsLearn()
    {
        var state = new AppUiState();
        Assert.That(state.ActiveTab, Is.EqualTo(TopTab.Learn));
    }

    [Test]
    public void SideNavNodes_InitiallyEmpty()
    {
        var state = new AppUiState();
        Assert.That(state.SideNavNodes, Is.Empty);
    }

    [Test]
    public void SelectedNodeId_InitiallyNull()
    {
        var state = new AppUiState();
        Assert.That(state.SelectedNodeId, Is.Null);
    }

    [Test]
    public void SetActiveTab_ChangesTab()
    {
        var state = new AppUiState();
        state.SetActiveTab(TopTab.Courses);
        Assert.That(state.ActiveTab, Is.EqualTo(TopTab.Courses));
    }

    [Test]
    public void SetActiveTab_FiresEvent()
    {
        var state = new AppUiState();
        var fired = false;
        state.OnTabChanged += () => fired = true;
        state.SetActiveTab(TopTab.Settings);
        Assert.That(fired, Is.True);
    }

    [Test]
    public void SetActiveTab_SameTab_DoesNotFireEvent()
    {
        var state = new AppUiState();
        state.SetActiveTab(TopTab.Home);
        var fired = false;
        state.OnTabChanged += () => fired = true;
        state.SetActiveTab(TopTab.Home); // same tab again
        Assert.That(fired, Is.False);
    }

    [Test]
    public void SetSideNavNodes_UpdatesNodes()
    {
        var state = new AppUiState();
        var nodes = new List<NavNode>
        {
            new() { Id = "n1", Title = "Lesson 1" },
            new() { Id = "n2", Title = "Lesson 2" }
        };
        state.SetSideNavNodes(nodes);
        Assert.That(state.SideNavNodes.Count, Is.EqualTo(2));
    }

    [Test]
    public void SetSideNavNodes_FiresEvent()
    {
        var state = new AppUiState();
        var fired = false;
        state.OnSideNavChanged += () => fired = true;
        state.SetSideNavNodes([new NavNode { Id = "n1" }]);
        Assert.That(fired, Is.True);
    }

    [Test]
    public void SelectNode_UpdatesSelectedNodeId()
    {
        var state = new AppUiState();
        state.SetSideNavNodes([new NavNode { Id = "n1" }]);
        state.SelectNode("n1");
        Assert.That(state.SelectedNodeId, Is.EqualTo("n1"));
    }

    [Test]
    public void SelectNode_MarksNodeAsSelected()
    {
        var node = new NavNode { Id = "n1" };
        var state = new AppUiState();
        state.SetSideNavNodes([node]);
        state.SelectNode("n1");
        Assert.That(node.IsSelected, Is.True);
    }

    [Test]
    public void SelectNode_ClearsPreviousSelection()
    {
        var n1 = new NavNode { Id = "n1" };
        var n2 = new NavNode { Id = "n2" };
        var state = new AppUiState();
        state.SetSideNavNodes([n1, n2]);

        state.SelectNode("n1");
        Assert.That(n1.IsSelected, Is.True);

        state.SelectNode("n2");
        Assert.That(n1.IsSelected, Is.False);
        Assert.That(n2.IsSelected, Is.True);
    }

    [Test]
    public void SelectNode_Null_ClearsSelection()
    {
        var n1 = new NavNode { Id = "n1" };
        var state = new AppUiState();
        state.SetSideNavNodes([n1]);
        state.SelectNode("n1");
        state.SelectNode(null);
        Assert.That(state.SelectedNodeId, Is.Null);
        Assert.That(n1.IsSelected, Is.False);
    }

    [Test]
    public void SelectNode_FiresOnNodeSelected()
    {
        var state = new AppUiState();
        var node = new NavNode { Id = "n1", Title = "Test" };
        state.SetSideNavNodes([node]);

        NavNode? selectedNode = null;
        state.OnNodeSelected += n => selectedNode = n;
        state.SelectNode("n1");
        Assert.That(selectedNode, Is.Not.Null);
        Assert.That(selectedNode!.Id, Is.EqualTo("n1"));
    }

    [Test]
    public void ToggleNode_FlipsExpandedState()
    {
        var node = new NavNode { Id = "n1", IsExpanded = false };
        var state = new AppUiState();
        state.SetSideNavNodes([node]);

        state.ToggleNode("n1");
        Assert.That(node.IsExpanded, Is.True);

        state.ToggleNode("n1");
        Assert.That(node.IsExpanded, Is.False);
    }

    [Test]
    public void SelectNode_ExpandsParents()
    {
        var child = new NavNode { Id = "child" };
        var parent = new NavNode { Id = "parent", Children = [child], IsExpanded = false };
        var state = new AppUiState();
        state.SetSideNavNodes([parent]);

        state.SelectNode("child");
        Assert.That(parent.IsExpanded, Is.True);
    }

    [Test]
    public void ExpandToNode_ExpandsAncestors()
    {
        var grandchild = new NavNode { Id = "gc" };
        var child = new NavNode { Id = "child", Children = [grandchild], IsExpanded = false };
        var root = new NavNode { Id = "root", Children = [child], IsExpanded = false };
        var state = new AppUiState();
        state.SetSideNavNodes([root]);

        state.ExpandToNode("gc");
        Assert.That(root.IsExpanded, Is.True);
        Assert.That(child.IsExpanded, Is.True);
    }

    [Test]
    public void NavNode_Defaults()
    {
        var node = new NavNode();
        Assert.That(node.Id, Is.EqualTo(""));
        Assert.That(node.Title, Is.EqualTo(""));
        Assert.That(node.Children, Is.Empty);
        Assert.That(node.IsExpanded, Is.False);
        Assert.That(node.IsSelected, Is.False);
        Assert.That(node.Status, Is.EqualTo(Tutor.Core.Models.SectionStatus.NotStarted));
        Assert.That(node.CompletionPercentage, Is.EqualTo(0));
        Assert.That(node.Depth, Is.EqualTo(0));
    }

    [TestCase(TopTab.Home)]
    [TestCase(TopTab.Learn)]
    [TestCase(TopTab.Courses)]
    [TestCase(TopTab.KnowledgeGraph)]
    [TestCase(TopTab.Settings)]
    public void TopTab_AllValues(TopTab tab)
    {
        Assert.That(Enum.IsDefined(tab), Is.True);
    }
}
