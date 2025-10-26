using FastVocab.Shared.Enums;

namespace FastVocab.Shared.Utils;

public record SearchOption(
    string Property, 
    object Value,
    SearchOperator Operator = SearchOperator.Equal,
    object? OtherValue = null // for between
    );