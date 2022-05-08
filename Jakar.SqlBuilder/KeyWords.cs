namespace Jakar.SqlBuilder;


public static class KeyWords
{
    public const string SELECT             = nameof(SELECT);
    public const string FROM               = nameof(FROM);
    public const string JOIN               = nameof(JOIN);
    public const string IN                 = nameof(IN);
    public const string ON                 = nameof(ON);
    public const string AS                 = nameof(AS);
    public const string BETWEEN            = nameof(BETWEEN);
    public const string AND                = nameof(AND);
    public const string OR                 = nameof(OR);
    public const string NOT                = nameof(NOT);
    public const string WHERE              = nameof(WHERE);
    public const string LIKE               = nameof(LIKE);
    public const string COUNT              = nameof(COUNT);
    public const string AVERAGE            = nameof(AVERAGE);
    public const string SUM                = nameof(SUM);
    public const string MIN                = nameof(MIN);
    public const string TOP                = nameof(TOP);
    public const string MAX                = nameof(MAX);
    public const string DELETE             = nameof(DELETE);
    public const string UPDATE             = nameof(UPDATE);
    public const string IS                 = nameof(IS);
    public const string NULL               = nameof(NULL);
    public const string INTO               = nameof(INTO);
    public const string ASC                = nameof(ASC);
    public const string DESC               = nameof(DESC);
    public const string ORDER              = nameof(ORDER);
    public const string BY                 = nameof(BY);
    public const string DISTINCT           = nameof(DISTINCT);
    public const string LEFT               = nameof(LEFT);
    public const string FULL               = nameof(FULL);
    public const string RIGHT              = nameof(RIGHT);
    public const string INNER              = nameof(INNER);
    public const string SET                = nameof(SET);
    public const string INSERT             = nameof(INSERT);
    public const string VALUES             = nameof(VALUES);
    public const string GROUP              = nameof(GROUP);
    public const string EXISTS             = nameof(EXISTS);
    public const string FILTER             = nameof(FILTER);
    public const string GREATER            = ">";
    public const string LESS_THAN          = "<";
    public const string GREATER_OR_EQUAL   = ">=";
    public const string LESS_THAN_OR_EQUAL = "<=";
    public const string EQUAL              = "==";
    public const string NOT_EQUAL          = "!=";
    public const string ABOVE_OR_BELOW     = "<>";


    public static string GetName( this    Type   type ) => type.GetTableName();
    public static string GetName<T>( this T      _ ) => typeof(T).GetTableName();
    public static string GetName<T>( this T      _, string columnName ) => $"{typeof(T).GetTableName()}.{columnName}";
    public static string GetName<T>( this string columnName ) => $"{typeof(T).GetTableName()}.{columnName}";
}