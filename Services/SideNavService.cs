using Tutor.Models;

namespace Tutor.Services;

/// <summary>
/// Service for populating side navigation based on the active context.
/// 
/// The Learn page has two navigation views:
/// 1. Table of Contents: Static hierarchical navigation (Lessons -> Topics -> Concepts)
/// 2. ConceptMap view: Concepts organized by relationships and complexity
/// 
/// Both views reference Concepts from the ConceptMap by ID.
/// </summary>
public sealed class SideNavService
{
    private readonly CourseService courseService;
    private readonly ConceptMapStorageService conceptMapStorageService;
    private readonly CourseStructureStorageService structureStorageService;
    private readonly UserProgressService progressService;
    private readonly AppUiState uiState;

    public SideNavService(
        CourseService courseService,
        ConceptMapStorageService conceptMapStorageService,
        CourseStructureStorageService structureStorageService,
        UserProgressService progressService,
        AppUiState uiState)
    {
        this.courseService = courseService;
        this.conceptMapStorageService = conceptMapStorageService;
        this.structureStorageService = structureStorageService;
        this.progressService = progressService;
        this.uiState = uiState;
    }

    /// <summary>
    /// Populates the side nav with the course curriculum (Lessons -> Topics -> Concepts).
    /// This is the Table of Contents view for guided learning.
    /// </summary>
    public async Task PopulateLearnNavAsync(string? courseId = null, CancellationToken ct = default)
    {
        var nodes = new List<NavNode>();

        // Get the active course if not specified
        if (string.IsNullOrEmpty(courseId))
        {
            var activeCourse = await courseService.GetActiveCourseAsync();
            courseId = activeCourse?.Id;
        }

        if (string.IsNullOrEmpty(courseId))
        {
            uiState.SetSideNavNodes(nodes);
            return;
        }

        // Get the course
        var course = await courseService.GetCourseAsync(courseId);
        if (course == null)
        {
            uiState.SetSideNavNodes(nodes);
            return;
        }

        // Get user progress for this course
        var progress = await progressService.GetProgressAsync(courseId);

        // Check if course has required components
        if (!course.HasConceptMapCollection || !course.HasCourseStructure)
        {
            nodes.Add(new NavNode
            {
                Id = "no-content",
                Title = "No curriculum available",
                Icon = "bi-info-circle",
                Description = "Build concept maps and generate course structure in Courses."
            });
            uiState.SetSideNavNodes(nodes);
            return;
        }

        // Load ConceptMap collection and CourseStructure
        var conceptMap = await conceptMapStorageService.LoadAsync(course.ConceptMapCollectionId!, ct);
        var structure = await structureStorageService.LoadAsync(course.CourseStructureId!, ct);

        if (conceptMap == null || conceptMap.Status != ConceptMapStatus.Ready)
        {
            nodes.Add(new NavNode
            {
                Id = "kb-not-ready",
                Title = "Concept maps not ready",
                Icon = "bi-hourglass-split",
                Description = "The concept maps are still being built."
            });
            uiState.SetSideNavNodes(nodes);
            return;
        }

        if (structure == null || structure.Status != CourseStructureStatus.Ready)
        {
            nodes.Add(new NavNode
            {
                Id = "structure-not-ready",
                Title = "Course structure not ready",
                Icon = "bi-hourglass-split",
                Description = "The course structure is still being generated."
            });
            uiState.SetSideNavNodes(nodes);
            return;
        }

        // Build navigation from CourseStructure (references Concepts in ConceptMap)
        nodes = BuildNavFromCourseStructure(structure, conceptMap, progress);

        // Expand to current concept
        if (!string.IsNullOrEmpty(progress.CurrentConceptId))
        {
            ExpandToCurrentConcept(nodes, progress.CurrentConceptId);
        }
        else if (nodes.Count > 0)
        {
            // Expand the first lesson by default
            nodes[0].IsExpanded = true;
        }


        uiState.SetSideNavNodes(nodes);
    }

