namespace StockManagementDemo.Core.Common;

public class NotFoundException : Exception { public NotFoundException(string entity) : base($"{entity} not found") { } }
public class ConflictException : Exception { public ConflictException(string message) : base(message) { } }
public class DomainRuleException : Exception { public DomainRuleException(string message) : base(message) { } }
