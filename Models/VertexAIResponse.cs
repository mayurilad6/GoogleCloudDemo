namespace HCIHelp.Models
{
    public class VertexAIResponse
    {

        public List<Prediction> predictions { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Prediction
    {
        public string content { get; set; }
        public CitationMetadata citationMetadata { get; set; }
        public SafetyAttributes safetyAttributes { get; set; }
    }

    public class CitationMetadata
    {
        public List<object> citations { get; set; }
    }

    public class SafetyAttributes
    {
        public List<double> scores { get; set; }
        public bool blocked { get; set; }
        public List<string> categories { get; set; }
    }

    public class Metadata
    {
        public TokenMetadata tokenMetadata { get; set; }
    }

    public class TokenMetadata
    {
        public OutputTokenCount outputTokenCount { get; set; }
        public InputTokenCount inputTokenCount { get; set; }
    }

    public class OutputTokenCount
    {
        public int totalBillableCharacters { get; set; }
        public int totalTokens { get; set; }
    }

    public class InputTokenCount
    {
        public int totalBillableCharacters { get; set; }
        public int totalTokens { get; set; }
    }

}

