namespace AppBase.Utils;

public static class Message
{
    public static string Error_General = "An internal error has occurred";

    public static string Field_Required = "The field is required";

    public static string Field_Optional = "The field is optional";

    public static string Field_NotEmpty = "The field is empty";

    public static string Error_Concurrency = "Resource was modified by another user";

    public static string Error_Constraint = "Resource has violated a constraint rule";

    public static string Warning_NotFound = "Resource not found";

    public static string Resource_Deleted = "Resource successfully deleted";

    public static string Feature_Type = "Feature type do not match";

    public static string Error_Reprojection = "Resource reprojection failed";

    public static string Error_PostGIS_Fn_Exec = "Error executing PostGIS function";
}