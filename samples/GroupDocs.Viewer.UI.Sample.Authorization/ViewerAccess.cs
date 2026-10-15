namespace GroupDocs.Viewer.UI.Sample.Authorization;

// Names shared by Program.cs, the controllers and the views
public static class ViewerAccess
{
    // The authorization policy that protects both the viewer UI and the viewer API
    public const string Policy = "ViewerUsers";

    // The role that the policy requires
    public const string Role = "viewer";

    public const string UIPath = "/viewer";
    public const string ApiPath = "/viewer-api";
}