    /// <summary>
    /// Builds navigation nodes from CourseStructure.
    /// The structure references Concepts in the ConceptMap by ID.
    /// Hierarchy: Lessons (Chapters) -> Sections -> Subsections -> Concepts
    /// Falls back to: Lessons -> Topics -> Concepts if no sections exist
    /// </summary>
    private List<NavNode> BuildNavFromCourseStructure(
        CourseStructure structure, 
        ConceptMap conceptMap, 
        UserProgress progress)
    {
        var nodes = new List<NavNode>();

        foreach (var lesson in structure.GetLessonsInOrder())
        {
            var lessonNode = new NavNode
            {
                Id = $"lesson-{lesson.Id}",
                Title = lesson.Title,
                Icon = lesson.Icon ?? "bi-journal-text",
                Description = lesson.Summary,
                IsExpanded = false,
                Data = lesson
            };

            // Check if lesson has hierarchical sections
            if (lesson.Sections.Count > 0)
            {
                // Use sections hierarchy (new encyclopedia-style navigation)
                foreach (var section in lesson.Sections.OrderBy(s => s.Order))
                {
                    var sectionNode = BuildSectionNavNode(section, conceptMap, progress);
                    lessonNode.Children.Add(sectionNode);
                }

                // Calculate lesson progress from sections
                var totalSections = lesson.GetTotalSectionCount();
                var completedSections = lesson.GetAllSectionsFlattened()
                    .Count(s => progress.GetSectionStatus(s.Id) == SectionStatus.Complete);
                if (totalSections > 0)
                {
                    var pct = (int)((double)completedSections / totalSections * 100);
                    lessonNode.Description = $"{completedSections}/{totalSections} sections ({pct}%)";
                }
            }
            else
            {
                // Fallback: use topics hierarchy (original navigation)
                foreach (var topic in lesson.Topics.OrderBy(t => t.Order))
                {
                    var topicNode = new NavNode
                    {
                        Id = $"topic-{topic.Id}",
                        Title = topic.Title,
                        Icon = topic.Icon ?? "bi-bookmark",
                        Description = topic.Summary,
                        IsExpanded = false,
                        Data = topic
                    };

                    // Add concepts as children (look up from ConceptMap)
                    foreach (var conceptId in topic.ConceptIds)
                    {
                        var concept = conceptMap.GetConcept(conceptId);
                        if (concept == null) continue;

                        var isLearned = progress.IsConceptLearned(conceptId);
                        var isVisited = progress.IsConceptVisited(conceptId);

                        var conceptNode = new NavNode
                        {
                            Id = $"concept-{conceptId}",
                            Title = concept.Title,
                            Icon = isLearned ? "bi-check-circle-fill" : (isVisited ? "bi-circle-half" : "bi-circle"),
                            Data = concept
                        };

                        topicNode.Children.Add(conceptNode);
                    }

                    // Calculate topic progress
                    var totalInTopic = topic.ConceptIds.Count;
                    var learnedInTopic = topic.ConceptIds.Count(id => progress.IsConceptLearned(id));
                    if (totalInTopic > 0)
                    {
                        var pct = (int)((double)learnedInTopic / totalInTopic * 100);
                        topicNode.Description = $"{learnedInTopic}/{totalInTopic} ({pct}%)";
                    }

                    lessonNode.Children.Add(topicNode);
                }

                // Calculate lesson progress from concepts
                var totalInLesson = lesson.TotalConceptCount;
                var learnedInLesson = lesson.GetAllConceptIds().Count(id => progress.IsConceptLearned(id));
                if (totalInLesson > 0)
                {
                    var pct = (int)((double)learnedInLesson / totalInLesson * 100);
                    lessonNode.Description = $"{learnedInLesson}/{totalInLesson} concepts ({pct}%)";
                }
            }

            nodes.Add(lessonNode);
        }

        return nodes;
    }

    /// <summary>
    /// Builds a navigation node for a section and its children recursively.
    /// </summary>
    private NavNode BuildSectionNavNode(Section section, ConceptMap conceptMap, UserProgress progress)
    {
        var status = progress.GetSectionStatus(section.Id);
        var icon = GetSectionIcon(section, status);
        
        var sectionNode = new NavNode
        {
            Id = $"section-{section.Id}",
            Title = $"{section.Number}. {section.Title}",
            Icon = icon,
            Description = section.Summary,
            IsExpanded = false,
            Data = section
        };

        // Add child sections recursively
        foreach (var child in section.Children.OrderBy(c => c.Order))
        {
            var childNode = BuildSectionNavNode(child, conceptMap, progress);
            sectionNode.Children.Add(childNode);
        }

        // If this is a leaf section (no children), add concepts
        if (section.Children.Count == 0 && section.ConceptIds.Count > 0)
        {
            foreach (var conceptId in section.ConceptIds)
            {
                var concept = conceptMap.GetConcept(conceptId);
                if (concept == null) continue;

                var isLearned = progress.IsConceptLearned(conceptId);
                var isVisited = progress.IsConceptVisited(conceptId);

                var conceptNode = new NavNode
                {
                    Id = $"concept-{conceptId}",
                    Title = concept.Title,
                    Icon = isLearned ? "bi-check-circle-fill" : (isVisited ? "bi-circle-half" : "bi-circle"),
                    Data = concept
                };

                sectionNode.Children.Add(conceptNode);
            }
        }

        // Update description with progress and quiz indicator
        var progressText = GetSectionProgressText(section, progress);
        var quizIndicator = section.HasQuiz ? " 📝" : "";
        sectionNode.Description = $"{progressText}{quizIndicator}";

        return sectionNode;
    }

