module rec IssueTypes

open System

type IssueType =
| Question
| Bug
| Request
    static member ofString(str: string) =
        match str.ToLower() with
        | "question"    -> IssueType.Question
        | "bug"         -> IssueType.Bug
        | "request"     -> IssueType.Request
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

        static member ofString(str: string) =
            match str.ToLower() with
            | "data security" | "datasecurity"                  -> DataSecurity
            | "data literacy" | "dataliteracy"                  -> DataLiteracy
            | "teaching"                                        -> Teaching
            | "arc structure" | "arcstructure"                  -> ARCStructure
            | "annotation principles"| "annotationprinciples"   -> AnnotationPrinciples
            | "more"                                            -> More
            | anythingElse -> failwith $"Could not parse {anythingElse} to RDM subtopic"

    type Infrastructure =
    | RegistrationLogin
    | DataHUB
    | MetadataRegistry
    | Invenio
    | InfrastructureCode
    | More

    with
        member this.toString =
            match this with
            | RegistrationLogin     -> "Registration Login"
            | DataHUB               -> "DataHUB"
            | MetadataRegistry      -> "Metadata Registry"
            | Invenio               -> "Invenio"
            | InfrastructureCode    -> "Infrastructure Code"
            | More                  -> "More"

        static member ofString(str: string) =
            match str.ToLower() with
            | "registration login"  | "registrationlogin"    -> RegistrationLogin
            | "gitlab" | "datahub" -> DataHUB
            | "metadata registry"   | "metadataregistry"     -> MetadataRegistry
            | "invenio" -> Invenio
            | "infrastructure code" | "infrastructurecode"   -> InfrastructureCode
            | "more" -> More
            | anythingElse -> failwith $"Could not parse {anythingElse} to Infrastructure subtopic"

    type Tools =
    | ARCCommander
    | ARCitect
    | DataHUB
    | Swate
    | DataPLAN
    | Swobup
    | Converters
    | More

    with
        member this.toString =
            match this with
            | ARCCommander  -> "ARC Commander"
            | ARCitect      -> "ARCitect"
            | Swate         -> "Swate"
            | DataHUB      -> "DataHUB"
            | DataPLAN      -> "DataPLAN"
            | Swobup        -> "Swobup"
            | Converters    -> "Converters"
            | More          -> "More"

        static member ofString(str: string) =
            match str.ToLower() with
            | "arc commander" | "arccommander"   -> ARCCommander
            | "arcitect"        -> ARCitect
            | "swate"           -> Swate
            | "datahub" | "gitlab" -> DataHUB
            | "dmp generator" | "dmpgenerator" | "dataplan" -> DataPLAN
            | "swobup"          -> Swobup
            | "converters"      -> Converters
            | "more"            -> More
            | anythingElse -> failwith $"Could not parse {anythingElse} to Tools subtopic"

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

        static member ofString(str: string) =
            match str.ToLower() with
            | "cwl"    -> CWL
            | "galaxy" -> Galaxy
            | "more"   -> More
            | anythingElse -> failwith $"Could not parse {anythingElse} to Workflows subtopic"

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

        static member ofString(str: string) =
            match str.ToLower() with
            | "ontology update" | "ontologyupdate" ->  OntologyUpdate
            | "swate template" | "swatetemplate" ->  SwateTemplate
            | "more" -> More
            | anythingElse -> failwith $"Could not parse {anythingElse} to Metadata subtopic"

    type Documentation =
    | KnowledgeBase
    | More

    with
        member this.toString =
            match this with
            | KnowledgeBase    -> "Knowledge Base"
            | More     -> "More"

        static member ofString(str: string) =
            match str.ToLower() with
            | "knowledge base" | "knowledgebase" ->  KnowledgeBase
            | "more" -> More
            | anythingElse -> failwith $"Could not parse {anythingElse} to Metadata subtopic"

