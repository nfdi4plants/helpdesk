module IssueTypes

type IssueType =
| Question
| Bug
| Request

module Subcategories =

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
            | Galaxy    -> "Galacy"
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


[<RequireQualifiedAccess>]
type IssueSubcategory =
/// Research Data Management
| RDM of Subcategories.RDM
| Infrastructure of Subcategories.Infrastructure
| Tools of Subcategories.Tools
| Workflows of Subcategories.Workflows
| Metadata of Subcategories.Metadata
| Other

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
                IssueSubcategory.RDM Subcategories.RDM.AnnotationPrinciples
                IssueSubcategory.RDM Subcategories.RDM.ARCStructure
                IssueSubcategory.RDM Subcategories.RDM.DataLiteracy
                IssueSubcategory.RDM Subcategories.RDM.DataSecurity
                IssueSubcategory.RDM Subcategories.RDM.Teaching
                IssueSubcategory.RDM Subcategories.RDM.More
            |]
        | Infrastructure ->
            [|
                IssueSubcategory.Infrastructure Subcategories.Infrastructure.GitLab
                IssueSubcategory.Infrastructure Subcategories.Infrastructure.InfrastructureCode
                IssueSubcategory.Infrastructure Subcategories.Infrastructure.Invenio
                IssueSubcategory.Infrastructure Subcategories.Infrastructure.MetadataRegistry
                IssueSubcategory.Infrastructure Subcategories.Infrastructure.RegistrationLogin
                IssueSubcategory.Infrastructure Subcategories.Infrastructure.More
            |]
        | Tools ->
            [|
                IssueSubcategory.Tools Subcategories.Tools.ARCCommander
                IssueSubcategory.Tools Subcategories.Tools.Converters
                IssueSubcategory.Tools Subcategories.Tools.DMPGenerator
                IssueSubcategory.Tools Subcategories.Tools.Swate
                IssueSubcategory.Tools Subcategories.Tools.Swobup
                IssueSubcategory.Tools Subcategories.Tools.More
            |]
        | Workflows ->
            [|
                IssueSubcategory.Workflows Subcategories.Workflows.CWL
                IssueSubcategory.Workflows Subcategories.Workflows.Galaxy
                IssueSubcategory.Workflows Subcategories.Workflows.More
            |]
        | Metadata ->
            [|
                IssueSubcategory.Metadata Subcategories.Metadata.SwateTemplate
                IssueSubcategory.Metadata Subcategories.Metadata.OntologyUpdate
                IssueSubcategory.Metadata Subcategories.Metadata.More
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
        