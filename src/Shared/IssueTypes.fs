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

module IssueSubtopics =

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
type Topic =
/// Research Data Management
| RDM of IssueSubtopics.RDM
| Infrastructure of IssueSubtopics.Infrastructure
| Tools of IssueSubtopics.Tools
| Workflows of IssueSubtopics.Workflows
| Metadata of IssueSubtopics.Metadata
| Other

    member this.toSubCategoryString =
        match this with
        | RDM rdm -> rdm.toString
        | Infrastructure i -> i.toString
        | Tools t -> t.toString
        | Workflows w -> w.toString
        | Metadata m -> m.toString
        | Other -> IssueGeneralTopic.Other.toString

    member this.toCategoryString =
        match this with
        | RDM _             -> IssueGeneralTopic.RDM.toString
        | Infrastructure _  -> IssueGeneralTopic.Infrastructure.toString
        | Tools _           -> IssueGeneralTopic.Tools.toString
        | Workflows _       -> IssueGeneralTopic.Workflows.toString
        | Metadata _        -> IssueGeneralTopic.Metadata.toString
        | Other             -> IssueGeneralTopic.Other.toString

/// Use this type only to create elements
[<RequireQualifiedAccess>]
type IssueGeneralTopic =
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
                Topic.RDM IssueSubtopics.RDM.AnnotationPrinciples
                Topic.RDM IssueSubtopics.RDM.ARCStructure
                Topic.RDM IssueSubtopics.RDM.DataLiteracy
                Topic.RDM IssueSubtopics.RDM.DataSecurity
                Topic.RDM IssueSubtopics.RDM.Teaching
                Topic.RDM IssueSubtopics.RDM.More
            |]
        | Infrastructure ->
            [|
                Topic.Infrastructure IssueSubtopics.Infrastructure.GitLab
                Topic.Infrastructure IssueSubtopics.Infrastructure.InfrastructureCode
                Topic.Infrastructure IssueSubtopics.Infrastructure.Invenio
                Topic.Infrastructure IssueSubtopics.Infrastructure.MetadataRegistry
                Topic.Infrastructure IssueSubtopics.Infrastructure.RegistrationLogin
                Topic.Infrastructure IssueSubtopics.Infrastructure.More
            |]
        | Tools ->
            [|
                Topic.Tools IssueSubtopics.Tools.ARCCommander
                Topic.Tools IssueSubtopics.Tools.Converters
                Topic.Tools IssueSubtopics.Tools.DMPGenerator
                Topic.Tools IssueSubtopics.Tools.Swate
                Topic.Tools IssueSubtopics.Tools.Swobup
                Topic.Tools IssueSubtopics.Tools.More
            |]
        | Workflows ->
            [|
                Topic.Workflows IssueSubtopics.Workflows.CWL
                Topic.Workflows IssueSubtopics.Workflows.Galaxy
                Topic.Workflows IssueSubtopics.Workflows.More
            |]
        | Metadata ->
            [|
                Topic.Metadata IssueSubtopics.Metadata.SwateTemplate
                Topic.Metadata IssueSubtopics.Metadata.OntologyUpdate
                Topic.Metadata IssueSubtopics.Metadata.More
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