module rec IssueTypes

type IssueType =
| Question
| Bug
| Request
    static member ofString(str) =
        match str with
        | "Question"    -> IssueType.Question
        | "Bug"         -> IssueType.Bug
        | "Request"     -> IssueType.Request
        | anythingElse  -> failwith $"Unable to parse '{anythingElse}' to IssueType."

module IssueSubcategories =

    type RDM =
    | DataSecurity
    | DataLiteracy
    | Teaching
    | ARCStructure
    | AnnotationPrinciples
    | More

    with
        member this.toString =
            match this with
            | DataSecurity          -> "Data Security"
            | DataLiteracy          -> "Data Literacy"
            | Teaching              -> "Teaching"
            | ARCStructure          -> "ARC Structure"
            | AnnotationPrinciples  -> "Annotation Principles"
            | More                  -> "More"

    type Infrastructure =
    | RegistrationLogin
    | GitLab
    | MetadataRegistry
    | Invenio
    | InfrastructureCode
    | More

    with
        member this.toString =
            match this with
            | RegistrationLogin    -> "Registration Login"
            | GitLab               -> "GitLab"
            | MetadataRegistry     -> "Metadata Registry"
            | Invenio              -> "Invenio"
            | InfrastructureCode   -> "Infrastructure Code"
            | More                 -> "More"

    type Tools =
    | ARCCommander
    | Swate
    | DMPGenerator
    | Swobup
    | Converters
    | More

    with
        member this.toString =
            match this with
            | ARCCommander  -> "ARC Commander"
            | Swate         -> "Swate"
            | DMPGenerator  -> "DMP Generator"
            | Swobup        -> "Swobup"
            | Converters    -> "Converters"
            | More          -> "More"

    type Workflows =
    | CWL
    | Galaxy
    | More

    with
        member this.toString =
            match this with
            | CWL       -> "CWL"
            | Galaxy    -> "Galaxy"
            | More      -> "More"

    type Metadata =
    | OntologyUpdate
    | SwateTemplate
    | More

    with
        member this.toString =
            match this with
            | OntologyUpdate    -> "Ontology Update"
            | SwateTemplate     -> "Swate Template"
            | More              -> "More"

/// This should be the main type for working with issue categories
type IssueTopic =
/// Research Data Management
| RDM of IssueSubcategories.RDM
| Infrastructure of IssueSubcategories.Infrastructure
| Tools of IssueSubcategories.Tools
| Workflows of IssueSubcategories.Workflows
| Metadata of IssueSubcategories.Metadata
| Other

    member this.toSubCategoryString =
        match this with
        | RDM rdm -> rdm.toString
        | Infrastructure i -> i.toString
        | Tools t -> t.toString
        | Workflows w -> w.toString
        | Metadata m -> m.toString
        | Other -> IssueCategory.Other.toString

    member this.toCategoryString =
        match this with
        | RDM _             -> IssueCategory.RDM.toString
        | Infrastructure _  -> IssueCategory.Infrastructure.toString
        | Tools _           -> IssueCategory.Tools.toString
        | Workflows _       -> IssueCategory.Workflows.toString
        | Metadata _        -> IssueCategory.Metadata.toString
        | Other             -> IssueCategory.Other.toString

/// Use this type only to create elements
[<RequireQualifiedAccess>]
type IssueCategory =
/// Research Data Management
| RDM
| Infrastructure   
| Tools
| Workflows
| Metadata
| Other

with
    static member ofString (str:string) =
        match str with
        | "RDM" | "Research Data Management"    -> RDM
        | "Infrastructure"                      -> Infrastructure
        | "Tools"                               -> Tools
        | "Workflows"                           -> Workflows
        | "Metadata"                            -> Metadata
        | "Other"                               -> Other
        | anythingElse -> failwith $"Unable to parse '{anythingElse}' to IssueCategory."

    member this.toString =
        match this with
        | RDM               -> "Research Data Management"
        | Infrastructure    -> "Infrastructure"
        | Tools             -> "Tools"
        | Workflows         -> "Workflows"
        | Metadata          -> "Metadata"
        | Other             -> "Other"

    member this.subcategories =
        match this with
        | RDM ->
            [|
                IssueTopic.RDM IssueSubcategories.RDM.AnnotationPrinciples
                IssueTopic.RDM IssueSubcategories.RDM.ARCStructure
                IssueTopic.RDM IssueSubcategories.RDM.DataLiteracy
                IssueTopic.RDM IssueSubcategories.RDM.DataSecurity
                IssueTopic.RDM IssueSubcategories.RDM.Teaching
                IssueTopic.RDM IssueSubcategories.RDM.More
            |]
        | Infrastructure ->
            [|
                IssueTopic.Infrastructure IssueSubcategories.Infrastructure.GitLab
                IssueTopic.Infrastructure IssueSubcategories.Infrastructure.InfrastructureCode
                IssueTopic.Infrastructure IssueSubcategories.Infrastructure.Invenio
                IssueTopic.Infrastructure IssueSubcategories.Infrastructure.MetadataRegistry
                IssueTopic.Infrastructure IssueSubcategories.Infrastructure.RegistrationLogin
                IssueTopic.Infrastructure IssueSubcategories.Infrastructure.More
            |]
        | Tools ->
            [|
                IssueTopic.Tools IssueSubcategories.Tools.ARCCommander
                IssueTopic.Tools IssueSubcategories.Tools.Converters
                IssueTopic.Tools IssueSubcategories.Tools.DMPGenerator
                IssueTopic.Tools IssueSubcategories.Tools.Swate
                IssueTopic.Tools IssueSubcategories.Tools.Swobup
                IssueTopic.Tools IssueSubcategories.Tools.More
            |]
        | Workflows ->
            [|
                IssueTopic.Workflows IssueSubcategories.Workflows.CWL
                IssueTopic.Workflows IssueSubcategories.Workflows.Galaxy
                IssueTopic.Workflows IssueSubcategories.Workflows.More
            |]
        | Metadata ->
            [|
                IssueTopic.Metadata IssueSubcategories.Metadata.SwateTemplate
                IssueTopic.Metadata IssueSubcategories.Metadata.OntologyUpdate
                IssueTopic.Metadata IssueSubcategories.Metadata.More
            |]
        | Other ->
            [||]

    //member this.toStringExtended =
    //    match this with
    //    | RDM rdm           -> if rdm.IsNone then this.toString else $"{this.toString} {rdm.Value.toString}" 
    //    | Infrastructure i  -> if i.IsNone then this.toString else $"{this.toString} {i.Value.toString}" 
    //    | Tools t           -> if t.IsNone then this.toString else $"{this.toString} {t.Value.toString}" 
    //    | Workflows w       -> if w.IsNone then this.toString else $"{this.toString} {w.Value.toString}" 
    //    | Metadata m        -> if m.IsNone then this.toString else $"{this.toString} {m.Value.toString}" 
    //    | Other             -> "Other"