/// This should be the main type for working with issue categories
type Topic =
/// Research Data Management
| RDM of IssueSubtopics.RDM
| Infrastructure of IssueSubtopics.Infrastructure
| Tools of IssueSubtopics.Tools
| Workflows of IssueSubtopics.Workflows
| Metadata of IssueSubtopics.Metadata
| Documentation of IssueSubtopics.Documentation
| Other

    member this.toSubCategoryString =
        match this with
        | RDM rdm -> rdm.toString
        | Infrastructure i -> i.toString
        | Tools t -> t.toString
        | Workflows w -> w.toString
        | Metadata m -> m.toString
        | Documentation d -> d.toString
        | Other -> IssueGeneralTopic.Other.toString

    member this.toCategoryString =
        match this with
        | RDM _             -> IssueGeneralTopic.RDM.toString
        | Infrastructure _  -> IssueGeneralTopic.Infrastructure.toString
        | Tools _           -> IssueGeneralTopic.Tools.toString
        | Workflows _       -> IssueGeneralTopic.Workflows.toString
        | Metadata _        -> IssueGeneralTopic.Metadata.toString
        | Documentation _    -> IssueGeneralTopic.Documentation.toString
        | Other             -> IssueGeneralTopic.Other.toString

    member this.toUrlString =
        let rmWhitespace(str:string) = str.Replace(" ","")
        match this with
        | RDM rdm           -> $"{rmWhitespace IssueGeneralTopic.RDM.toString}_{rmWhitespace rdm.toString}"
        | Infrastructure i  -> $"{rmWhitespace IssueGeneralTopic.Infrastructure.toString}_{rmWhitespace i.toString}"
        | Tools t           -> $"{rmWhitespace IssueGeneralTopic.Tools.toString}_{rmWhitespace t.toString}"
        | Workflows w       -> $"{rmWhitespace IssueGeneralTopic.Workflows.toString}_{rmWhitespace w.toString}"
        | Metadata m        -> $"{rmWhitespace IssueGeneralTopic.Metadata.toString}_{rmWhitespace m.toString}"
        | Documentation d   -> $"{rmWhitespace IssueGeneralTopic.Documentation.toString}_{rmWhitespace d.toString}"
        | Other             -> rmWhitespace IssueGeneralTopic.Other.toString

    static member ofUrlString(str:string) =
        let majorTopic, subtopic =
            let ind = str.IndexOf("_")
            str.[..ind-1], str.[ind+1..]
        match IssueGeneralTopic.ofString majorTopic with
        | IssueGeneralTopic.RDM            ->
            let subtopic = IssueSubtopics.RDM.ofString subtopic
            Topic.RDM subtopic
        | IssueGeneralTopic.Infrastructure ->
            let subtopic = IssueSubtopics.Infrastructure.ofString subtopic
            Topic.Infrastructure subtopic
        | IssueGeneralTopic.Tools          ->
            let subtopic = IssueSubtopics.Tools.ofString subtopic
            Topic.Tools subtopic
        | IssueGeneralTopic.Workflows      ->
            let subtopic = IssueSubtopics.Workflows.ofString subtopic
            Topic.Workflows subtopic
        | IssueGeneralTopic.Metadata       ->
            let subtopic = IssueSubtopics.Metadata.ofString subtopic
            Topic.Metadata subtopic
        | IssueGeneralTopic.Documentation   ->
            let subtopic = IssueSubtopics.Documentation.ofString subtopic
            Topic.Documentation subtopic
        | IssueGeneralTopic.Other          ->
            Topic.Other

/// Use this type only to create elements
[<RequireQualifiedAccess>]
type IssueGeneralTopic =
/// Research Data Management
| RDM
| Infrastructure
| Tools
| Workflows
| Metadata
| Documentation
| Other

with
    static member ofString (str:string) =
        match str.ToLower() with
        | "rdm" | "research data management" | "researchdatamanagement" -> RDM
        | "infrastructure"                      -> Infrastructure
        | "tools" | "tools and services" | "toolsandservices" -> Tools
        | "workflows"                           -> Workflows
        | "metadata" | "metadata and ontologies" | "metadataandontologies" -> Metadata
        | "documentation"                       -> Documentation
        | "other"                               -> Other
        | anythingElse -> failwith $"Unable to parse '{anythingElse}' to IssueCategory."

    member this.toString =
        match this with
        | RDM               -> "Research Data Management"
        | Infrastructure    -> "Infrastructure"
        | Tools             -> "Tools and Services"
        | Workflows         -> "Workflows"
        | Metadata          -> "Metadata and Ontologies"
        | Documentation     -> "Documentation"
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
                Topic.Infrastructure IssueSubtopics.Infrastructure.DataHUB
                Topic.Infrastructure IssueSubtopics.Infrastructure.InfrastructureCode
                Topic.Infrastructure IssueSubtopics.Infrastructure.Invenio
                Topic.Infrastructure IssueSubtopics.Infrastructure.MetadataRegistry
                Topic.Infrastructure IssueSubtopics.Infrastructure.RegistrationLogin
                Topic.Infrastructure IssueSubtopics.Infrastructure.More
            |]
        | Tools ->
            [|
                Topic.Tools IssueSubtopics.Tools.ARCCommander
                Topic.Tools IssueSubtopics.Tools.ARCitect
                Topic.Tools IssueSubtopics.Tools.Converters
                Topic.Tools IssueSubtopics.Tools.DataHUB
                Topic.Tools IssueSubtopics.Tools.DataPLAN
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
        | Documentation ->
            [|
                Topic.Documentation IssueSubtopics.Documentation.KnowledgeBase
                Topic.Documentation IssueSubtopics.Documentation.More
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