module Form

open IssueTypes

[<RequireQualifiedAccess>]
type Model = {
    IssueType : IssueTypes.IssueType
    /// Contains Category and Subcategory
    IssueTopic : IssueTypes.IssueTopic option
    IssueTitle : string
    IssueContent: string
    /// If the user wants updated on their issue, they can give us their email
    Email: string
} with
    static member init() = {
        IssueType = IssueTypes.Question
        IssueTopic = None
        IssueTitle = ""
        IssueContent = ""
        Email = ""
    }