    /// <summary>
    /// Gets the appropriate icon for a section based on its status.
    /// </summary>
    private static string GetSectionIcon(Section section, SectionStatus status)
    {
        // Use section's custom icon if provided
        if (!string.IsNullOrEmpty(section.Icon))
            return section.Icon;

        // Otherwise use status-based icons
        return status switch
        {
            SectionStatus.Complete => "bi-check-circle-fill",
            SectionStatus.Read => "bi-book-fill",
            SectionStatus.Visited => "bi-circle-half",
            _ => section.Depth switch
            {
                0 => "bi-folder",
                1 => "bi-file-text",
                2 => "bi-file-earmark-text",
                _ => "bi-dot"
            }
        };
    }

    /// <summary>
    /// Gets progress text for a section.
    /// </summary>
    private static string GetSectionProgressText(Section section, UserProgress progress)
    {
        if (section.Children.Count == 0)
        {
            // Leaf section - show concept progress
            var total = section.ConceptIds.Count;
            var learned = section.ConceptIds.Count(id => progress.IsConceptLearned(id));
            if (total > 0)
            {
                var pct = (int)((double)learned / total * 100);
                return $"{learned}/{total} ({pct}%)";
            }
            return section.Summary ?? "";
        }
        else
        {
            // Parent section - show child section progress
            var allChildren = GetAllDescendantSections(section);
            var total = allChildren.Count;
            var completed = allChildren.Count(s => progress.GetSectionStatus(s.Id) == SectionStatus.Complete);
            if (total > 0)
            {
                var pct = (int)((double)completed / total * 100);
                return $"{completed}/{total} subsections ({pct}%)";
            }
            return section.Summary ?? "";
        }
    }

    /// <summary>
    /// Gets all descendant sections (flattened) of a section.
    /// </summary>
    private static List<Section> GetAllDescendantSections(Section section)
    {
        var result = new List<Section>();
        foreach (var child in section.Children)
        {
            result.Add(child);
            result.AddRange(GetAllDescendantSections(child));
        }
        return result;
    }

    /// <summary>
    /// Builds navigation nodes for the ConceptMap view.
    /// This shows Concepts organized by complexity level, not the course hierarchy.
    /// Allows exploration of related concepts outside the current lesson.
    /// </summary>
    public List<NavNode> BuildConceptMapNav(ConceptMap conceptMap, UserProgress progress)
    {
        var nodes = new List<NavNode>();
        var maxLevel = conceptMap.MaxComplexityLevel;

        // Group concepts by complexity level
        for (int level = 0; level <= maxLevel; level++)
        {
            var conceptsAtLevel = conceptMap.GetConceptsAtLevel(level).ToList();
            if (conceptsAtLevel.Count == 0) continue;

            var levelNode = new NavNode
            {
                Id = $"level-{level}",
                Title = GetLevelTitle(level, maxLevel),
                Icon = GetLevelIcon(level, maxLevel),
                Description = $"{conceptsAtLevel.Count} concepts",
                IsExpanded = level == 0 // Expand foundational level by default
            };

            foreach (var concept in conceptsAtLevel.OrderBy(c => c.Title))
            {
                var isLearned = progress.IsConceptLearned(concept.Id);
                var isVisited = progress.IsConceptVisited(concept.Id);
                var complexity = conceptMap.GetComplexity(concept.Id);

                var conceptNode = new NavNode
                {
                    Id = $"concept-{concept.Id}",
                    Title = concept.Title,
                    Icon = isLearned ? "bi-check-circle-fill" : (isVisited ? "bi-circle-half" : "bi-circle"),
                    Description = $"{complexity?.PrerequisiteCount ?? 0} prerequisites",
                    Data = concept
                };

                levelNode.Children.Add(conceptNode);
            }

            // Calculate level progress
            var learnedAtLevel = conceptsAtLevel.Count(c => progress.IsConceptLearned(c.Id));
            var pct = (int)((double)learnedAtLevel / conceptsAtLevel.Count * 100);
            levelNode.Description = $"{learnedAtLevel}/{conceptsAtLevel.Count} learned ({pct}%)";

            nodes.Add(levelNode);
        }

        return nodes;
    }

