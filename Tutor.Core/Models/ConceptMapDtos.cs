using System.Text.Json.Serialization;

namespace Tutor.Core.Models;

// ============================================================================
// DTOs for AI responses during knowledge base generation.
// These define the expected JSON structure from AI API calls.
// ============================================================================

// ============================================================================
// Concept Extraction DTOs
// ============================================================================

/// <summary>
/// Response from the AI when extracting concepts from content.
/// </summary>
public class ConceptExtractionResponse
{
    /// <summary>
    /// The extracted concepts.
    /// </summary>
    [JsonPropertyName("concepts")]
    public List<ExtractedConceptDto> Concepts { get; set; } = [];
}

/// <summary>
/// A single concept as extracted by the AI.
/// </summary>
public class ExtractedConceptDto
{
    /// <summary>
    /// The canonical term or name for this concept.
    /// </summary>
    [JsonPropertyName("term")]
    public string Term { get; set; } = "";

    /// <summary>
    /// A concise definition/description of the concept.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = "";

    /// <summary>
    /// Alternative names or synonyms.
    /// </summary>
    [JsonPropertyName("aliases")]
    public List<string> Aliases { get; set; } = [];

    /// <summary>
    /// Related terms mentioned in the same context.
    /// </summary>
    [JsonPropertyName("relatedTerms")]
    public List<string> RelatedTerms { get; set; } = [];

    /// <summary>
    /// Terms that appear to be prerequisites.
    /// </summary>
    [JsonPropertyName("potentialPrerequisites")]
    public List<string> PotentialPrerequisites { get; set; } = [];

    /// <summary>
    /// Tags for categorization.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Confidence score from the AI (0.0 to 1.0).
    /// </summary>
    [JsonPropertyName("confidence")]
    public float Confidence { get; set; } = 0.8f;
}

// ============================================================================
// Topic Organization DTOs
// ============================================================================

/// <summary>
/// Response from the AI when organizing concepts into topics.
/// </summary>
public class TopicOrganizationResponse
{
    /// <summary>
    /// The organized topics.
    /// </summary>
    [JsonPropertyName("topics")]
    public List<OrganizedTopicDto> Topics { get; set; } = [];
}

/// <summary>
/// A topic as organized by the AI.
/// </summary>
public class OrganizedTopicDto
{
    /// <summary>
    /// The title of the topic.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    /// <summary>
    /// A brief summary of what this topic covers.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = "";

    /// <summary>
    /// The concept titles/terms that belong to this topic, in suggested order.
    /// </summary>
    [JsonPropertyName("conceptTerms")]
    public List<string> ConceptTerms { get; set; } = [];

    /// <summary>
    /// Suggested order for this topic (lower = earlier in curriculum).
    /// </summary>
    [JsonPropertyName("suggestedOrder")]
    public int SuggestedOrder { get; set; }

    /// <summary>
    /// Tags for categorization.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
}

// ============================================================================
// Chapter Organization DTOs
// ============================================================================

/// <summary>
/// Response from the AI when organizing topics into chapters.
/// </summary>
public class ChapterOrganizationResponse
{
    /// <summary>
    /// The organized chapters.
    /// </summary>
    [JsonPropertyName("chapters")]
    public List<OrganizedChapterDto> Chapters { get; set; } = [];
}

/// <summary>
/// A chapter as organized by the AI.
/// </summary>
public class OrganizedChapterDto
{
    /// <summary>
    /// The title of the chapter.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    /// <summary>
    /// A brief summary of what this chapter covers.
    /// </summary>
    [JsonPropertyName("summary")]
    public string Summary { get; set; } = "";

    /// <summary>
    /// The topic titles that belong to this chapter, in order.
    /// </summary>
    [JsonPropertyName("topicTitles")]
    public List<string> TopicTitles { get; set; } = [];

    /// <summary>
    /// The order of this chapter in the curriculum (0-based).
    /// </summary>
    [JsonPropertyName("order")]
    public int Order { get; set; }

    /// <summary>
    /// Optional icon suggestion.
    /// </summary>
    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    /// <summary>
    /// Tags for categorization.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];
}

// ============================================================================
// Build Progress DTOs
// ============================================================================

/// <summary>
/// Progress information during concept map building.
/// </summary>
public class ConceptMapBuildProgress
{
    /// <summary>
    /// Current status of the build.
    /// </summary>
    public ConceptMapStatus Status { get; set; }

    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// Current operation description.
    /// </summary>
    public string Message { get; set; } = "";

    /// <summary>
    /// Optional detail about the current step.
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// Number of concepts extracted so far.
    /// </summary>
    public int ConceptsExtracted { get; set; }

    /// <summary>
    /// Number of topics organized so far.
    /// </summary>
    public int TopicsOrganized { get; set; }

    /// <summary>
    /// Number of chapters organized so far.
    /// </summary>
    public int ChaptersOrganized { get; set; }

    /// <summary>
    /// Error message if status is Failed.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
