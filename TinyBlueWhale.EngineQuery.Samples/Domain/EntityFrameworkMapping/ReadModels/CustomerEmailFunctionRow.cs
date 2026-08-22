namespace TinyBlueWhale.EngineQuery.Samples.Domain.EntityFrameworkMapping.ReadModels
{
    public sealed class CustomerEmailFunctionRow
    {
        public int CustomerId { get; set; }
        public string NormalizedEmail { get; set; } = string.Empty;
        public int EmailLength { get; set; }
        public string SafeEmail { get; set; } = string.Empty;
        public string EmailLabel { get; set; } = string.Empty;
    }
}