    private static string GetLevelTitle(int level, int maxLevel)
    {
        if (level == 0) return "Foundational";
        if (level == maxLevel) return "Advanced";
        if (level <= maxLevel / 3) return "Basic";
        if (level <= 2 * maxLevel / 3) return "Intermediate";
        return "Advanced";
    }

    private static string GetLevelIcon(int level, int maxLevel)
    {
        if (level == 0) return "bi-1-circle";
        if (level == maxLevel) return "bi-star";
        return $"bi-{Math.Min(level + 1, 9)}-circle";
    }

    private static void ExpandToCurrentConcept(List<NavNode> nodes, string conceptId)
    {
        foreach (var node in nodes)
        {
            if (ExpandToConceptRecursive(node, conceptId))
                return;
        }

        // If not found, expand first node
        if (nodes.Count > 0)
            nodes[0].IsExpanded = true;
    }

    private static bool ExpandToConceptRecursive(NavNode node, string conceptId)
    {
        if (node.Id == $"concept-{conceptId}")
            return true;

        foreach (var child in node.Children)
        {
            if (ExpandToConceptRecursive(child, conceptId))
            {
                node.IsExpanded = true;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Populates the side nav with the list of courses.
    /// </summary>
    public async Task PopulateCoursesNavAsync(CancellationToken ct = default)
    {
        var nodes = new List<NavNode>();

        var courses = await courseService.GetAllCoursesAsync();
        var activeCourse = await courseService.GetActiveCourseAsync();

        if (courses.Count == 0)
        {
            nodes.Add(new NavNode
            {
                Id = "no-courses",
                Title = "No courses yet",
                Icon = "bi-info-circle",
                Description = "Create a course in the Courses page."
            });
        }
        else
        {
            foreach (var course in courses.OrderBy(c => c.Name))
            {
                var isActive = course.Id == activeCourse?.Id;

                nodes.Add(new NavNode
                {
                    Id = $"course-{course.Id}",
                    Title = course.Name,
                    Icon = isActive ? "bi-book-fill" : "bi-book",
                    Description = course.Description,
                    Data = course
                });
            }
        }

        uiState.SetSideNavNodes(nodes);
    }

    /// <summary>
    /// Populates the side nav with settings sections.
    /// </summary>
    public void PopulateSettingsNav()
    {
        var nodes = new List<NavNode>
        {
            new()
            {
                Id = "settings-general",
                Title = "General",
                Icon = "bi-gear"
            },
            new()
            {
                Id = "settings-courses",
                Title = "Courses",
                Icon = "bi-book"
            },
            new()
            {
                Id = "settings-appearance",
                Title = "Appearance",
                Icon = "bi-palette"
            },
            new()
            {
                Id = "settings-storage",
                Title = "Storage",
                Icon = "bi-hdd"
            },
            new()
            {
                Id = "settings-quiz",
                Title = "Quiz",
                Icon = "bi-question-circle"
            },
            new()
            {
                Id = "settings-ai",
                Title = "AI Models",
                Icon = "bi-robot",
                IsExpanded = false,
                Children =
                [
                    new NavNode { Id = "settings-chatgpt", Title = "ChatGPT", Icon = "bi-chat" },
                    new NavNode { Id = "settings-claude", Title = "Claude", Icon = "bi-chat" },
                    new NavNode { Id = "settings-gemini", Title = "Gemini", Icon = "bi-chat" }
                ]
            },
            new()
            {
                Id = "settings-about",
                Title = "About",
                Icon = "bi-info-circle"
            }
        };

        uiState.SetSideNavNodes(nodes);
    }

    /// <summary>
    /// Clears the side nav.
    /// </summary>
    public void ClearSideNav()
    {
        uiState.SetSideNavNodes([]);
    }
}
